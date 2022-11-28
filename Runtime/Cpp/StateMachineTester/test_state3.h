#ifndef __TEST_STATE3_H__
#define __TEST_STATE3_H__

#include <stdio.h>
#include "state_machine.h"
#include "test_action_tokens.h"

using namespace Bitcraft::StateMachine;

class TestState3 : public StateBase
{
public:

    TestState3()
        : StateBase(TestStateTokens::StateToken3)
    {
    }

    void OnInitialize() override
    {
        RegisterActionHandler(TestActionTokens::GoToState1Action, OnGotoState1);
        RegisterActionHandler(TestActionTokens::EndAction, OnEnd);
    }

    void OnEnter(StateEnterEventArgs* e) override
    {
        const StateToken* const from = e->GetFrom();

        printf("State '%S': OnEnter(from '%S')\n", GetToken()->ToString(), from != NULL ? from->ToString() : L"(null)");
    }

    static void OnGotoState1(StateBase* self, StateData* data, TransitionInfo* result)
    {
        result->TargetStateToken = TestStateTokens::StateToken1;
    }

    static void OnEnd(StateBase* self, StateData* data, TransitionInfo* result)
    {
        // result->TargetStateToken = NULL;
    }

    void OnExit() override
    {
        printf("State '%S': OnExit()\n", GetToken()->ToString());
    }
};

#endif // __TEST_STATE_H__
