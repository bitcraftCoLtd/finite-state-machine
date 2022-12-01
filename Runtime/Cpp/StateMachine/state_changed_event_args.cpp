#include "state_changed_event_args.h"

namespace Bitcraft
{
    namespace StateMachine
    {
        StateChangedEventArgs::StateChangedEventArgs(ActionToken* triggeringAction, StateBase* oldState, StateBase* newState)
        {
            _triggeringAction = triggeringAction;
            _oldState = oldState;
            _newState = newState;
        }

        ActionToken* StateEnterEventArgs::GetTriggeringAction()
        {
            return _triggeringAction;
        }

        StateBase* StateChangedEventArgs::GetOldState()
        {
            return _oldState;
        }

        StateBase* StateChangedEventArgs::GetNewState()
        {
            return _newState;
        }
    }
}
