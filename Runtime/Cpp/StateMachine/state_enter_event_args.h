#ifndef __BITCRAFT_STATEMACHINE_STATE_ENTER_EVENT_ARGS_H__
#define __BITCRAFT_STATEMACHINE_STATE_ENTER_EVENT_ARGS_H__

#include "state_token.h"
#include "state_data.h"
#include "transition_info.h"

namespace Bitcraft
{
    namespace StateMachine
    {
        /// <summary>
        /// Represents event arguments when entering a new state.
        /// </summary>
        class StateEnterEventArgs
        {
        private:
            const StateToken* const  _from;
            StateData* _data;
            TransitionInfo _redirect;

        public:
            /// <summary>
            /// Initializes the StateEnterEventArgs instance.
            /// </summary>
            /// <param name="from">The source state of the transition.</param>
            /// <param name="data">The data provided from the source state, for the target state.</param>
            StateEnterEventArgs(const StateToken* const from, StateData* data);

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
    }
}

#endif // __BITCRAFT_STATEMACHINE_STATE_ENTER_EVENT_ARGS_H__
