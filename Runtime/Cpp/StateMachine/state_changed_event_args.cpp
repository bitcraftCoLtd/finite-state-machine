// Copyright ax, Inc. All Rights Reserved.

#include "ax-fsm/state_changed_event_args.h"

namespace ax { namespace fsm
{
    StateChangedEventArgs::StateChangedEventArgs(const ActionToken* const triggeringAction, StateBase* oldState, StateBase* newState)
        : _triggeringAction(triggeringAction)
    {
        _oldState = oldState;
        _newState = newState;
    }

    const ActionToken* const StateChangedEventArgs::GetTriggeringAction() const
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
} }
