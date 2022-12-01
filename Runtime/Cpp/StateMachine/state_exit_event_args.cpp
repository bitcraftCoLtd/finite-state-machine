#include <stdio.h>
#include "state_machine.h"

namespace Bitcraft
{
    namespace StateMachine
    {
        StateExitEventArgs::StateExitEventArgs(const ActionToken* const triggeringAction, const StateToken* const to, StateData* data)
            : _triggeringAction(triggeringAction), _to(to)
        {
            _data = data;
        }

        const ActionToken* const StateExitEventArgs::GetTriggeringAction() const
        {
            return _triggeringAction;
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
