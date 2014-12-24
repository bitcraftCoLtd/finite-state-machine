using System;

namespace Bitcraft.StateMachine
{
    /// <summary>
    /// Represents event arguments when entering a new state.
    /// </summary>
    public class StateEnterEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the source state token.
        /// </summary>
        public StateToken From { get; private set; }

        /// <summary>
        /// Gets the data provided from source, for the target.
        /// </summary>
        public object Data { get; private set; }

        /// <summary>
        /// Gets a TransitionInfo object used for potential redirection directly from an Enter state event.
        /// </summary>
        public TransitionInfo Redirect { get; private set; }

        /// <summary>
        /// Initializes the StateEnterEventArgs instance.
        /// </summary>
        /// <param name="from">The source state of the transition.</param>
        /// <param name="data">The data provided from the source state, for the target state.</param>
        public StateEnterEventArgs(StateToken from, object data)
        {
            Redirect = new TransitionInfo();
            From = from;
            Data = data;
        }
    }

    /// <summary>
    /// Represents a state transition event arguments.
    /// </summary>
    public class StateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the previous state. (the state before transition)
        /// </summary>
        public StateBase OldState { get; private set; }

        /// <summary>
        /// Gets the new state. (the state after transition)
        /// </summary>
        public StateBase NewState { get; private set; }

        /// <summary>
        /// Initializes the StateChangedEventArgs instance.
        /// </summary>
        /// <param name="oldState">Old state.</param>
        /// <param name="newState">New state.</param>
        public StateChangedEventArgs(StateBase oldState, StateBase newState)
        {
            OldState = oldState;
            NewState = newState;
        }
    }
}
