// Copyright AX, Inc. All Rights Reserved.

#include "ax-fsm/exceptions.h"

namespace AX
{
    namespace StateMachine
    {
        // === ActionExceptionBase ========================================================================================

        const ActionToken* const ActionExceptionBase::GetActionToken() const
        {
            return _actionToken;
        }

        const StateToken* const ActionExceptionBase::GetStateToken() const
        {
            return _stateToken;
        }

        ActionExceptionBase::ActionExceptionBase(const ActionToken* const actionToken, const StateToken* const stateToken)
            : exception(NULL), _actionToken(actionToken), _stateToken(stateToken)
        {
        }

        ActionExceptionBase::ActionExceptionBase(const ActionToken* const actionToken, const StateToken* const stateToken, char* message)
            : exception(message), _actionToken(actionToken), _stateToken(stateToken)
        {
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

        UnknownActionException::UnknownActionException(const ActionToken* const actionToken, const StateToken* const stateToken)
            : ActionExceptionBase(actionToken, stateToken)
        {
        }

        // === UnknownStateException ========================================================================================

        const StateToken* UnknownStateException::GetSourceStateToken()
        {
            return _sourceStateToken;
        }

        const StateToken* UnknownStateException::GetUnknownStateToken()
        {
            return _unknownStateToken;
        }

        UnknownStateException::UnknownStateException(const StateToken* sourceStateToken, const StateToken* unknownStateToken)
        {
            _sourceStateToken = sourceStateToken;
            _unknownStateToken = unknownStateToken;
        }
    }
}
