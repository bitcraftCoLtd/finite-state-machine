using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Bitcraft.StateMachine
{
    /// <summary>
    /// Represents a finite state machine.
    /// The state machine manages the states and the transtions.
    /// </summary>
    public class StateManager
    {
        /// <summary>
        /// Gets the context of the current state machine.
        /// </summary>
        public object Context { get; }

        /// <summary>
        /// Gets the currently active state.
        /// </summary>
        public StateBase CurrentState { get; private set; }

        /// <summary>
        /// Gets the token of the currently active state. (shortcut to CurrentState.Token)
        /// </summary>
        public StateToken CurrentStateToken => CurrentState?.Token;

        /// <summary>
        /// Gets the registered states.
        /// </summary>
        public IReadOnlyCollection<StateBase> States { get; }

        private readonly List<StateBase> states = new List<StateBase>();

        private bool isPerformActionLocked;

        /// <summary>
        /// Initializes the StateManager instance without context.
        /// </summary>
        public StateManager()
            : this(default)
        {
        }

        /// <summary>
        /// Initializes the StateManager instance with a context.
        /// </summary>
        /// <param name="context">The context to share among the states of the current state machine.</param>
        public StateManager(object context)
        {
            Context = context;
            States = new ReadOnlyCollection<StateBase>(states);
        }

        /// <summary>
        /// Sets the initial state of the current state machine, and resets its internal state.
        /// </summary>
        /// <param name="initialState">The initial state.</param>
        public void SetInitialState(StateToken initialState)
        {
            SetInitialState(initialState, null);
        }

        /// <summary>
        /// Sets the initial state of the current state machine, and resets its internal state.
        /// </summary>
        /// <param name="initialState">The initial state.</param>
        /// <param name="data">The data to be provided to the initial state.</param>
        public void SetInitialState(StateToken initialState, object data)
        {
            if (initialState == null)
                throw new ArgumentNullException(nameof(initialState));

            RaiseOnExitEvent(null, initialState, data);

            CurrentState = null;

            PerformTransitionTo(null, initialState, data);
        }

        private void PerformTransitionTo(ActionToken actionToken, StateToken stateToken, object data)
        {
            ActionToken triggeringActionToken = actionToken;
            StateToken targetStateToken = stateToken;
            object targetData = data;

            while (true)
            {
                TransitionInfo transition = TransitionTo(triggeringActionToken, targetStateToken, targetData);
                if (transition.TargetState == null)
                    break;

                triggeringActionToken = transition.TriggeringAction;
                targetStateToken = transition.TargetState;
                targetData = transition.TargetStateData;
            }
        }

        private TransitionInfo TransitionTo(ActionToken actionToken, StateToken stateToken, object data)
        {
            if (stateToken == null)
                throw new ArgumentNullException(nameof(stateToken));

            StateBase state = states.FirstOrDefault(s => s.Token == stateToken);
            if (state == null)
                throw new UnknownStateException(CurrentStateToken, stateToken);

            RaiseOnExitEvent(actionToken, stateToken, data);

            StateBase oldState = CurrentState;
            CurrentState = state;

            var stateEnterEventArgs = new StateEnterEventArgs(actionToken, oldState?.Token, data);

            isPerformActionLocked = true;

            try
            {
                OnStateChanged(new StateChangedEventArgs(actionToken, oldState, CurrentState));

                CurrentState.OnEnter(stateEnterEventArgs);
            }
            finally
            {
                isPerformActionLocked = false;
            }

            return stateEnterEventArgs.Redirect;
        }

        private void RaiseOnExitEvent(ActionToken triggeringAction, StateToken stateToken, object data)
        {
            if (CurrentState == null)
                return;

            var stateExitEventArgs = new StateExitEventArgs(triggeringAction, stateToken, data);

            isPerformActionLocked = true;

            try
            {
                CurrentState.OnExit(stateExitEventArgs);
            }
            finally
            {
                isPerformActionLocked = false;
            }
        }

        /// <summary>
        /// Gets whether it is possible to call PerformAction without being returned false.
        /// <remarks>Typically, CanPerformAction returns false when a transition is being evaluated asynchronously and still underway.</remarks>
        /// </summary>
        public bool CanPerformAction
        {
            get
            {
                if (CurrentState == null)
                    return false;

                if (isPerformActionLocked)
                    return false;

                return CurrentState.IsHandlingAsync == false;
            }
        }

        /// <summary>
        /// Tells the state machine that an external action occured.
        /// This is the only way to make the state machine to possibly change its internal state.
        /// </summary>
        /// <param name="action">The action done that may change the state machine internal state.</param>
        /// <returns>Returns false if it is already processing an asynchronous action, true otherwise.</returns>
        public async Task<ActionResultType> PerformAction(ActionToken action)
        {
            return await PerformAction(action, null);
        }

        /// <summary>
        /// Tells the state machine that an external action occured.
        /// This is the only way to make the state machine to possibly change its internal state.
        /// </summary>
        /// <param name="action">The action done that may change the state machine internal state.</param>
        /// <param name="data">A custom data related to the action performed.</param>
        /// <returns>Returns false if it is already processing an asynchronous action, true otherwise.</returns>
        public async Task<ActionResultType> PerformAction(ActionToken action, object data)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (CurrentState == null)
                throw new InvalidOperationException("State machine not yet initialized or has reached its final state.");

            if (isPerformActionLocked)
                return ActionResultType.ErrorForbiddenFromSpecialEvents;

            StateBase.InternalHandlerResult internalActionHandler = await CurrentState.Handle(action, data);

            if (internalActionHandler.ResultType != ActionResultType.Success)
                return internalActionHandler.ResultType;

            if (internalActionHandler.State == null)
            {
                RaiseOnExitEvent(action, CurrentState.Token, internalActionHandler.Data);
                CurrentState = null;
                OnCompleted(internalActionHandler.Data);
            }
            else if (CurrentState.Token != internalActionHandler.State)
                PerformTransitionTo(action, internalActionHandler.State, internalActionHandler.Data);

            return ActionResultType.Success;
        }

        /// <summary>
        /// Raised when the state machine transitions from a state to another.
        /// </summary>
        public event EventHandler<StateChangedEventArgs> StateChanged;

        /// <summary>
        /// Called when the state machine transitions from a state to another.
        /// </summary>
        /// <param name="e">Custom event arguments.</param>
        protected virtual void OnStateChanged(StateChangedEventArgs e)
        {
            StateChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Raised when the state machine has reached its final state.
        /// </summary>
        public event EventHandler Completed;

        /// <summary>
        /// Called when the state machine has reached its final state.
        /// </summary>
        protected virtual void OnCompleted(object data)
        {
            Completed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Registers a state, given a new context for this state and its sub states.
        /// </summary>
        /// <param name="state">A state to be known by the state machine.</param>
        public void RegisterState(StateBase state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            if (states.Contains(state))
                throw new InvalidOperationException($"State '{state.Token}' already registered.");

            states.Add(state);
            state.Initialize(this);
        }
    }
}
