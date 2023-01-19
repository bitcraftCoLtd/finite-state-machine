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

        printf("State '%S': OnEnter(from '%S')\n", GetToken()->ToString(), from != nullptr ? from->ToString() : L"(null)");
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

        printf("State '%S': OnExit(to '%S')\n", GetToken()->ToString(), to != nullptr ? to->ToString() : L"(null)");
    }
};
