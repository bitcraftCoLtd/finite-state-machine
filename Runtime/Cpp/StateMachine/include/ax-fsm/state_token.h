#pragma once

#include "ax-fsm/token.h"

namespace AX
{
    namespace StateMachine
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
            StateToken::StateToken() : Token() { }

            /// <summary>
            /// Initializes the StateToken instance.
            /// </summary>
            /// <param name="name">Name of the state token.</param>
            StateToken::StateToken(wchar_t* name) : Token(name) { }
        };
    }
}
