#ifndef __BITCRAFT_STATEMACHINE_STATE_MANAGER_H__
#define __BITCRAFT_STATEMACHINE_STATE_MANAGER_H__

#include <list>
#include "state_changed_event_args.h"
#include "state_base.h"
#include "state_token.h"
#include "state_data.h"
#include "transition_info.h"
#include "action_token.h"

using namespace std;

namespace Bitcraft
{
	namespace StateMachine
	{
		class StateBase;
		class StateChangedEventArgs;

		/// <summary>
		/// Represents a state manager.
		/// The state manager manages the states and the transtions.
		/// </summary>
		class StateManager
		{
		private:
			void* _context;
			StateBase* _currentState;
			list<StateBase*> _states;

		private:
			void InternalInitialization(void* context);
			void PerformTransitionTo(StateToken* stateToken, StateData* data);
			TransitionInfo* TransitionTo(StateToken* stateToken, StateData* data);
			StateBase* FindState(StateToken* token);
			bool StateExists(StateBase* state);

		protected:
			/// <summary>
			/// Called when the state machine transitions from a state to another.
			/// </summary>
			/// <param name="e">Custom event arguments.</param>
			virtual void OnStateChanged(StateChangedEventArgs* e) { }

			/// <summary>
			/// Called when the state machine has reached its final state.
			/// </summary>
			virtual void OnCompleted() { }

		public:
			/// <summary>
			/// Gets the context of the current state machine.
			/// </summary>
			void* GetContext();

			/// <summary>
			/// Gets the currently active state.
			/// </summary>
			StateBase* GetCurrentState();

			/// <summary>
			/// Gets the token of the currently active state. (shortcut to CurrentState.Token)
			/// </summary>
			StateToken* GetCurrentStateToken();

			/// <summary>
			/// Initializes the StateManager instance without context.
			/// </summary>
			StateManager();

			/// <summary>
			/// Initializes the StateManager instance with a context.
			/// </summary>
			/// <param name="context">The context to share among the states of the current state machine.</param>
			StateManager(void* context);

			/// <summary>
			/// Sets the initial state of the current state machine, and resets its internal state.
			/// </summary>
			/// <param name="initialState">The initial state.</param>
			/// <param name="data">The data to be provided to the initial state.</param>
			void SetInitialState(StateToken* initialState, StateData* data);

			/// <summary>
			/// Inform the state machine that an external action occured.
			/// This is the only way to make the state machine to possibly change its internal state.
			/// </summary>
			/// <param name="action">The action done that may change the state machine internal state.</param>
			void PerformAction(ActionToken* action);

			/// <summary>
			/// Inform the state machine that an external action occured.
			/// This is the only way to make the state machine to possibly change its internal state.
			/// </summary>
			/// <param name="action">The action done that may change the state machine internal state.</param>
			/// <param name="data">A custom data related to the action performed.</param>
			void PerformAction(ActionToken* action, StateData* data);

			/// <summary>
			/// Registers a state, given a new context for this state and its sub states.
			/// </summary>
			/// <param name="state">A state to be known by the state machine.</param>
			void RegisterState(StateBase* state);
		};
	}
}

#endif // __BITCRAFT_STATEMACHINE_STATE_MANAGER_H__
