// Copyright ax, Inc. All Rights Reserved.

#include <stdio.h>

#include "ax-fsm/state_manager.h"
#include "ax-fsm/state_base.h"
#include "ax-fsm/action_token.h"
#include "ax-fsm/state_token.h"
#include "ax-fsm/state_data.h"
#include "ax-fsm/exceptions.h"
#include "ax-fsm/transition_info.h"
#include "ax-fsm/state_changed_event_args.h"
#include "ax-fsm/state_enter_event_args.h"
#include "ax-fsm/state_exit_event_args.h"

using namespace std;

namespace ax { namespace fsm
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
        return _currentState != nullptr ? _currentState->GetToken() : nullptr;
    }

    StateManager::StateManager()
    {
        InternalInitialization(nullptr);
    }

    StateManager::StateManager(void* context)
    {
        InternalInitialization(context);
    }

    void StateManager::InternalInitialization(void* context)
    {
        _context = context;
        _currentState = nullptr;
        _isPerformActionLocked = false;
    }

    void StateManager::SetInitialState(const StateToken* const initialState, StateData* data)
    {
        if (initialState == nullptr)
            throw new invalid_argument("Invalid 'initialState' argument.");

        if (_currentState != nullptr)
        {
            _isPerformActionLocked = true;
            StateExitEventArgs stateExitEventArgs = StateExitEventArgs(nullptr, initialState, data);
            _currentState->OnExit(&stateExitEventArgs);
            _isPerformActionLocked = false;
        }

        _currentState = nullptr;
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
            if (transition->TargetState == nullptr)
                break;

            triggeringActionToken = transition->TriggeringAction;
            targetStateToken = transition->TargetState;
            targetData = transition->TargetStateData;
        }
    }

    TransitionInfo* StateManager::TransitionTo(const ActionToken* actionToken, const StateToken* const stateToken, StateData* data)
    {
        if (stateToken == nullptr)
            throw new invalid_argument("Invalid 'stateToken' argument.");

        StateBase* state = FindState(stateToken);
        if (state == nullptr)
            throw new UnknownStateException(GetCurrentStateToken(), stateToken);

        if (_currentState != nullptr)
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

        StateEnterEventArgs stateEnterEventArgs = StateEnterEventArgs(actionToken, oldState != nullptr ? oldState->GetToken() : nullptr, data);
        _currentState->OnEnter(&stateEnterEventArgs);

        _isPerformActionLocked = false;

        return stateEnterEventArgs.GetRedirect();
    }

    StateBase* StateManager::FindState(const StateToken* const token)
    {
        if (token == nullptr)
            throw new invalid_argument("Invalid 'stateToken' argument.");

        for (list<StateBase*>::const_iterator it = _states.begin(), end = _states.end(); it != end; it++)
        {
            StateBase* currentState = *it;

            const StateToken* const currentToken = currentState->GetToken();
            if (currentToken == nullptr)
                continue;

            if (currentToken->Equals(token))
                return currentState;
        }

        return nullptr;
    }

    bool StateManager::StateExists(StateBase* state)
    {
        if (state == nullptr)
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
        PerformAction(action, nullptr);
    }

    void StateManager::PerformAction(ActionToken* action, StateData* data)
    {
        if (action == nullptr)
            throw new invalid_argument("Invalid 'action' argument.");

        if (_currentState == nullptr)
            throw new exception("State machine not yet initialized or has reached its final state.");

        if (_isPerformActionLocked)
            return; // not that good :/

        TransitionInfo transitionInfo;
        transitionInfo.TargetState = nullptr;
        transitionInfo.TargetStateData = data;

        _currentState->Handle(action, data, &transitionInfo);
        if (transitionInfo.TargetState == nullptr)
        {
            _currentState = nullptr;
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
        if (state == nullptr)
            throw new invalid_argument("Invalid 'state' argument.");

        if (StateExists(state))
            throw new invalid_argument("State already registered.");

        _states.push_back(state);

        _isPerformActionLocked = true;
        state->Initialize(this);
        _isPerformActionLocked = false;
    }
} }
