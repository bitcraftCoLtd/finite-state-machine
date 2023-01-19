// Copyright ax, Inc. All Rights Reserved.

#include "ax-fsm/token.h"

namespace ax { namespace fsm
{
    uint32_t Token::_globalId = 1;

    Token::Token()
    {
        Initialization(nullptr);
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
        return _name == nullptr ? L"(no name)" : _name;
    }

    bool Token::Equals(const Token* const other) const
    {
        if (other == nullptr)
            return false;

        return _id == other->_id;
    }
} }
