#ifndef __BITCRAFT_STATEMACHINE_TRANSITION_INFO_H__
#define __BITCRAFT_STATEMACHINE_TRANSITION_INFO_H__

#include "state_token.h"
#include "state_data.h"

namespace Bitcraft
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
                TargetStateData = NULL;
                TargetStateToken = NULL;
            }

            /// <summary>
            /// Gets the target state token. (state active before transition)
            /// </summary>
            const StateToken* TargetStateToken;

            /// <summary>
            /// Gets the data provided from the target state, for the source state.
            /// </summary>
            StateData* TargetStateData;
        };
    }
}

#endif // __BITCRAFT_STATEMACHINE_TRANSITION_INFO_H__
