import token = require('token');
import stateManager = require('stateManager');
import events = require('events');
import transitionInfo = require('transitionInfo');

export class StateBase {

    private _handlers: {};
    private _token: token.StateToken;
    private _stateManager: stateManager.StateManager;

    constructor(token: token.StateToken) {
        if (!token)
            throw new Error('Invalid token argument.');

        this._token = token;
    }

    getToken(): token.StateToken {
        return this._token;
    }

    getStateManager(): stateManager.StateManager {
        return this._stateManager;
    }

    getContext(): any {
        if (!this._stateManager) {
            return null;
        }
        return this._stateManager.getContext();
    }

    onInitialized(): void { }
    onEnter(e: events.StateEnterEventArgs): void { }
    onExit(e: events.StateExitEventArgs): void { }

    /* internal */ __initialize(parent: stateManager.StateManager): void {
        this._stateManager = parent;
        if (typeof this.onInitialized === 'function') {
            this.onInitialized();
        }
    }

    registerHandler(action: token.ActionToken, handler: any): void {
        var key = action.getIdentifier().toString();
        if (this._handlers[key]) {
            throw new Error('Action already registered');
        }
        this._handlers[key] = handler;
    }

    /* internal */ __handle(action: token.ActionToken, data: any): transitionInfo.TransitionInfo {
        if (!action) {
            throw new Error('Invalid action argument.');
        }

        var key = action.getIdentifier().toString();
        if (!this._handlers[key]) {
            throw new Error('Unknown action. (' + action.toString() + ')');
        }

        var result = this._handlers[key](data);

        if (typeof result === 'TransitionInfo') {
            return result;
        } else if (typeof result === 'StateToken') {
            var ti = new transitionInfo.TransitionInfo();
            ti.targetStateData = data;
            ti.targetState = result;
            return ti;
        } else {
            throw new Error('Invalid handler result');
        }
    }
}
