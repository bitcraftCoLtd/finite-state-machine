#include <stdio.h>
#include "state_machine.h"

namespace Bitcraft
{
    namespace StateMachine
    {
        StateEnterEventArgs::StateEnterEventArgs(StateToken* from, StateData* data)
        {
            _from = from;
            _data = data;
        }

        StateToken* StateEnterEventArgs::GetFrom()
        {
            return _from;
        }

        StateData* StateEnterEventArgs::GetData()
        {
            return _data;
        }

        TransitionInfo* StateEnterEventArgs::GetRedirect()
        {
            return &_redirect;
        }
    }
}
