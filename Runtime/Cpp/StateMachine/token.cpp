#include "ax-fsm/token.h"

namespace AX
{
    namespace StateMachine
    {
        uint32_t Token::_globalId = 1;

        Token::Token()
        {
            Initialization(NULL);
        }

        Token::Token(const wchar_t* name)
        {
            Initialization(name);
        }

        void Token::Initialization(const wchar_t* name)
        {
            _id = _globalId++;
            _name = name;
        }

        const wchar_t* Token::ToString() const
        {
            return _name == NULL ? L"(no name)" : _name;
        }

        bool Token::Equals(const Token* const other) const
        {
            if (other == NULL)
                return false;

            return _id == other->_id;
        }
    }
}
