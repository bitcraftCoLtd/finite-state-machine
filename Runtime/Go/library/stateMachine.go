package fsm

import "fmt"

type StateMachine struct {
}

func (*StateMachine) Test() {
	fmt.Println("Test from FSM library")
}
