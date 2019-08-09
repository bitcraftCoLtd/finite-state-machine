using System;
using System.Collections.Generic;

namespace Bitcraft.StateMachine
{
    /// <summary>
    /// Represent a state of the state machine.
    /// A state is also a sub state machine in order to allow building a hierarchical state machine.
    /// </summary>
    public abstract class StateBase
    {
        private Dictionary<ActionToken, Action<object, Action<StateToken>>> handlers = new Dictionary<ActionToken, Action<object, Action<StateToken>>>();
        private Dictionary<ActionToken, Action<object, Action<StateToken, object>>> dataHandlers = new Dictionary<ActionToken, Action<object, Action<StateToken, object>>>();

        /// <summary>
        /// Gets the token that identifies the current state.
        /// </summary>
        public StateToken Token { get; }

        /// <summary>
        /// Gets the state manager in which the current state is registered.
        /// </summary>
        public StateManager StateManager { get; private set; }

        /// <summary>
        /// Gets the sequence of registered action tokens.
        /// </summary>
        public IEnumerable<ActionToken> Handlers
        {
            get
            {
                foreach (var handler in handlers)
                    yield return handler.Key;
                foreach (var handler in dataHandlers)
                    yield return handler.Key;
            }
        }

        /// <summary>
        /// Gets the context of the current state machine.
        /// </summary>
        public object Context
        {
            get
            {
                if (StateManager == null)
                    return null;

                return StateManager.Context;
            }
        }

        /// <summary>
        /// Raised when the state machine enters the current state.
        /// </summary>
        public event EventHandler Enter;

        /// <summary>
        /// Raised when the state machine exits the current state.
        /// </summary>
        public event EventHandler Exit;

        /// <summary>
        /// Initializes the StateBase instance.
        /// </summary>
        /// <param name="token">The token that identifies the current state.</param>
        public StateBase(StateToken token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            Token = token;
        }

        /// <summary>
        /// Initializes the current state.
        /// </summary>
        /// <remarks>This method MUST NEVER BE CALLED by any external component.</remarks>
        /// <param name="parent">The state machine in which the current state is registered.</param>
        internal void Initialize(StateManager parent)
        {
            StateManager = parent;
            OnInitialized();
        }

        /// <summary>
        /// Raised when the state machine is initialized.
        /// </summary>
        public event EventHandler Initialized;

        /// <summary>
        /// Called when the state machine is initialized.
        /// </summary>
        protected virtual void OnInitialized()
        {
            Initialized?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets the typed state machine context.
        /// </summary>
        /// <typeparam name="T">The type of the state machine context.</typeparam>
        /// <returns>Returns the state machine context.</returns>
        protected T GetContext<T>()
        {
            if (Context == null)
                return default(T);

            if (Context is T)
                return (T)Context;

            string message = string.Format(
                "Context is of type '{0}', impossible to cast it to '{1}'",
                Context.GetType().FullName,
                typeof(T).FullName);

            throw new InvalidOperationException(message);
        }

        /// <summary>
        /// Called when the state machine enters the current state.
        /// </summary>
        /// <param name="e">Custem event arguments.</param>
        protected internal virtual void OnEnter(StateEnterEventArgs e)
        {
            Enter?.Invoke(this, e);
        }

        /// <summary>
        /// Called when the state machine exits the current state.
        /// </summary>
        /// <param name="e">Custem event arguments.</param>
        protected internal virtual void OnExit(StateExitEventArgs e)
        {
            Exit?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Registers a handler where the state does not transition on the given action.
        /// </summary>
        /// <param name="action">The action that do not ignite transition.</param>
        protected void RegisterNoopActionHandler(ActionToken action)
        {
            RegisterActionHandler(action, NoopHandler);
        }

        private void NoopHandler(object data, Action<StateToken, object> callback)
        {
            callback(this.Token, data);
        }

        /// <summary>
        /// Registers a handler where the state transitions to another one for a given action.
        /// </summary>
        /// <param name="action">The action that makes the handler to be evaluated.</param>
        /// <param name="handler">The handler that evaluate and possibly performs the state transition.</param>
        protected void RegisterActionHandler(ActionToken action, Action<object, Action<StateToken>> handler)
        {
            if (dataHandlers.ContainsKey(action))
                throw new InvalidOperationException(string.Format("Action '{0}' already registered.", action));

            handlers.Add(action, handler);
        }

        /// <summary>
        /// Registers a handler where the state transitions to another one for a given action.
        /// </summary>
        /// <param name="action">The action that makes the handler to be evaluated.</param>
        /// <param name="handler">The handler that evaluate and possibly performs the state transition.</param>
        protected void RegisterActionHandler(ActionToken action, Action<object, Action<StateToken, object>> handler)
        {
            if (dataHandlers.ContainsKey(action))
                throw new InvalidOperationException(string.Format("Action '{0}' already registered.", action));

            dataHandlers.Add(action, handler);
        }

        private bool isHandlingAsync;

        /// <summary>
        /// Tells whether a transition is being handled asynchronously and still underway or not.
        /// </summary>
        internal bool IsHandlingAsync => isHandlingAsync;

        /// <summary>
        /// Evaluates a handler that decides transition to the next state for a given action.
        /// </summary>
        /// <param name="action">The action that makes the handler to be evaluated.</param>
        /// <param name="data">A custom data related to the action performed.</param>
        /// <param name="callback">When called, performs the state transition.</param>
        internal ActionResultType Handle(ActionToken action, object data, Action<StateToken, object> callback)
        {
            if (isHandlingAsync)
                return ActionResultType.ErrorAlreadyPerformingAction;

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            Action<object, Action<StateToken>> handler1 = null;
            Action<object, Action<StateToken, object>> handler2 = null;

            if (handlers.TryGetValue(action, out handler1) || dataHandlers.TryGetValue(action, out handler2))
            {
                isHandlingAsync = true;

                try
                {
                    bool callFlag = false;

                    if (handler1 != null)
                    {
                        handler1(data, st =>
                            {
                                if (callFlag)
                                    return;
                                RunCallback(callback, st, data);
                                callFlag = true;
                            });
                        return ActionResultType.Success;
                    }
                    else if (handler2 != null)
                    {
                        handler2(data, (st, d) =>
                            {
                                if (callFlag)
                                    return;
                                RunCallback(callback, st, d);
                                callFlag = true;
                            });
                        return ActionResultType.Success;
                    }
                    else
                        isHandlingAsync = false;
                }
                catch
                {
                    isHandlingAsync = false;
                    throw;
                }

                // action found but handler is null (should never happen because handler nullity is checked at the origin)
                throw new IllegalActionException(action, Token, "No handler definded for the current action.");
            }

            // no action found
            return ActionResultType.ErrorUnknownAction;
        }

        private void RunCallback(Action<StateToken, object> callback, StateToken stateToken, object data)
        {
            try
            {
                callback(stateToken, data);
            }
            finally
            {
                // ensure isHandlingAsync flag is properly restored
                isHandlingAsync = false;
            }
        }

        /// <summary>
        /// Provides a string representation of the state.
        /// </summary>
        /// <returns>Returns the string representation of the state.</returns>
        public override string ToString()
        {
            return Token?.ToString() ?? "(null token)";
        }
    }
}
