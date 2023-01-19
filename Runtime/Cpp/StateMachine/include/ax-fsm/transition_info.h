#pragma once

#include "ax-fsm/action_token.h"
#include "ax-fsm/state_token.h"
#include "ax-fsm/state_data.h"

namespace AX
{
    namespace StateMachine
    {
        /// <summary>
        /// Represents information of a state machine transition.
        /// </summary>
        struct TransitionInfo
        {
            TransitionInfo()
            {
                TriggeringAction = NULL;
                TargetStateData = NULL;
                TargetState = NULL;
            }

            /// <summary>
            /// Gets the action token of the action that triggered the transition.
            /// </summary>
            ActionToken* TriggeringAction;

            /// <summary>
            /// Gets the target state token. (state active before transition)
            /// </summary>
            const StateToken* TargetState;

            /// <summary>
            /// Gets the data provided from the target state, for the source state.
            /// </summary>
            StateData* TargetStateData;
        };
    }
}
