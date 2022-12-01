#include <stdio.h>
#include "state_machine.h"

namespace Bitcraft
{
    namespace StateMachine
    {
        StateExitEventArgs::StateExitEventArgs(const StateToken* const to, StateData* data)
            : _to(to)
        {
            _data = data;
        }

        const StateToken* const StateExitEventArgs::GetTo() const
        {
            return _to;
        }

        StateData* StateExitEventArgs::GetData()
        {
            return _data;
        }
    }
}
