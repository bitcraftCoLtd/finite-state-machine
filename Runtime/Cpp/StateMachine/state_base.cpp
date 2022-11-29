#include "state_base.h"
#include "exceptions.h"

namespace Bitcraft
{
    namespace StateMachine
    {
        StateBase::StateBase(StateToken* token)
        {
            if (token == NULL)
                throw new invalid_argument("Invalid 'token' argument.");

            _stateManager = NULL;
            _token = token;
        }

        const StateToken* const StateBase::GetToken() const
        {
            return _token;
        }

        StateManager* StateBase::GetStateManager()
        {
            return _stateManager;
        }

        void* StateBase::GetContext()
        {
            if (_stateManager == NULL)
                return NULL;

            return _stateManager->GetContext();
        }

        void StateBase::Initialize(StateManager* parent)
        {
            _stateManager = parent;
            OnInitialized();
        }

        void StateBase::OnInitialized() { }

        void StateBase::OnEnter(StateEnterEventArgs* e) { }
        void StateBase::OnExit() { }

        void StateBase::RegisterActionHandler(ActionToken* action, StateHandler handler)
        {
            if (_handlers.find(action) != _handlers.end())
                throw new exception("Action already registered.");

            _handlers[action] = handler;
        }

        void StateBase::Handle(ActionToken* action, StateData* data, TransitionInfo* result)
        {
            if (action == NULL)
                throw new invalid_argument("Invalid 'action' argument.");

            map<ActionToken*, StateHandler>::iterator it = _handlers.find(action);
            if (it != _handlers.end())
                it->second(data, result);
            else
                throw new UnknownActionException(action, GetToken());
        }
    }
}
