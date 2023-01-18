using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bitcraft.StateMachine
{
    public readonly struct HandlerResult
    {
        public readonly StateToken State;
        public readonly object Data;

        public HandlerResult(StateToken state)
            : this(state, null)
        {
        }

        public HandlerResult(StateToken state, object data)
        {
            State = state;
            Data = data;
        }
    }

    public delegate Task<HandlerResult> ActionHandler();
    public delegate Task<HandlerResult> DataActionHandler(object data);

    /// <summary>
    /// Represent a state of the state machine.
    /// A state is also a sub state machine in order to allow building a hierarchical state machine.
    /// </summary>
    public abstract class StateBase
    {
        private readonly Dictionary<ActionToken, ActionHandler> handlers = new Dictionary<ActionToken, ActionHandler>();
        private readonly Dictionary<ActionToken, DataActionHandler> dataHandlers = new Dictionary<ActionToken, DataActionHandler>();

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
                foreach (var dataHandler in dataHandlers)
                    yield return dataHandler.Key;
            }
        }

        /// <summary>
        /// Gets the context of the current state machine.
        /// </summary>
        public object Context
        {
            get
            {
                return StateManager?.Context;
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
                return default;

            if (Context is T typedContext)
                return typedContext;

            string message = $"Context is of type '{Context.GetType().FullName}', impossible to cast it to '{typeof(T).FullName}'";
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

        private Task<HandlerResult> NoopHandler()
        {
            return Task.FromResult(default(HandlerResult));
        }

        /// <summary>
        /// Registers a handler where the state transitions to another one for a given action.
        /// </summary>
        /// <param name="action">The action that makes the handler to be evaluated.</param>
        /// <param name="handler">The handler that evaluate and possibly performs the state transition.</param>
        protected void RegisterActionHandler(ActionToken action, ActionHandler handler)
        {
            if (handlers.ContainsKey(action) || dataHandlers.ContainsKey(action))
                throw new InvalidOperationException($"Action '{action}' already registered.");

            handlers.Add(action, handler);
        }

        /// <summary>
        /// Registers a handler where the state transitions to another one for a given action.
        /// </summary>
        /// <param name="action">The action that makes the handler to be evaluated.</param>
        /// <param name="handler">The handler that evaluate and possibly performs the state transition.</param>
        protected void RegisterActionHandler(ActionToken action, DataActionHandler handler)
        {
            if (handlers.ContainsKey(action) || dataHandlers.ContainsKey(action))
                throw new InvalidOperationException($"Action '{action}' already registered.");

            dataHandlers.Add(action, handler);
        }

        private bool isHandlingAsync;

        /// <summary>
        /// Tells whether a transition is being handled asynchronously and still underway or not.
        /// </summary>
        internal bool IsHandlingAsync => isHandlingAsync;

        internal readonly struct InternalHandlerResult
        {
            public readonly ActionResultType ResultType;
            public readonly StateToken State;
            public readonly object Data;

            public static readonly InternalHandlerResult ErrorUnknownAction = new InternalHandlerResult(ActionResultType.ErrorUnknownAction);
            public static readonly InternalHandlerResult ErrorAlreadyPerformingAction = new InternalHandlerResult(ActionResultType.ErrorAlreadyPerformingAction);
            public static readonly InternalHandlerResult ErrorForbiddenFromSpecialEvents = new InternalHandlerResult(ActionResultType.ErrorForbiddenFromSpecialEvents);

            private InternalHandlerResult(ActionResultType resultType)
            {
                ResultType = resultType;
                State = null;
                Data = null;
            }

            public InternalHandlerResult(StateToken state, object data)
            {
                ResultType = ActionResultType.Success;
                State = state;
                Data = data;
            }
        }

        /// <summary>
        /// Evaluates a handler that decides transition to the next state for a given action.
        /// </summary>
        /// <param name="action">The action that makes the handler to be evaluated.</param>
        /// <param name="data">A custom data related to the action performed.</param>
        /// <param name="callback">When called, performs the state transition.</param>
        internal async Task<InternalHandlerResult> Handle(ActionToken action, object data)
        {
            if (isHandlingAsync)
                return InternalHandlerResult.ErrorAlreadyPerformingAction;

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            bool hasHandler = handlers.TryGetValue(action, out ActionHandler handler);
            bool hasDataHandler = dataHandlers.TryGetValue(action, out DataActionHandler dataHandler);

            if (hasHandler == false && hasDataHandler == false)
            {
                // No action found.
                return InternalHandlerResult.ErrorUnknownAction;
            }
            else if (hasHandler && hasDataHandler)
            {
                // That means there is a bug in the code somewhere else.
                throw new IllegalActionException(action, Token, $"Both handle and data handler registered for the same action ({action}).");
            }

            if (handler == null && dataHandler == null)
            {
                // Action found but handler is null (should never happen because handler nullity is checked at the origin).
                throw new IllegalActionException(action, Token, $"No handler definded for the current action ({action}).");
            }

            // Here it is strictly impossible that both handler and dataHandler are non-null.

            isHandlingAsync = true;

            try
            {
                HandlerResult handlerResult;

                if (handler != null)
                    handlerResult = await handler();
                else
                    handlerResult = await dataHandler(data);

                return new InternalHandlerResult(handlerResult.State, handlerResult.Data);
            }
            finally
            {
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
