/**
 * @name fsm
 * @namespace
 */
var fsm;
//noinspection JSUnusedAssignment
fsm = fsm || {};

/**
 * Identification token
 * @typedef {Object} Token
 * @property {number} id
 * @property {string} name
 */

/**
 * Transition information container
 * @typedef {Object} TransitionInfo
 * @property {Token} targetStateToken
 * @property {*} targetStateData
 */

/**
 * Represents a state of the state machine
 * @typedef {Object} State
 * @property {Token} token
 * @property {function: StateManager} getStateManager
 * @property {function} getContext
 * @property {function: State} getCurrentState
 * @property {function: Token} getCurrentStateToken
 * @property {function: boolean} isHandlingAsync
 * @property {function(Token, ActionHandler)} registerActionHandler
 * @property {function} onInitialize Called after the state has been initialized by its state machine
 * @property {function} onEnter Called after the current state has begun active
 * @property {function} onExit Called after the current state has begun inactive
 */

/**
 * A state changed event argument
 * @typedef {Object} StateChangedEventArgs
 * @property {State} oldState
 * @property {State} newState
 */

/**
 * A state enter event argument
 * @typedef {Object} StateEnterEventArgs
 * @property {Token} from
 * @property {*} data
 * @property {TransitionInfo} redirect
 */

/**
 * A state exit event argument
 * @typedef {Object} StateExitEventArgs
 * @property {Token} to
 * @property {*} data
 */

/**
 * An event subscription
 * @typedef {Object} EventSubscription
 * @property {Function} unregister
 */

/**
 * Enum indicating result of an action request on the StateManager
 * @readonly
 * @enum {number}
 */
fsm.ACTION_RESULT_TYPE = {
    /** Action has performed successfully */
    SUCCESS: 0,
    /** Error, the current state does not know about the given action */
    ERROR_UNKNOWN_ACTION: 1,
    /** Error, the state machine is already performing an action asynchronously */
    ERROR_ALREADY_PERFORMING_ACTION: 2
};

/**
 * Convert the enum value to a string representation
 * @param {number} value Value of an enum of type ActionResultType
 * @returns {string}
 */
fsm.ACTION_RESULT_TYPE.stringify = function (value) {
    'use strict';
    if (value === this.SUCCESS) {
        return 'SUCCESS';
    }
    if (value === this.ERROR_ALREADY_PERFORMING_ACTION) {
        return 'ERROR_ALREADY_PERFORMING_ACTION';
    }
    if (value === this.ERROR_UNKNOWN_ACTION) {
        return 'ERROR_UNKNOWN_ACTION';
    }
    return null;
};

/**
 * Represents a finite state machine
 * @constructor
 * @param {*} [context] A custom value shared among all states of this state machine
 */
fsm.StateManager = function (context) {
    'use strict';
    this._context = context || null;

    /**
     * The state currently active
     * @type {State}
     * @private
     */
    this._currentState = null;

    /**
     * A collection of registered states
     * @type {State[]}
     * @private
     */
    this._states = [];

    this._stateChangedCallbackContainers = [];
    this._completedCallbackContainers = [];
};

/**
 * Checks whether the given token is valid or not
 * @param {Token} token The identification token to check for validity
 */
fsm.StateManager.checkTokenValidity = function (token) {
    'use strict';

    if (!token) {
        throw new Error('Invalid \'token\' argument.');
    }

    if (typeof token.id !== 'number') {
        throw new Error('Invalid token, it must contain an \'id\' member of type \'number\'.');
    }

    if (typeof token.name !== 'string') {
        throw new Error('Invalid token, it must contain a \'name\' member of type \'string\'.');
    }
};

//noinspection JSUnusedGlobalSymbols
/**
 * Registers an event to be raised when the active state changes
 * @param {Object} [context] Execution context of the callback
 * @param {StateChangedHandler} cb
 * @returns {EventSubscription}
 */
fsm.StateManager.prototype.registerOnStateChanged = function (context, cb) {
    'use strict';

    // make sure we are given a function to call back
    if (typeof cb !== 'function') {
        throw new Error('Invalid cb argument.');
    }

    var self = this;
    var localSubscription = { context: context, cb: cb };

    this._stateChangedCallbackContainers.push(localSubscription);

    return {
        unregister: function () {
            var index = self._stateChangedCallbackContainers.indexOf(localSubscription);
            if (index >= 0) {
                self._stateChangedCallbackContainers.splice(index, 1);
            }
        }
    };
};

//noinspection JSUnusedGlobalSymbols
/**
 * Registers an event to be raised when the state machine reaches a final state
 * @param {Object} [context] Execution context of the callback
 * @param {Function} cb
 * @returns {EventSubscription}
 */
fsm.StateManager.prototype.registerOnCompleted = function (context, cb) {
    'use strict';

    // make sure we are given a function to call back
    if (typeof cb !== 'function') {
        throw new Error('Invalid cb argument.');
    }

    var self = this;
    var localSubscription = { context: context, cb: cb };

    this._completedCallbackContainers.push(localSubscription);

    return {
        unregister: function () {
            var index = self._completedCallbackContainers.indexOf(localSubscription);
            if (index >= 0) {
                self._completedCallbackContainers.splice(index, 1);
            }
        }
    };
};

/**
 * Transition from a state to another
 * @param {Token} stateToken The token that identifies the new state activate
 * @param {*} data
 * @returns {TransitionInfo}
 * @private
 */
fsm.StateManager.prototype._transitionTo = function (stateToken, data) {
    'use strict';

    // check state token validity
    if (!stateToken) {
        throw new Error('Invalid stateToken argument.');
    }

    // find a state identified bt the given state token
    //noinspection JSUnresolvedFunction
    var filteredStates = this._states.filter(function (s) { return s.token.id === stateToken.id; });
    if (!filteredStates || filteredStates.length === 0) {
        // no such state registered
        throw new Error('Unknown state [' + stateToken.name + ']');
    }

    // check whether the current state has defined a function named 'onExit'
    // and call it if available, providing the said state as execution context
    if (this._currentState && typeof this._currentState.onExit === 'function') {

        // create enter state event argument
        var exitEventArgs = {
            to: stateToken,
            data: data
        };

        this._currentState.onExit(exitEventArgs);
    }

    // keep reference to previous state
    /** @type {State} */
    var oldState = this._currentState;
    // set new active state
    this._currentState = filteredStates[0];

    // create enter state event argument
    var enterEventArgs = {
        from: oldState ? oldState.token : null,
        data: data,
        redirect: {
            targetStateToken: null,
            targetStateData: null
        }
    };

    // check whether the current state has defined a function named 'onEnter'
    // and call it if available, providing the said state as execution context
    if (typeof this._currentState.onEnter === 'function') {
        this._currentState.onEnter(enterEventArgs);
    }

    // check if the user didn't mess up the provided event argument
    if (!enterEventArgs.redirect) {
        throw new Error('Illegal operation.');
    }

    // create a state changed event argument
    var stateChangedEventArgs = {
        oldState: oldState,
        newState: this._currentState
    };

    var i;
    var obj;
    // notify all registered parties of the state change
    for (i = 0; i < this._stateChangedCallbackContainers.length; i += 1) {
        obj = this._stateChangedCallbackContainers[i];
        obj.cb.call(obj.context, this, stateChangedEventArgs);
    }

    // return the TransitionInfo object
    return enterEventArgs.redirect;
};

/**
 * Transition from a state to another, chaining the redirections
 * @param {Token} stateToken
 * @param {*} data
 * @private
 */
fsm.StateManager.prototype._performTransitionTo = function (stateToken, data) {
    'use strict';

    // checks for token validity
    fsm.StateManager.checkTokenValidity(stateToken);

    // prepare current token and data
    /** @type {Token} */
    var targetStateToken = stateToken;
    var targetData = data;

    var transition;
    while (true) {
        // transition to the requested state, and check for a possible redirection
        transition = this._transitionTo(targetStateToken, targetData);
        if (!transition || !transition.targetStateToken) {
            // no redirection any more
            break;
        }

        // chain the state token and data for the next iteration
        targetStateToken = transition.targetStateToken;
        targetData = transition.targetStateData;
    }
};

//noinspection JSUnusedGlobalSymbols
/**
 * Gets the common context shared among all states of the same state machine
 * @returns {*}
 */
fsm.StateManager.prototype.getContext = function () {
    'use strict';
    return this._context;
};

//noinspection JSUnusedGlobalSymbols
/**
 * Gets the state currently active
 * @returns {State} Returns the currently active state
 */
fsm.StateManager.prototype.getCurrentState = function () {
    'use strict';
    return this._currentState;
};

//noinspection JSUnusedGlobalSymbols
/**
 * Gets the token of the state currently active, or null if there is no active state
 * @returns {Token} Return the token of the currently active state, or null if unavailable
 */
fsm.StateManager.prototype.getCurrentStateToken = function () {
    'use strict';
    return this._currentState ? this._currentState.token : null;
};

//noinspection JSUnusedGlobalSymbols
/**
 * Sets the initial state of the state machine
 * @param {Token} initialState
 * @param {*} [data]
 */
fsm.StateManager.prototype.setInitialState = function (initialState, data) {
    'use strict';

    // checks whether initial state argument is valid or not
    if (!initialState) {
        throw new Error('Invalid \'initialState\' argument.');
    }

    // initialize the current state to null
    this._currentState = null;
    // run transition to the initial state
    this._performTransitionTo(initialState, data);
};

//noinspection JSUnusedGlobalSymbols
/**
 * Checks whether the state machine is capable of performing and action or if it is still performing a previous one
 * @returns {boolean} Returns true if ready to perform an action, false otherwise
 */
fsm.StateManager.prototype.canPerformAction = function () {
    'use strict';
    if (!this._currentState) {
        return false;
    }
    return this._currentState.isHandlingAsync() === false;
};

//noinspection JSUnusedGlobalSymbols
/**
 * Tells the state machine to perform an action on the currently active state
 * @param {Token} action The token that identifies the action to perform
 * @param {*} [data] A custom data to pass to the state that receives the action
 * @returns {number} Returns the status telling whether action has been performed or if an error occurred
 */
fsm.StateManager.prototype.performAction = function (action, data) {
    'use strict';

    // checks for action token validity
    fsm.StateManager.checkTokenValidity(action);

    // check for current state validity
    if (!this._currentState) {
        throw new Error('State machine not yet initialized or has reached its final state.');
    }

    // keep track of the current execution context
    var self = this;

    // propagate action performing to the current state, passing it the action to perform,
    // the custom data and the transition decision callback
    return this._currentState._handle(action, data, function (innerAction, innerData) {

        // enforce at least the action token to be provided
        if (arguments.length === 0) {
            throw new Error('Missing state argument.');
        }

        // checks whether innerAction argument is provided
        if (!innerAction) {
            // in this case, the state machine has reached a final state and is done

            // clear the current state
            self._currentState = null;

            var i;
            var obj;
            // notify all registered parties that the state machine has reached its completed state
            for (i = 0; i < self._completedCallbackContainers.length; i += 1) {
                obj = self._completedCallbackContainers[i];
                obj.cb.call(obj.context, self);
            }
        } else {
            // ensure the action token is valid
            fsm.StateManager.checkTokenValidity(innerAction);

            // check whether to targeting state is not the current one
            if (self._currentState.token.id !== innerAction.id) {

                if (arguments.length < 2) {
                    // data not explicitly provided, so the previous data flows
                    innerData = data;
                }

                // transition to the targeting state, chaining the provided custom data
                self._performTransitionTo(innerAction, innerData);
            }
        }
    });
};

/**
 * The private static method to be injected in a registerActionHandler() public member method in a state
 * @param {State} state The state in which to inject the method
 * @param {Token} action The token that identifies the action being performed
 * @param {ActionHandler} handler The action handler that decides to which state to transition
 * @private
 */
fsm.StateManager._injectableRegisterActionHandler = function (state, action, handler) {
    'use strict';

    // checks whether the handler argument is valid
    if (typeof handler !== 'function') {
        throw new Error('Invalid handler argument.');
    }

    // checks whether the action token argument is valid
    fsm.StateManager.checkTokenValidity(action);

    // make a dictionary key out of the token identifier
    var key = action.id.toString();
    // check whether the key is already registered
    if (state._handlers[key]) {
        throw new Error("Action already registered.");
    }

    // register the handler for the given action
    state._handlers[key] = handler;
};

/**
 * The private static method to be injected in a _handle() private member method in a state
 * @param {State} state The state in which to inject the method
 * @param {Token} action The token that identifies the action being performed
 * @param {*} data The custom data to be passed to the action handler
 * @param {StateTransitionDecisionHandler} decisionCallback The state transition decision callback
 * that decides to which state to move on
 * @returns {number} Returns the status telling whether action has been performed or if an error occurred
 * @private
 */
fsm.StateManager._injectableHandle = function (state, action, data, decisionCallback) {
    'use strict';

    // check if a previous performAction() method call is still pending
    if (state._isHandlingAsync) {
        return fsm.ACTION_RESULT_TYPE.ERROR_ALREADY_PERFORMING_ACTION;
    }

    // check for the action argument validity
    if (!action) {
        throw new Error('Invalid action argument.');
    }

    // check for the action token validity
    fsm.StateManager.checkTokenValidity(action);

    // make a dictionary key out of the token identifier
    var key = action.id.toString();
    // get the handler from its key
    var handler = state._handlers[key];

    // check for the handler validity
    if (!handler) {
        return fsm.ACTION_RESULT_TYPE.ERROR_UNKNOWN_ACTION;
    }

    // set flag telling a state change decision is being taken and pending
    //noinspection JSUndefinedPropertyAssignment
    state._isHandlingAsync = true;

    try {
        // create a call flag to protect against multiple calls
        var callFlag = false;

        // call the action handler in the execution context of the state
        // providing it the custom data and the transition decision callback
        handler.call(state, data, function (innerAction, innerData) {

            // check the protection flag
            if (callFlag) {
                return;
            }
            try {
                // call the transition decision callback transfering the arguments as is
                decisionCallback.apply(this, arguments);
            } finally {
                // ensure _isHandlingAsync flag is properly cleared
                //noinspection JSUndefinedPropertyAssignment
                state._isHandlingAsync = false;
            }
            // set the multiple call protection flag
            callFlag = true;
        });
        return fsm.ACTION_RESULT_TYPE.SUCCESS;
    } catch (ex) {
        // clears the async handling flag if something goes wrong
        //noinspection JSUndefinedPropertyAssignment
        state._isHandlingAsync = false;
        // rethrow, the exception must not be swallowed
        throw ex;
    }
};

/**
 * Globally unique identifier
 * @type {number}
 * @private
 */
fsm.StateManager._globalUniqueId = 0;

/**
 * Gets the next globally unique available identifier
 * @returns {number}
 */
fsm.StateManager.nextId = function () {
    'use strict';
    // increment the identifier in order to ensure uniqueness
    fsm.StateManager._globalUniqueId += 1;
    return fsm.StateManager._globalUniqueId;
};

/**
 * Creates a well formed token
 * @param {string} name The display name of the token
 * @returns {Token} Returns a well formed token
 */
fsm.StateManager.makeToken = function (name) {
    'use strict';
    // create a token object with a unique identifier and the desired display name
    return { id: fsm.StateManager.nextId(), name: name };
};

//noinspection JSUnusedGlobalSymbols
/**
 * Registers a state into the state machine
 * @param {State} state
 */
fsm.StateManager.prototype.registerState = function (state) {
    'use strict';

    // checks the state argument validity
    if (!state) {
        throw new Error('Invalid state argument.');
    }

    // checks the state token validity
    fsm.StateManager.checkTokenValidity(state.token);

    // check whether the given state is already registered
    //noinspection JSUnresolvedFunction
    if (this._states.some(function (s) { return state.token.id === s.token.id; })) {
        throw new Error('State \'' + state.token.name + '\' already registered');
    }

    // inject an action handlers array into the state for storage
    //noinspection JSUndefinedPropertyAssignment
    state._handlers = {};

    // inject an async handling check flag in the state
    //noinspection JSUndefinedPropertyAssignment
    state._isHandlingAsync = false;

    // add the state the registered states array
    this._states.push(state);

    // keep track of the current execution context
    var self = this;

    // inject the getStateManager() method into the state
    state.getStateManager = function () {
        return self;
    };

    // inject the getContext() method into the state
    state.getContext = function () {
        return self._context;
    };

    // inject the isHandlingAsync() method into the state
    state.isHandlingAsync = function () {
        return state._isHandlingAsync;
    };

    // inject the registerActionHandler() method into the state
    state.registerActionHandler = function (action, handler) {
        return fsm.StateManager._injectableRegisterActionHandler(state, action, handler);
    };

    // inject the _handle() private method into the state
    //noinspection JSUndefinedPropertyAssignment
    state._handle = function (action, data, decisionCallback) {
        return fsm.StateManager._injectableHandle(state, action, data, decisionCallback);
    };

    // calls the onInitialize() method of the state if available
    if (typeof state.onInitialize === 'function') {
        state.onInitialize();
    }
};

// exports the StateManager class
module.exports = fsm;

/**
 * A state changed handler
 * @callback StateChangedHandler
 * @param {StateChangedEventArgs} eventArg
 */

/**
 * A state transition decision handler
 * @callback StateTransitionDecisionHandler
 * @param {Token} action
 * @param {*} [data]
 */

/**
 * An action handler
 * @callback ActionHandler
 * @param {*} data
 * @param {StateTransitionDecisionHandler} cb
 */
