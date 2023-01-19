#pragma once

#include "ax-fsm/state_token.h"

using namespace ax::fsm;

namespace TestStateTokens
{
    StateToken* StateToken1 = new StateToken(L"State 1");
    StateToken* StateToken2 = new StateToken(L"State 2");
    StateToken* StateToken3 = new StateToken(L"State 3");
}
