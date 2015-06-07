#include "state_changed_event_args.h"

namespace Bitcraft
{
    namespace StateMachine
    {
        StateChangedEventArgs::StateChangedEventArgs(StateBase* oldState, StateBase* newState)
        {
            _oldState = oldState;
            _newState = newState;
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
