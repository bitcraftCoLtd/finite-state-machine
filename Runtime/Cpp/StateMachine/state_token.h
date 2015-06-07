#ifndef __BITCRAFT_STATEMACHINE_STATE_TOKEN_H__
#define __BITCRAFT_STATEMACHINE_STATE_TOKEN_H__

#include "token.h"

namespace Bitcraft
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

#endif // __BITCRAFT_STATEMACHINE_STATE_TOKEN_H__
