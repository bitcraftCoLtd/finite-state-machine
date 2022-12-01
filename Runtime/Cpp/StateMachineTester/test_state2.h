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

    void OnInitialized() override
    {
        RegisterActionHandler(TestActionTokens::GoToState1Action, [this](StateData* data, TransitionInfo* result) {
            this->OnGoBackToState1(data, result);
        });
        RegisterActionHandler(TestActionTokens::GoToState3Action, [this](StateData* data, TransitionInfo* result) {
            this->OnGoToState3(data, result);
        });
    }

    void OnEnter(StateEnterEventArgs* e) override
    {
        const StateToken* const from = e->GetFrom();

        printf("State '%S': OnEnter(from '%S')\n", GetToken()->ToString(), from != NULL ? from->ToString() : L"(null)");
    }

    void OnGoBackToState1(StateData* data, TransitionInfo* result)
    {
        result->TargetState = TestStateTokens::StateToken1;
    }

    void OnGoToState3(StateData* data, TransitionInfo* result)
    {
        result->TargetState = TestStateTokens::StateToken3;
    }

    void OnExit(StateExitEventArgs* e) override
    {
        const StateToken* const to = e->GetTo();

        printf("State '%S': OnExit(to '%S')\n", GetToken()->ToString(), to != NULL ? to->ToString() : L"(null)");
    }
};

#endif // __TEST_STATE_H__
