#pragma once

#include <stdio.h>

#include "ax-fsm/state_base.h"
#include "ax-fsm/state_data.h"
#include "ax-fsm/state_token.h"
#include "ax-fsm/transition_info.h"
#include "ax-fsm/state_enter_event_args.h"
#include "ax-fsm/state_exit_event_args.h"

#include "test_action_tokens.h"

using namespace ax::fsm;

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

        printf("State '%S': OnEnter(from '%S')\n", GetToken()->ToString(), from != nullptr ? from->ToString() : L"(null)");

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

        printf("State '%S': OnExit(to '%S')\n", GetToken()->ToString(), to != nullptr ? to->ToString() : L"(null)");
    }

private:
    bool _alreadyPassed;
};
