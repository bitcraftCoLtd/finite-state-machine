// Copyright ax, Inc. All Rights Reserved.

#include "ax-fsm/state_enter_event_args.h"
#include "ax-fsm/action_token.h"
#include "ax-fsm/state_token.h"
#include "ax-fsm/state_data.h"
#include "ax-fsm/transition_info.h"

namespace ax { namespace fsm
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
} }
