#ifndef __BITCRAFT_STATEMACHINE_ACTION_TOKEN_H__
#define __BITCRAFT_STATEMACHINE_ACTION_TOKEN_H__

#include "token.h"

namespace Bitcraft
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

#endif // __BITCRAFT_STATEMACHINE_ACTION_TOKEN_H__
