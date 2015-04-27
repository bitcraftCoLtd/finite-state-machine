#include <stdio.h>
#include "state_machine.h"

namespace Bitcraft
{
	namespace StateMachine
	{
		StateExitEventArgs::StateExitEventArgs(StateToken* to, StateData* data)
		{
			_to = to;
			_data = data;
		}

		StateToken* StateExitEventArgs::GetTo()
		{
			return _to;
		}

		StateData* StateExitEventArgs::GetData()
		{
			return _data;
		}
	}
}
