#ifndef __BITCRAFT_STATEMACHINE_EXCEPTIONS_H__
#define __BITCRAFT_STATEMACHINE_EXCEPTIONS_H__

#include <exception>
#include "action_token.h"
#include "state_token.h"

using namespace std;

namespace Bitcraft
{
	namespace StateMachine
	{
		/// <summary>
		/// Represents an exception related to a state machine action.
		/// </summary>
		class ActionExceptionBase : public exception
		{
		private:
			ActionToken* _actionToken;
			StateToken* _stateToken;

		public:
			/// <summary>
			/// Gets the token of the action that produced the error.
			/// </summary>
			ActionToken* GetActionToken();

			/// <summary>
			/// Gets the token of the state that was active when the error has been produced.
			/// </summary>
			StateToken* GetStateToken();

			/// <summary>
			/// Initializes the ActionExceptionBase instance.
			/// </summary>
			/// <param name="actionToken">The token of the action that produced the error.</param>
			/// <param name="stateToken">The token of the state that was active when the error has been produced.</param>
			ActionExceptionBase(ActionToken* actionToken, StateToken* stateToken);

			/// <summary>
			/// Initializes the ActionExceptionBase instance.
			/// </summary>
			/// <param name="actionToken">The token of the action that produced the error.</param>
			/// <param name="stateToken">The token of the state that was active when the error has been produced.</param>
			/// <param name="message">Custom message explaining the error.</param>
			ActionExceptionBase(ActionToken* actionToken, StateToken* stateToken, char* message);
		};

		/// <summary>
		/// Represents an exception related to an invalid state machine action.
		/// </summary>
		class IllegalActionException : public ActionExceptionBase
		{
		public:
			/// <summary>
			/// Initializes the IllegalActionException instance.
			/// </summary>
			/// <param name="actionToken">The token of the action that produced the error.</param>
			/// <param name="stateToken">The token of the state that was active when the error has been produced.</param>
			IllegalActionException(ActionToken* actionToken, StateToken* stateToken);

			/// <summary>
			/// Initializes the IllegalActionException instance.
			/// </summary>
			/// <param name="actionToken">The token of the action that produced the error.</param>
			/// <param name="stateToken">The token of the state that was active when the error has been produced.</param>
			/// <param name="message">Custom message explaining the error.</param>
			IllegalActionException(ActionToken* actionToken, StateToken* stateToken, char* message);
		};

		/// <summary>
		/// Represents an exception related to an undecalred state machine action.
		/// </summary>
		class UnknownActionException : public ActionExceptionBase
		{
		public:
			/// <summary>
			/// Initializes the UnknownActionException instance.
			/// </summary>
			/// <param name="actionToken">The token of the action that produced the error.</param>
			/// <param name="stateToken">The token of the state that was active when the error has been produced.</param>
			UnknownActionException(ActionToken* actionToken, StateToken* stateToken);
		};

		/// <summary>
		/// Represents an exception related to an undeclared state machine state.
		/// </summary>
		class UnknownStateException : public exception
		{
		private:
			StateToken* _sourceStateToken;
			StateToken* _unknownStateToken;

		public:
			/// <summary>
			/// Gets the token of the source state.
			/// </summary>
			StateToken* GetSourceStateToken();

			/// <summary>
			/// Gets the undeclared token that was targeting the new state.
			/// </summary>
			StateToken* GetUnknownStateToken();

			/// <summary>
			/// Initializes the UnknownStateException instance.
			/// </summary>
			/// <param name="sourceStateToken">The token of the source state.</param>
			/// <param name="unknownStateToken">The undeclared token that was targeting the new state.</param>
			UnknownStateException(StateToken* sourceStateToken, StateToken* unknownStateToken);
		};
	}
}

#endif // __BITCRAFT_STATEMACHINE_EXCEPTIONS_H__
