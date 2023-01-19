#pragma once

#include "ax-fsm/action_token.h"
#include "ax-fsm/state_token.h"
#include "ax-fsm/state_data.h"

namespace AX { namespace StateMachine
{
    /// <summary>
    /// Represents event arguments when exiting a state.
    /// </summary>
    class StateExitEventArgs
    {
    private:
        const ActionToken* const _triggeringAction;
        const StateToken* const _to;
        StateData* _data;

    public:
        /// <summary>
        /// Initializes the StateExitEventArgs instance.
        /// </summary>
        /// <param name="triggeringAction">The action token of the action that triggered the transition.</param>
        /// <param name="to">The target state of the transition.</param>
        /// <param name="data">The data provided to the target state.</param>
        StateExitEventArgs(const ActionToken* const triggeringAction, const StateToken* const to, StateData* data);

        /// <summary>
        /// Gets the action token of the action that triggered the transition.
        /// </summary>
        const ActionToken* const GetTriggeringAction() const;

        /// <summary>
        /// Gets the target state token.
        /// </summary>
        const StateToken* const GetTo() const;

        /// <summary>
        /// Gets the data provided to the target.
        /// </summary>
        StateData* GetData();
    };
} }
