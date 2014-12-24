#include "exceptions.h"

namespace Bitcraft
{
	namespace StateMachine
	{
		// === ActionExceptionBase ========================================================================================

		ActionToken* ActionExceptionBase::GetActionToken()
		{
			return _actionToken;
		}

		StateToken* ActionExceptionBase::GetStateToken()
		{
			return _stateToken;
		}

		ActionExceptionBase::ActionExceptionBase(ActionToken* actionToken, StateToken* stateToken)
			: exception(NULL)
		{
			_actionToken = actionToken;
			_stateToken = stateToken;
		}

		ActionExceptionBase::ActionExceptionBase(ActionToken* actionToken, StateToken* stateToken, char* message)
			: exception(message)
		{
			_actionToken = actionToken;
			_stateToken = stateToken;
		}

		// === IllegalActionException ========================================================================================

		IllegalActionException::IllegalActionException(ActionToken* actionToken, StateToken* stateToken)
			: ActionExceptionBase(actionToken, stateToken)
		{
		}

		IllegalActionException::IllegalActionException(ActionToken* actionToken, StateToken* stateToken, char* message)
			: ActionExceptionBase(actionToken, stateToken, message)
		{
		}

		// === UnknownActionException ========================================================================================

		UnknownActionException::UnknownActionException(ActionToken* actionToken, StateToken* stateToken)
			: ActionExceptionBase(actionToken, stateToken)
		{
		}

		// === UnknownStateException ========================================================================================

		StateToken* UnknownStateException::GetSourceStateToken()
		{
			return _sourceStateToken;
		}

		StateToken* UnknownStateException::GetUnknownStateToken()
		{
			return _unknownStateToken;
		}

		UnknownStateException::UnknownStateException(StateToken* sourceStateToken, StateToken* unknownStateToken)
		{
			_sourceStateToken = sourceStateToken;
			_unknownStateToken = unknownStateToken;
		}
	}
}
