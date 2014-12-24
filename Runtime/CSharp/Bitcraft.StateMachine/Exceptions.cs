using System;

namespace Bitcraft.StateMachine
{
    /// <summary>
    /// Represents an exception related to a state machine action.
    /// </summary>
    public abstract class ActionExceptionBase : Exception
    {
        /// <summary>
        /// Gets the token of the action that produced the error.
        /// </summary>
        public ActionToken ActionToken { get; private set; }

        /// <summary>
        /// Gets the token of the state that was active when the error has been produced.
        /// </summary>
        public StateToken StateToken { get; private set; }

        /// <summary>
        /// Initializes the ActionExceptionBase instance.
        /// </summary>
        /// <param name="actionToken">The token of the action that produced the error.</param>
        /// <param name="stateToken">The token of the state that was active when the error has been produced.</param>
        protected ActionExceptionBase(ActionToken actionToken, StateToken stateToken)
            : this(actionToken, stateToken, null)
        {
        }

        /// <summary>
        /// Initializes the ActionExceptionBase instance.
        /// </summary>
        /// <param name="actionToken">The token of the action that produced the error.</param>
        /// <param name="stateToken">The token of the state that was active when the error has been produced.</param>
        /// <param name="message">Custom message explaining the error.</param>
        protected ActionExceptionBase(ActionToken actionToken, StateToken stateToken, string message)
            : base((message ?? string.Empty) + string.Format(" (action: {0}, state: {1})", actionToken, stateToken))
        {
            ActionToken = actionToken;
            StateToken = stateToken;
        }
    }

    /// <summary>
    /// Represents an exception related to an invalid state machine action.
    /// </summary>
    public class IllegalActionException : ActionExceptionBase
    {
        /// <summary>
        /// Initializes the IllegalActionException instance.
        /// </summary>
        /// <param name="actionToken">The token of the action that produced the error.</param>
        /// <param name="stateToken">The token of the state that was active when the error has been produced.</param>
        public IllegalActionException(ActionToken actionToken, StateToken stateToken)
            : base(actionToken, stateToken)
        {
        }

        /// <summary>
        /// Initializes the IllegalActionException instance.
        /// </summary>
        /// <param name="actionToken">The token of the action that produced the error.</param>
        /// <param name="stateToken">The token of the state that was active when the error has been produced.</param>
        /// <param name="message">Custom message explaining the error.</param>
        public IllegalActionException(ActionToken actionToken, StateToken stateToken, string message)
            : base(actionToken, stateToken, message)
        {
        }
    }

    /// <summary>
    /// Represents an exception related to an undecalred state machine action.
    /// </summary>
    public class UnknownActionException : ActionExceptionBase
    {
        /// <summary>
        /// Initializes the UnknownActionException instance.
        /// </summary>
        /// <param name="actionToken">The token of the action that produced the error.</param>
        /// <param name="stateToken">The token of the state that was active when the error has been produced.</param>
        public UnknownActionException(ActionToken actionToken, StateToken stateToken)
            : base(actionToken, stateToken)
        {
        }
    }

    /// <summary>
    /// Represents an exception related to an undeclared state machine state.
    /// </summary>
    public class UnknownStateException : Exception
    {
        /// <summary>
        /// Gets the token of the source state.
        /// </summary>
        public StateToken SourceStateToken { get; private set; }

        /// <summary>
        /// Gets the undeclared token that was targeting the new state.
        /// </summary>
        public StateToken UnknownStateToken { get; private set; }

        /// <summary>
        /// Initializes the UnknownStateException instance.
        /// </summary>
        /// <param name="sourceStateToken">The token of the source state.</param>
        /// <param name="unknownStateToken">The undeclared token that was targeting the new state.</param>
        public UnknownStateException(StateToken sourceStateToken, StateToken unknownStateToken)
            : base(string.Format("(source state: {0}, unknown state: {1})", sourceStateToken, unknownStateToken))
        {
            SourceStateToken = sourceStateToken;
            UnknownStateToken = unknownStateToken;
        }
    }
}
