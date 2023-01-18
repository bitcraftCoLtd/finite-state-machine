#include <stdio.h>
#include "state_machine.h"

namespace AX
{
    namespace StateMachine
    {
        StateEnterEventArgs::StateEnterEventArgs(const ActionToken* const triggeringAction, const StateToken* const from, StateData* data)
            : _triggeringAction(triggeringAction), _from(from)
        {
            _data = data;
        }

        const ActionToken* const StateEnterEventArgs::GetTriggeringAction() const
        {
            return _triggeringAction;
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
