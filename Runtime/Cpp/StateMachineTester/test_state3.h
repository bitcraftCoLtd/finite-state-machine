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

    void OnInitialized() override
    {
        RegisterActionHandler(TestActionTokens::GoToState1Action, [this](StateData* data, TransitionInfo* result) {
            this->OnGotoState1(data, result);
        });
        RegisterActionHandler(TestActionTokens::EndAction, [this](StateData* data, TransitionInfo* result) {
            this->OnEnd(data, result);
        });
    }

    void OnEnter(StateEnterEventArgs* e) override
    {
        const StateToken* const from = e->GetFrom();

        printf("State '%S': OnEnter(from '%S')\n", GetToken()->ToString(), from != NULL ? from->ToString() : L"(null)");
    }

    void OnGotoState1(StateData* data, TransitionInfo* result)
    {
        result->TargetState = TestStateTokens::StateToken1;
    }

    void OnEnd(StateData* data, TransitionInfo* result)
    {
        // result->TargetState = nullptr;
    }

    void OnExit(StateExitEventArgs* e) override
    {
        const StateToken* const to = e->GetTo();

        printf("State '%S': OnExit(to '%S')\n", GetToken()->ToString(), to != NULL ? to->ToString() : L"(null)");
    }
};

#endif // __TEST_STATE_H__
