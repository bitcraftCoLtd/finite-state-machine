#ifndef __TEST_STATE2_H__
#define __TEST_STATE2_H__

#include <stdio.h>
#include "state_machine.h"
#include "test_action_tokens.h"

using namespace Bitcraft::StateMachine;

class TestState2 : public StateBase
{
public:

    TestState2()
        : StateBase(TestStateTokens::StateToken2)
    {
    }

    void OnInitialize() override
    {
        RegisterActionHandler(TestActionTokens::GoToState1Action, OnGoBackToState1);
        RegisterActionHandler(TestActionTokens::GoToState3Action, OnGoToState3);
    }

    void OnEnter(StateEnterEventArgs* e) override
    {
        StateToken* from = e->GetFrom();

        printf("State '%S': OnEnter(from '%S')\n", GetToken()->ToString(), from != NULL ? from->ToString() : L"(null)");
    }

    static void OnGoBackToState1(StateBase* self, StateData* data, TransitionInfo* result)
    {
        result->TargetStateToken = TestStateTokens::StateToken1;
    }

    static void OnGoToState3(StateBase* self, StateData* data, TransitionInfo* result)
    {
        result->TargetStateToken = TestStateTokens::StateToken3;
    }

    void OnExit() override
    {
        printf("State '%S': OnExit()\n", GetToken()->ToString());
    }
};

#endif // __TEST_STATE_H__
