using System;

namespace Bitcraft.StateMachine
{
    /// <summary>
    /// Represents information of a state machine transition.
    /// </summary>
    public class TransitionInfo
    {
        /// <summary>
        /// Gets the target state token. (state active before transition)
        /// </summary>
        public StateToken TargetStateToken { get; set; }

        /// <summary>
        /// Gets the data provided from the target state, for the source state.
        /// </summary>
        public object TargetStateData { get; set; }
    }
}
