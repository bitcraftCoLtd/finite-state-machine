// Copyright ax, Inc. All Rights Reserved.

#pragma once

#include "ax-fsm/action_token.h"
#include "ax-fsm/state_token.h"
#include "ax-fsm/state_data.h"

namespace ax { namespace fsm
{
    /// <summary>
    /// Represents information of a state machine transition.
    /// </summary>
    struct TransitionInfo
    {
        TransitionInfo()
        {
            TriggeringAction = nullptr;
            TargetStateData = nullptr;
            TargetState = nullptr;
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
} }
