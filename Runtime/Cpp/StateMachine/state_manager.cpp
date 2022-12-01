#include <stdio.h>
#include "state_manager.h"
#include "state_base.h"
#include "state_token.h"
#include "state_data.h"
#include "exceptions.h"

using namespace std;

namespace Bitcraft
{
    namespace StateMachine
    {
        void* StateManager::GetContext()
        {
            return _context;
        }

        StateBase* StateManager::GetCurrentState()
        {
            return _currentState;
        }

        const StateToken* const StateManager::GetCurrentStateToken() const
        {
            return _currentState != NULL ? _currentState->GetToken() : NULL;
        }

        StateManager::StateManager()
        {
            InternalInitialization(NULL);
        }

        StateManager::StateManager(void* context)
        {
            InternalInitialization(context);
        }

        void StateManager::InternalInitialization(void* context)
        {
            _context = context;
            _currentState = NULL;
            _isPerformActionLocked = false;
        }

        void StateManager::SetInitialState(const StateToken* const initialState, StateData* data)
        {
            if (initialState == NULL)
                throw new invalid_argument("Invalid 'initialState' argument.");

            if (_currentState != NULL)
            {
                _isPerformActionLocked = true;
                StateExitEventArgs stateExitEventArgs = StateExitEventArgs(NULL, initialState, data);
                _currentState->OnExit(&stateExitEventArgs);
                _isPerformActionLocked = false;
            }

            _currentState = NULL;
            PerformTransitionTo(nullptr, initialState, data);
        }

        void StateManager::PerformTransitionTo(const ActionToken* actionToken, const StateToken* stateToken, StateData* data)
        {
            const ActionToken* triggeringActionToken = actionToken;
            const StateToken* targetStateToken = stateToken;
            StateData* targetData = data;

            while (true)
            {
                TransitionInfo* transition = TransitionTo(triggeringActionToken, targetStateToken, targetData);
                if (transition->TargetState == NULL)
                    break;

                triggeringActionToken = transition->TriggeringAction;
                targetStateToken = transition->TargetState;
                targetData = transition->TargetStateData;
            }
        }

        TransitionInfo* StateManager::TransitionTo(const ActionToken* actionToken, const StateToken* const stateToken, StateData* data)
        {
            if (stateToken == NULL)
                throw new invalid_argument("Invalid 'stateToken' argument.");

            StateBase* state = FindState(stateToken);
            if (state == NULL)
                throw new UnknownStateException(GetCurrentStateToken(), stateToken);

            if (_currentState != NULL)
            {
                _isPerformActionLocked = true;
                StateExitEventArgs stateExitEventArgs = StateExitEventArgs(actionToken, stateToken, data);
                _currentState->OnExit(&stateExitEventArgs);
                _isPerformActionLocked = false;
            }

            StateBase* oldState = _currentState;
            _currentState = state;

            _isPerformActionLocked = true;

            StateChangedEventArgs stateChangedEventArgs = StateChangedEventArgs(actionToken, oldState, _currentState);
            OnStateChanged(&stateChangedEventArgs);

            StateEnterEventArgs stateEnterEventArgs = StateEnterEventArgs(actionToken, oldState != NULL ? oldState->GetToken() : NULL, data);
            _currentState->OnEnter(&stateEnterEventArgs);

            _isPerformActionLocked = false;

            return stateEnterEventArgs.GetRedirect();
        }

        StateBase* StateManager::FindState(const StateToken* const token)
        {
            if (token == NULL)
                throw new invalid_argument("Invalid 'stateToken' argument.");

            for (list<StateBase*>::const_iterator it = _states.begin(), end = _states.end(); it != end; it++)
            {
                StateBase* currentState = *it;

                const StateToken* const currentToken = currentState->GetToken();
                if (currentToken == NULL)
                    continue;

                if (currentToken->Equals(token))
                    return currentState;
            }

            return NULL;
        }

        bool StateManager::StateExists(StateBase* state)
        {
            if (state == NULL)
                return false;

            for (list<StateBase*>::const_iterator it = _states.begin(), end = _states.end(); it != end; it++)
            {
                if ((*it)->GetToken()->Equals(state->GetToken()))
                    return true;
            }

            return false;
        }

        void StateManager::PerformAction(ActionToken* action)
        {
            PerformAction(action, NULL);
        }

        void StateManager::PerformAction(ActionToken* action, StateData* data)
        {
            if (action == NULL)
                throw new invalid_argument("Invalid 'action' argument.");

            if (_currentState == NULL)
                throw new exception("State machine not yet initialized or has reached its final state.");

            if (_isPerformActionLocked)
                return; // not that good :/

            TransitionInfo transitionInfo;
            transitionInfo.TargetState = NULL;
            transitionInfo.TargetStateData = data;

            _currentState->Handle(action, data, &transitionInfo);
            if (transitionInfo.TargetState == NULL)
            {
                _currentState = NULL;
                _isPerformActionLocked = true;
                OnCompleted();
                _isPerformActionLocked = false;
                return;
            }

            if (_currentState->GetToken() != transitionInfo.TargetState)
                PerformTransitionTo(action, transitionInfo.TargetState, transitionInfo.TargetStateData);
        }

        void StateManager::RegisterState(StateBase* state)
        {
            if (state == NULL)
                throw new invalid_argument("Invalid 'state' argument.");

            if (StateExists(state))
                throw new invalid_argument("State already registered.");

            _states.push_back(state);

            _isPerformActionLocked = true;
            state->Initialize(this);
            _isPerformActionLocked = false;
        }
    }
}
