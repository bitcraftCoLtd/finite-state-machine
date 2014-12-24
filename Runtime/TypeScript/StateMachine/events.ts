import token = require('token');
import transitionInfo = require('transitionInfo');
import stateBase = require('stateBase');

export class StateEnterEventArgs {

    private _from: token.StateToken;
    private _data: any;
    private _redirect: transitionInfo.TransitionInfo;

    /// <summary>
    /// Gets the source state token.
    /// </summary>
    getFrom(): token.StateToken {
        return this._from;
    }

    /// <summary>
    /// Gets the data provided from source, for the target.
    /// </summary>
    getData(): any {
        return this._data;
    }

    /// <summary>
    /// Gets a TransitionInfo object used for potential redirection directly from an Enter state event.
    /// </summary>
    getRedirect(): transitionInfo.TransitionInfo {
        return this._redirect;
    }

    /// <summary>
    /// Initializes the StateEnterEventArgs instance.
    /// </summary>
    /// <param name="from">The source state of the transition.</param>
    /// <param name="data">The data provided from the source state, for the target state.</param>
    constructor(from: token.StateToken, data: any) {
        this._redirect = new transitionInfo.TransitionInfo();
        this._from = from;
        this._data = data;
    }
}

export class StateChangedEventArgs {

    private _oldState: stateBase.StateBase;
    private _newState: stateBase.StateBase;

    /// <summary>
    /// Gets the previous state. (the state before transition)
    /// </summary>
    getOldState(): stateBase.StateBase {
        return this._newState;
    }

    /// <summary>
    /// Gets the new state. (the state after transition)
    /// </summary>
    getNewState(): stateBase.StateBase {
        return this._newState;
    }

    /// <summary>
    /// Initializes the StateChangedEventArgs instance.
    /// </summary>
    /// <param name="oldState">Old state.</param>
    /// <param name="newState">New state.</param>
    constructor(oldState: stateBase.StateBase, newState: stateBase.StateBase) {
        this._oldState = oldState;
        this._newState = newState;
    }
}
