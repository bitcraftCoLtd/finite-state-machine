using System;

namespace Bitcraft.StateMachine
{
    /// <summary>
    /// Represents event arguments when entering a new state.
    /// </summary>
    public class StateEnterEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the action token of the action that triggered the transition.
        /// </summary>
        public ActionToken TriggeringAction { get; }

        /// <summary>
        /// Gets the source state token.
        /// </summary>
        public StateToken From { get; }

        /// <summary>
        /// Gets the data provided from source, for the target.
        /// </summary>
        public object Data { get; }

        /// <summary>
        /// Gets a TransitionInfo object used for potential redirection directly from an Enter state event.
        /// </summary>
        public TransitionInfo Redirect { get; }

        /// <summary>
        /// Initializes the StateEnterEventArgs instance.
        /// </summary>
        /// <param name="triggeringAction">Action token of the action that triggered the transition.</param>
        /// <param name="from">The source state of the transition.</param>
        /// <param name="data">The data provided from the source state, for the target state.</param>
        public StateEnterEventArgs(ActionToken triggeringAction, StateToken from, object data)
        {
            TriggeringAction = triggeringAction;
            From = from;
            Data = data;
            Redirect = new TransitionInfo();
        }
    }

    /// <summary>
    /// Represents event arguments when exiting a state.
    /// </summary>
    public class StateExitEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the action token of the action that triggered the transition.
        /// </summary>
        public ActionToken TriggeringAction { get; }

        /// <summary>
        /// Gets the target state token.
        /// </summary>
        public StateToken To { get; }

        /// <summary>
        /// Gets the data provided to the target.
        /// </summary>
        public object Data { get; }

        /// <summary>
        /// Initializes the StateExitEventArgs instance.
        /// </summary>
        /// <param name="triggeringAction">Action token of the action that triggered the transition.</param>
        /// <param name="to">The target state of the transition.</param>
        /// <param name="data">The data provided to the target state.</param>
        public StateExitEventArgs(ActionToken triggeringAction, StateToken to, object data)
        {
            TriggeringAction = triggeringAction;
            To = to;
            Data = data;
        }
    }

    /// <summary>
    /// Represents a state transition event arguments.
    /// </summary>
    public class StateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the action token of the action that triggered the transition.
        /// </summary>
        public ActionToken TriggeringAction { get; }

        /// <summary>
        /// Gets the previous state. (the state before transition)
        /// </summary>
        public StateBase OldState { get; }

        /// <summary>
        /// Gets the new state. (the state after transition)
        /// </summary>
        public StateBase NewState { get; }

        /// <summary>
        /// Initializes the StateChangedEventArgs instance.
        /// </summary>
        /// <param name="triggeringAction">Action token of the action that triggered the transition.</param>
        /// <param name="oldState">Old state.</param>
        /// <param name="newState">New state.</param>
        public StateChangedEventArgs(ActionToken triggeringAction, StateBase oldState, StateBase newState)
        {
            TriggeringAction = triggeringAction;
            OldState = oldState;
            NewState = newState;
        }
    }
}
