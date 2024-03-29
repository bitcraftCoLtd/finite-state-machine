#ifndef __BITCRAFT_STATEMACHINE_STATE_CHANGED_EVENT_ARGS_H__
#define __BITCRAFT_STATEMACHINE_STATE_CHANGED_EVENT_ARGS_H__

#include "state_base.h"

namespace Bitcraft
{
    namespace StateMachine
    {
        class StateBase;

        /// <summary>
        /// Represents a state transition event arguments.
        /// </summary>
        class StateChangedEventArgs
        {
        private:
            const ActionToken* const _triggeringAction;
            StateBase* _oldState;
            StateBase* _newState;

        public:
            /// <summary>
            /// Initializes the StateChangedEventArgs instance.
            /// </summary>
            /// <param name="triggeringAction">The action token of the action that triggered the transition.</param>
            /// <param name="oldState">Old state.</param>
            /// <param name="newState">New state.</param>
            StateChangedEventArgs(const ActionToken* const triggeringAction, StateBase* oldState, StateBase* newState);

            /// <summary>
            /// Gets the action token of the action that triggered the transition.
            /// </summary>
            const ActionToken* const GetTriggeringAction() const;

            /// <summary>
            /// Gets the previous state. (the state before transition)
            /// </summary>
            StateBase* GetOldState();

            /// <summary>
            /// Gets the new state. (the state after transition)
            /// </summary>
            StateBase* GetNewState();
        };
    }
}

#endif // __BITCRAFT_STATEMACHINE_STATE_CHANGED_EVENT_ARGS_H__
