#include <Windows.h>
#include <stdio.h>

#include "test_state_tokens.h"
#include "test_state_machine.h"
#include "test_state1.h"
#include "test_state2.h"
#include "test_state3.h"

using namespace ax::fsm;

int main(int argc, char* argv[])
{
    TestStateMachine stateMachine;
    stateMachine.SetInitialState(TestStateTokens::StateToken1, nullptr);

    stateMachine.PerformAction(TestActionTokens::GoToState2Action);
    stateMachine.PerformAction(TestActionTokens::GoToState3Action);
    stateMachine.PerformAction(TestActionTokens::GoToState1Action);

    stateMachine.PerformAction(TestActionTokens::EndAction);

    return 0;
}
