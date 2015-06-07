/* version 1.1 */

import stateBase = require('stateBase');
import token = require('token');
import transitionInfo = require('transitionInfo');
import events = require('events');

export class StateManager {

    private _context: any = null;
    private _currentState: stateBase.StateBase = null;
    private _isPerformActionLocked: boolean = false;

    private _states = new Array<stateBase.StateBase>();

    getContext(): any {
        return this._context;
    }

    getCurrentState(): stateBase.StateBase {
        return this._currentState;
    }

    getCurrentStateToken(): token.StateToken {
        return this._currentState ? this._currentState.getToken() : null;
    }

    constructor(context?: any) {
        this._context = context;
    }

    setInitialState(initialState: token.StateToken, data?: any): void {
        if (!initialState) {
            throw new Error('Invalid initialState argument.');
        }

        this._currentState = null;
        this.performTransitionTo(initialState, data);
    }

    private performTransitionTo(stateToken: token.StateToken, data: any): void {

        var targetStateToken = stateToken;
        var targetData = data;

        while (true) {
            var transition = this.transitionTo(targetStateToken, targetData);
            if (!transition.targetStateToken) {
                break;
            }

            targetStateToken = transition.targetStateToken;
            targetData = transition.targetStateData;
        }
    }

    private transitionTo(stateToken: token.StateToken, data: any): transitionInfo.TransitionInfo {
        if (!stateToken) {
            throw new Error('Invalid stateToken argument.');
        }

        var filteredStates = this._states.filter(s => s.getToken().equals(stateToken));
        if (!filteredStates || filteredStates.length == 0) {
            throw new Error('Unknown state ' + stateToken.toString());
        }

        if (!this._currentState) {
            var stateExitEventArgs = new events.StateExitEventArgs(stateToken, data);
            this._isPerformActionLocked = true;
            try {
                this._currentState.onExit(stateExitEventArgs);
            } finally {
                this._isPerformActionLocked = false;
            }
        }

        var oldState = this._currentState;
        this._currentState = filteredStates[0];

        this._isPerformActionLocked = true;
        try {
            this.onStateChanged(new events.StateChangedEventArgs(oldState, this._currentState));

            var stateEnterEventArgs = new events.StateEnterEventArgs(!oldState ? oldState.getToken() : null, data);
            this._currentState.onEnter(stateEnterEventArgs);
        } finally {
            this._isPerformActionLocked = false;
        }

        return stateEnterEventArgs.getRedirect();
    }

    onStateChanged(e: events.StateChangedEventArgs): void {
    }

    onCompleted(): void {
    }

    performAction(action: token.ActionToken, data?: any): void {
        if (!action) {
            throw new Error('Invalid action argument');
        }

        if (!this._currentState) {
            throw new Error('State machine not yet initialized or has reached its final state');
        }

        if (this._isPerformActionLocked) {
            return; // not that good :/
        }

        var transitionInfo = this._currentState.__handle(action, data);

        if (!transitionInfo || !transitionInfo.targetStateToken) {
            this._currentState = null;
            this._isPerformActionLocked = true;
            try {
                this.onCompleted();
            } finally {
                this._isPerformActionLocked = false;
            }
        } else if (this._currentState.getToken().equals(transitionInfo.targetStateToken) == false) {
            this.performTransitionTo(transitionInfo.targetStateToken, transitionInfo.targetStateData);
        }
    }

    registerState(state: stateBase.StateBase): void {
        if (!state) {
            throw new Error('Invalid state argument');
        }

        if (this._states.some(s => state.getToken().equals(s.getToken()))) {
            throw new Error('State already registered');
        }

        this._states.push(state);
        this._isPerformActionLocked = true;
        try {
            state.__initialize(this);
        } finally {
            this._isPerformActionLocked = false;
        }
    }
}
