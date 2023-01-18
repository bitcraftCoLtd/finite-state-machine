#pragma once

#include <stdio.h>
#include "state_machine.h"
#include "test_state1.h"
#include "test_state2.h"
#include "test_state3.h"

using namespace AX::StateMachine;

class TestStateMachine : public StateManager
{
public:

    TestStateMachine()
    {
        RegisterState(new TestState1());
        RegisterState(new TestState2());
        RegisterState(new TestState3());
    }

    void OnStateChanged(StateChangedEventArgs* e) override
    {
        StateBase* oldState = e->GetOldState();
        StateBase* newState = e->GetNewState();

        printf("TestStateMachine: OnStateChanged (from '%S' to '%S')\n",
            oldState != NULL ? oldState->GetToken()->ToString() : L"(null)",
            newState != NULL ? newState->GetToken()->ToString() : L"(null)");
    }

    void OnCompleted() override
    {
        printf("TestStateMachine: OnCompleted\n");
    }
};
