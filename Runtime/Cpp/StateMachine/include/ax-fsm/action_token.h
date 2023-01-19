#pragma once

#include "ax-fsm/token.h"

namespace AX
{
    namespace StateMachine
    {
        /// <summary>
        /// Represents a state machine action.
        /// </summary>
        class ActionToken : public Token
        {
        public:
            /// <summary>
            /// Initializes the ActionToken instance.
            /// </summary>
            ActionToken()
                : Token()
            {
            }

            /// <summary>
            /// Initializes the ActionToken instance.
            /// </summary>
            /// <param name="name">Name of the action token.</param>
            ActionToken(wchar_t* name)
                : Token(name)
            {
            }
        };
    }
}
