// Copyright ax, Inc. All Rights Reserved.

#pragma once

#include <exception>
#include <stdexcept>

#include "ax-fsm/action_token.h"
#include "ax-fsm/state_token.h"

namespace ax { namespace fsm
{
    /// <summary>
    /// Represents an exception related to a state machine action.
    /// </summary>
    class ActionExceptionBase : public std::exception
    {
    private:
        const ActionToken* const _actionToken;
        const StateToken* const  _stateToken;

    public:
        /// <summary>
        /// Gets the token of the action that produced the error.
        /// </summary>
        const ActionToken* const GetActionToken() const;

        /// <summary>
        /// Gets the token of the state that was active when the error has been produced.
        /// </summary>
        const StateToken* const GetStateToken() const;

        /// <summary>
        /// Initializes the ActionExceptionBase instance.
        /// </summary>
        /// <param name="actionToken">The token of the action that produced the error.</param>
        /// <param name="stateToken">The token of the state that was active when the error has been produced.</param>
        ActionExceptionBase(const ActionToken* const actionToken, const StateToken* const stateToken);

        /// <summary>
        /// Initializes the ActionExceptionBase instance.
        /// </summary>
        /// <param name="actionToken">The token of the action that produced the error.</param>
        /// <param name="stateToken">The token of the state that was active when the error has been produced.</param>
        /// <param name="message">Custom message explaining the error.</param>
        ActionExceptionBase(const ActionToken* const actionToken, const StateToken* const stateToken, char* message);
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
        UnknownActionException(const ActionToken* const  actionToken, const StateToken* const stateToken);
    };

    /// <summary>
    /// Represents an exception related to an undeclared state machine state.
    /// </summary>
    class UnknownStateException : public std::exception
    {
    private:
        const StateToken* _sourceStateToken;
        const StateToken* _unknownStateToken;

    public:
        /// <summary>
        /// Gets the token of the source state.
        /// </summary>
        const StateToken* GetSourceStateToken();

        /// <summary>
        /// Gets the undeclared token that was targeting the new state.
        /// </summary>
        const StateToken* GetUnknownStateToken();

        /// <summary>
        /// Initializes the UnknownStateException instance.
        /// </summary>
        /// <param name="sourceStateToken">The token of the source state.</param>
        /// <param name="unknownStateToken">The undeclared token that was targeting the new state.</param>
        UnknownStateException(const StateToken* sourceStateToken, const StateToken* unknownStateToken);
    };
} }
