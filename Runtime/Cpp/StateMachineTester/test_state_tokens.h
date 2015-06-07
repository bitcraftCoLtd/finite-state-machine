#ifndef __TEST_STATE_TOKENS_H__
#define __TEST_STATE_TOKENS_H__

#include "state_machine.h"

using namespace Bitcraft::StateMachine;

namespace TestStateTokens
{
    StateToken* StateToken1 = new StateToken(L"State 1");
    StateToken* StateToken2 = new StateToken(L"State 2");
    StateToken* StateToken3 = new StateToken(L"State 3");
}

#endif // __TEST_STATE_TOKENS_H__
