#ifndef __BITCRAF_STATEMACHINE_TOKEN_H__
#define __BITCRAF_STATEMACHINE_TOKEN_H__

namespace Bitcraft
{
    namespace StateMachine
    {
        /// <summary>
        /// Represents a uniquely identifiable entity.
        /// </summary>
        class Token
        {
            friend class StateBase;

        private:
            unsigned long long _id;
            const wchar_t* _name;
            static unsigned long long _globalId;

        private:
            void Initialization(const wchar_t* name);

        protected:
            /// <summary>
            /// Initializes the Token instance.
            /// </summary>
            Token();

            /// <summary>
            /// Initializes the Token instance.
            /// </summary>
            /// <param name="name">Name of the token.</param>
            Token(const wchar_t* name);

        public:
            /// <summary>
            /// Provides a string representation of the token.
            /// </summary>
            /// <returns>Returns the string representation of the token.</returns>
            const wchar_t* ToString();

            /// <summary>
            /// Checks whether the current token is the same as another one.
            /// </summary>
            /// <param name="other">The other token to check equality upon.</param>
            /// <returns>Returns true if both tokens are the same, false otherwise.</returns>
            bool Equals(Token* other);
        };
    }
}

#endif // __BITCRAF_STATEMACHINE_TOKEN_H__
