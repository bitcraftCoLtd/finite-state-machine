// Copyright AX, Inc. All Rights Reserved.

#pragma once

#include <list>

#include "ax-fsm/state_changed_event_args.h"
#include "ax-fsm/state_enter_event_args.h"
#include "ax-fsm/state_exit_event_args.h"
#include "ax-fsm/state_base.h"
#include "ax-fsm/state_token.h"
#include "ax-fsm/state_data.h"
#include "ax-fsm/transition_info.h"
#include "ax-fsm/action_token.h"

namespace AX { namespace StateMachine
{
    class StateBase;
    class StateChangedEventArgs;

    /// <summary>
    /// Represents a state manager.
    /// The state manager manages the states and the transtions.
    /// </summary>
    class StateManager
    {
    private:
        void* _context;
        StateBase* _currentState;
        std::list<StateBase*> _states;
        bool _isPerformActionLocked;

    private:
        void InternalInitialization(void* context);
        void PerformTransitionTo(const ActionToken* actionToken, const StateToken* stateToken, StateData* data);
        TransitionInfo* TransitionTo(const ActionToken* actionToken, const StateToken* const stateToken, StateData* data);
        StateBase* FindState(const StateToken* const token);
        bool StateExists(StateBase* state);

    protected:
        /// <summary>
        /// Called when the state machine transitions from a state to another.
        /// </summary>
        /// <param name="e">Custom event arguments.</param>
        virtual void OnStateChanged(StateChangedEventArgs* e) { }

        /// <summary>
        /// Called when the state machine has reached its final state.
        /// </summary>
        virtual void OnCompleted() { }

    public:
        /// <summary>
        /// Gets the context of the current state machine.
        /// </summary>
        void* GetContext();

        /// <summary>
        /// Gets the currently active state.
        /// </summary>
        StateBase* GetCurrentState();

        /// <summary>
        /// Gets the token of the currently active state. (shortcut to CurrentState.Token)
        /// </summary>
        const StateToken* const GetCurrentStateToken() const;

        /// <summary>
        /// Initializes the StateManager instance without context.
        /// </summary>
        StateManager();

        /// <summary>
        /// Initializes the StateManager instance with a context.
        /// </summary>
        /// <param name="context">The context to share among the states of the current state machine.</param>
        StateManager(void* context);

        /// <summary>
        /// Sets the initial state of the current state machine, and resets its internal state.
        /// </summary>
        /// <param name="initialState">The initial state.</param>
        /// <param name="data">The data to be provided to the initial state.</param>
        void SetInitialState(const StateToken* const initialState, StateData* data);

        /// <summary>
        /// Inform the state machine that an external action occured.
        /// This is the only way to make the state machine to possibly change its internal state.
        /// </summary>
        /// <param name="action">The action done that may change the state machine internal state.</param>
        void PerformAction(ActionToken* action);

        /// <summary>
        /// Inform the state machine that an external action occured.
        /// This is the only way to make the state machine to possibly change its internal state.
        /// </summary>
        /// <param name="action">The action done that may change the state machine internal state.</param>
        /// <param name="data">A custom data related to the action performed.</param>
        void PerformAction(ActionToken* action, StateData* data);

        /// <summary>
        /// Registers a state, given a new context for this state and its sub states.
        /// </summary>
        /// <param name="state">A state to be known by the state machine.</param>
        void RegisterState(StateBase* state);
    };
} }
