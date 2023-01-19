// Copyright AX, Inc. All Rights Reserved.

#include "ax-fsm/state_exit_event_args.h"
#include "ax-fsm/action_token.h"
#include "ax-fsm/state_token.h"
#include "ax-fsm/state_data.h"

namespace AX
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
