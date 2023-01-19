#pragma once

#include "ax-fsm/action_token.h"

using namespace ax::fsm;

namespace TestActionTokens
{
    ActionToken* GoToState1Action = new ActionToken(L"Go To State 1");
    ActionToken* GoToState2Action = new ActionToken(L"Go To State 2");
    ActionToken* GoToState3Action = new ActionToken(L"Go To State 3");
    ActionToken* EndAction = new ActionToken(L"End of state machine");
}
