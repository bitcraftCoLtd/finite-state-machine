﻿using System;

namespace Bitcraft.StateMachine
{
    /// <summary>
    /// Represents the response to performing an action on the state machine.
    /// </summary>
    public enum ActionResultType
    {
        /// <summary>
        /// Action has performed successfully.
        /// </summary>
        Success,

        /// <summary>
        /// Error, the current state does not know about the given action.
        /// </summary>
        ErrorUnknownAction,

        /// <summary>
        /// Error, the state machine is already performing an action asynchronously.
        /// </summary>
        ErrorAlreadyPerformingAction,

        /// <summary>
        /// Error, cannot perform action from special events such as OnInitialize, OnEnter and OnExit.
        /// </summary>
        ErrorForbiddenFromSpecialEvents,
    }
}
