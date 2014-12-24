#include <stdio.h>
#include "state_machine.h"

namespace Bitcraft
{
	namespace StateMachine
	{
		unsigned long long Token::_globalId = 1;

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

		const wchar_t* Token::ToString()
        {
            return _name == NULL ? L"(no name)" : _name;
        }

		bool Token::Equals(Token* other)
        {
			if (other == NULL)
				return false;

            return _id == other->_id;
        }
	}
}
