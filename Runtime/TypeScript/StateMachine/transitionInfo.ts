import token = require('token');

export class TransitionInfo {
    /// <summary>
    /// Gets the target state token. (state active before transition)
    /// </summary>
    targetState: token.StateToken;

    /// <summary>
    /// Gets the data provided from the target state, for the source state.
    /// </summary>
    targetStateData: any;
}
