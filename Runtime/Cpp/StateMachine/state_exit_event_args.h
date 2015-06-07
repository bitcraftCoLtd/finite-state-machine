#ifndef __BITCRAFT_STATEMACHINE_STATE_EXIT_EVENT_ARGS_H__
#define __BITCRAFT_STATEMACHINE_STATE_EXIT_EVENT_ARGS_H__

#include "state_token.h"
#include "state_data.h"

namespace Bitcraft
{
    namespace StateMachine
    {
        /// <summary>
        /// Represents event arguments when exiting a state.
        /// </summary>
        class StateExitEventArgs
        {
        private:
            StateToken* _to;
            StateData* _data;

        public:
            /// <summary>
            /// Initializes the StateExitEventArgs instance.
            /// </summary>
            /// <param name="to">The target state of the transition.</param>
            /// <param name="data">The data provided to the target state.</param>
            StateExitEventArgs(StateToken* to, StateData* data);

            /// <summary>
            /// Gets the target state token.
            /// </summary>
            StateToken* GetTo();

            /// <summary>
            /// Gets the data provided to the target.
            /// </summary>
            StateData* GetData();
        };
    }
}

#endif // __BITCRAFT_STATEMACHINE_STATE_EXIT_EVENT_ARGS_H__
