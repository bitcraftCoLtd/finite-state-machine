#pragma once

#include "ax-fsm/action_token.h"
#include "ax-fsm/state_token.h"
#include "ax-fsm/state_data.h"
#include "ax-fsm/transition_info.h"

namespace AX { namespace StateMachine
{
    /// <summary>
    /// Represents event arguments when entering a new state.
    /// </summary>
    class StateEnterEventArgs
    {
    private:
        const ActionToken* const _triggeringAction;
        const StateToken* const _from;
        StateData* _data;
        TransitionInfo _redirect;

    public:
        /// <summary>
        /// Initializes the StateEnterEventArgs instance.
        /// </summary>
        /// <param name="triggeringAction">The action token of the action that triggered the transition.</param>
        /// <param name="from">The source state of the transition.</param>
        /// <param name="data">The data provided from the source state, for the target state.</param>
        StateEnterEventArgs(const ActionToken* const triggeringAction, const StateToken* const from, StateData* data);

        /// <summary>
        /// Gets the action token of the action that triggered the transition.
        /// </summary>
        const ActionToken* const GetTriggeringAction() const;

        /// <summary>
        /// Gets the source state token.
        /// </summary>
        const StateToken* const GetFrom() const;

        /// <summary>
        /// Gets the data provided from source, for the target.
        /// </summary>
        StateData* GetData();

        /// <summary>
        /// Gets a TransitionInfo object used for potential redirection directly from an Enter state event.
        /// </summary>
        TransitionInfo* GetRedirect();
    };
} }
