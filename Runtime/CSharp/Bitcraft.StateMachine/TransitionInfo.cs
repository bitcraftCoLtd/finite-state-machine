using System;

namespace Bitcraft.StateMachine
{
    /// <summary>
    /// Represents information of a state machine transition.
    /// </summary>
    public class TransitionInfo
    {
        /// <summary>
        /// Gets or sets the target state token. (state active after transition)
        /// </summary>
        public StateToken TargetStateToken { get; set; }

        /// <summary>
        /// Gets or sets the data to transfer from the current state to the target state.
        /// </summary>
        public object TargetStateData { get; set; }
    }
}
