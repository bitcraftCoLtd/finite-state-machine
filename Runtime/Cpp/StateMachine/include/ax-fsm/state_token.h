// Copyright ax, Inc. All Rights Reserved.

#pragma once

#include "ax-fsm/token.h"

namespace ax { namespace fsm
{
    /// <summary>
    /// Represents a state machine state.
    /// </summary>
    class StateToken : public Token
    {
    public:
        /// <summary>
        /// Initializes the StateToken instance.
        /// </summary>
        StateToken() : Token() { }

        /// <summary>
        /// Initializes the StateToken instance.
        /// </summary>
        /// <param name="name">Name of the state token.</param>
        StateToken(wchar_t* name) : Token(name) { }
    };
} }
