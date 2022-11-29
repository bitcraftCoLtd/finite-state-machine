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
            result->TargetStateToken = TestStateTokens::StateToken2;
        });
    }

    void OnEnter(StateEnterEventArgs* e) override
    {
        const StateToken* const from = e->GetFrom();

        printf("State '%S': OnEnter(from '%S')\n", GetToken()->ToString(), from != NULL ? from->ToString() : L"(null)");

        if (_alreadyPassed)
            e->GetRedirect()->TargetStateToken = TestStateTokens::StateToken3;

        _alreadyPassed = true;
    }

    void OnExit() override
    {
        printf("State '%S': OnExit()\n", GetToken()->ToString());
    }

private:
    bool _alreadyPassed;
};

#endif // __TEST_STATE_H__
