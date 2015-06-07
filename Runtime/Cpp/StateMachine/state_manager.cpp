#include <stdio.h>
#include "state_manager.h"
#include "state_base.h"
#include "state_token.h"
#include "state_data.h"
#include "exceptions.h"

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

        StateToken* StateManager::GetCurrentStateToken()
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
        }

        void StateManager::SetInitialState(StateToken* initialState, StateData* data)
        {
            if (initialState == NULL)
                throw new invalid_argument("Invalid 'initialState' argument.");

            _currentState = NULL;
            PerformTransitionTo(initialState, data);
        }

        void StateManager::PerformTransitionTo(StateToken* stateToken, StateData* data)
        {
            StateToken* targetStateToken = stateToken;
            StateData* targetData = data;

            while (true)
            {
                TransitionInfo* transition = TransitionTo(targetStateToken, targetData);
                if (transition->TargetStateToken == NULL)
                    break;

                targetStateToken = transition->TargetStateToken;
                targetData = transition->TargetStateData;
            }
        }

        TransitionInfo* StateManager::TransitionTo(StateToken* stateToken, StateData* data)
        {
            if (stateToken == NULL)
                throw new invalid_argument("Invalid 'stateToken' argument.");

            StateBase* state = FindState(stateToken);
            if (state == NULL)
                throw new UnknownStateException(GetCurrentStateToken(), stateToken);

            if (_currentState != NULL)
                _currentState->OnExit();

            StateBase* oldState = _currentState;
            _currentState = state;

            StateEnterEventArgs stateEnterEventArgs = StateEnterEventArgs(oldState != NULL ? oldState->GetToken() : NULL, data);
            _currentState->OnEnter(&stateEnterEventArgs);

            StateChangedEventArgs stateChangedEventArgs = StateChangedEventArgs(oldState, _currentState);
            OnStateChanged(&stateChangedEventArgs);

            return stateEnterEventArgs.GetRedirect();
        }

        StateBase* StateManager::FindState(StateToken* token)
        {
            for (list<StateBase*>::const_iterator it = _states.begin(), end = _states.end(); it != end; it++)
            {
                StateBase* currentState = *it;

                StateToken* currentToken = currentState->GetToken();
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

            TransitionInfo transitionInfo;
            transitionInfo.TargetStateToken = NULL;
            transitionInfo.TargetStateData = data;

            _currentState->Handle(action, data, &transitionInfo);
            if (transitionInfo.TargetStateToken == NULL)
            {
                _currentState = NULL;
                OnCompleted();
                return;
            }

            if (_currentState->GetToken() != transitionInfo.TargetStateToken)
                PerformTransitionTo(transitionInfo.TargetStateToken, transitionInfo.TargetStateData);
        }

        void StateManager::RegisterState(StateBase* state)
        {
            if (state == NULL)
                throw new invalid_argument("Invalid 'state' argument.");

            if (StateExists(state))
                throw new invalid_argument("State already registered.");

            _states.push_back(state);
            state->Initialize(this);
        }
    }
}
