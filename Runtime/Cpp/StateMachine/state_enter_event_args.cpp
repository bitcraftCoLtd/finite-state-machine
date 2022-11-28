#include <stdio.h>
#include "state_machine.h"

namespace Bitcraft
{
    namespace StateMachine
    {
        StateEnterEventArgs::StateEnterEventArgs(const StateToken* const from, StateData* data):
            _from(from)
        {
            _data = data;
        }

        const StateToken* const StateEnterEventArgs::GetFrom() const
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
