// Copyright AX, Inc. All Rights Reserved.

#pragma once

#include <functional>
#include <map>

#include "ax-fsm/state_token.h"
#include "ax-fsm/state_data.h"
#include "ax-fsm/state_enter_event_args.h"
#include "ax-fsm/state_exit_event_args.h"
#include "ax-fsm/action_token.h"
#include "ax-fsm/transition_info.h"
#include "ax-fsm/state_manager.h"

namespace AX { namespace StateMachine
{
    // action handler
    using StateHandler = std::function<void(StateData*, TransitionInfo*)>;

    /// <summary>
    /// Represent a state of the state machine.
    /// A state is also a sub state machine in order to allow building a hierarchical state machine.
    /// </summary>
    class StateBase
    {
        friend class StateManager;

    private:

        struct ActionTokenComparer {
            bool operator()(ActionToken* a, ActionToken* b) const {
                if (a == NULL || b == NULL)
                    return false;
                return a->_id < b->_id;
            }
        };

        std::map<ActionToken*, StateHandler, ActionTokenComparer> _handlers;

        StateManager* _stateManager;
        StateToken* _token;

        /// <summary>
        /// Initializes the current state.
        /// </summary>
        /// <param name="parent">The state machine in which the current state is registered.</param>
        /// <param name="context">The state machine context shared among the current state machine.</param>
        void Initialize(StateManager* parent);

        /// <summary>
        /// Evaluates a handler that decides transition to the next state for a given action.
        /// </summary>
        /// <param name="action">The action that makes the handler to be evaluated.</param>
        /// <param name="data">A custom data related to the action performed.</param>
        /// <param name="result">The TransitionInfo object to setup for the state machine to perform proper state transition.</param>
        void Handle(ActionToken* action, StateData* data, TransitionInfo* result);

        /// <summary>
        /// Called when the state machine enters the current state.
        /// </summary>
        /// <param name="e">Custem event arguments.</param>
        virtual void OnEnter(StateEnterEventArgs* e);

        /// <summary>
        /// Called when the state machine exits the current state.
        /// </summary>
        /// <param name="e">Custem event arguments.</param>
        virtual void OnExit(StateExitEventArgs* e);

    protected:
        /// <summary>
        /// Called when the state machine is initialized.
        /// </summary>
        virtual void OnInitialized();

        /// <summary>
        /// Registers a handler where the state transitions to the next one for a given action.
        /// </summary>
        /// <param name="action">The action that makes the handler to be evaluated.</param>
        /// <param name="handler">The handler that evaluate and possibly performs the state transition.</param>
        void RegisterActionHandler(ActionToken* action, StateHandler handler);

    public:
        /// <summary>
        /// Initializes the StateBase instance.
        /// </summary>
        /// <param name="token">The token that identifies the current state.</param>
        StateBase(StateToken* token);

        /// <summary>
        /// Gets the token that identifies the current state.
        /// </summary>
        const StateToken* const StateBase::GetToken() const;

        /// <summary>
        /// Gets the state manager in which the current state is registered.
        /// </summary>
        StateManager* GetStateManager();

        /// <summary>
        /// Gets the context of the current state machine.
        /// </summary>
        void* GetContext();
    };
} }
