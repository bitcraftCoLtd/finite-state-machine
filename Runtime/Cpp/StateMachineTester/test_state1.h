#ifndef __TEST_STATE1_H__
#define __TEST_STATE1_H__

#include <stdio.h>
#include "state_machine.h"
#include "test_action_tokens.h"

using namespace Bitcraft::StateMachine;

class TestState1 : public StateBase
{
public:

    TestState1()
        : StateBase(TestStateTokens::StateToken1)
    {
        _alreadyPassed = false;
    }

    void OnInitialized() override
    {
        RegisterActionHandler(TestActionTokens::GoToState2Action, [this](StateData* data, TransitionInfo* result) {
            this->OnGoToState2(data, result);
        });
    }

    void OnEnter(StateEnterEventArgs* e) override
    {
        const StateToken* const from = e->GetFrom();

        printf("State '%S': OnEnter(from '%S')\n", GetToken()->ToString(), from != NULL ? from->ToString() : L"(null)");

        if (_alreadyPassed)
            e->GetRedirect()->TargetState = TestStateTokens::StateToken3;

        _alreadyPassed = true;
    }

    void OnGoToState2(StateData* data, TransitionInfo* result)
    {
        result->TargetState = TestStateTokens::StateToken2;
    }

    void OnExit(StateExitEventArgs* e) override
    {
        const StateToken* const to = e->GetTo();

        printf("State '%S': OnExit(to '%S')\n", GetToken()->ToString(), to != NULL ? to->ToString() : L"(null)");
    }

private:
    bool _alreadyPassed;
};

#endif // __TEST_STATE_H__
