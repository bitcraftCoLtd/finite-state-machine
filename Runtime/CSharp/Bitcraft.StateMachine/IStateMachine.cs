using System;
using System.Collections.ObjectModel;

namespace Bitcraft.StateMachine
{
    /// <summary>
    /// The state machine manages the states and the transtions.
    /// </summary>
    public interface IStateMachine
    {
        /// <summary>
        /// Gets the context of the current state machine.
        /// </summary>
        object Context { get; }

        /// <summary>
        /// Gets the currently active state.
        /// </summary>
        StateBase CurrentState { get; }

        /// <summary>
        /// Gets the token of the currently active state. (shortcut to CurrentState.Token)
        /// </summary>
        StateToken CurrentStateToken { get; }

        /// <summary>
        /// Gets the registered states.
        /// </summary>
        ReadOnlyCollection<StateBase> States { get; }

        /// <summary>
        /// Sets the initial state of the current state machine, and resets its internal state.
        /// </summary>
        /// <param name="initialState">The initial state.</param>
        void SetInitialState(StateToken initialState);

        /// <summary>
        /// Sets the initial state of the current state machine, and resets its internal state.
        /// </summary>
        /// <param name="initialState">The initial state.</param>
        /// <param name="data">The data to be provided to the initial state.</param>
        void SetInitialState(StateToken initialState, object data);

        /// <summary>
        /// Gets whether it is possible to call PerformAction without being returned false.
        /// <remarks>Typically, CanPerformAction returns false when a transition is being evaluated asynchronously and still underway.</remarks>
        /// </summary>
        bool CanPerformAction { get; }

        /// <summary>
        /// Tells the state machine that an external action occured.
        /// This is the only way to make the state machine to possibly change its internal state.
        /// </summary>
        /// <param name="action">The action done that may change the state machine internal state.</param>
        /// <returns>Returns an ActionResultType explaining if performing action succeeded or not.</returns>
        ActionResultType PerformAction(ActionToken action);

        /// <summary>
        /// Tells the state machine that an external action occured.
        /// This is the only way to make the state machine to possibly change its internal state.
        /// </summary>
        /// <param name="action">The action done that may change the state machine internal state.</param>
        /// <param name="data">A custom data related to the action performed.</param>
        /// <returns>Returns an ActionResultType explaining if performing action succeeded or not.</returns>
        ActionResultType PerformAction(ActionToken action, object data);

        /// <summary>
        /// Raised when the state machine transitions from a state to another.
        /// </summary>
        event EventHandler<StateChangedEventArgs> StateChanged;

        /// <summary>
        /// Raised when the state machine has reached its final state.
        /// </summary>
        event EventHandler Completed;

        /// <summary>
        /// Registers a state, given a new context for this state and its sub states.
        /// </summary>
        /// <param name="state">A state to be known by the state machine.</param>
        void RegisterState(StateBase state);
    }
}
