using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.StateMachineTool.CodeGenerators
{
    public static class Constants
    {
        public static readonly string StatesFolder = "States";

        public static readonly string StateManagerType = "StateManager";
        public static readonly string StateBaseType = "StateBase";
        public static readonly string StateDataType = "object";

        public static readonly string StateTokensClass = "StateTokens";
        public static readonly string ActionTokensClass = "ActionTokens";

        public static readonly string TokenItemsProperty = "Items";

        public static readonly string StateTokenType = "StateToken";
        public static readonly string ActionTokenType = "ActionToken";

        public static readonly string StateMachineSuffix = "StateMachine";
        public static readonly string StateSuffix = "State";
        public static readonly string ActionSuffix = "Action";

        public static readonly string PreHandlersRegistrationMethod = "PreHandlersRegistration";
        public static readonly string PostHandlersRegistrationMethod = "PostHandlersRegistration";

        public static readonly string OnInitializedMethod = "OnInitialized";
        public static readonly string OnEnterMethod = "OnEnter";
        public static readonly string OnExitMethod = "OnExit";
        public static readonly string RegisterActionHandlerMethod = "RegisterActionHandler";
        public static readonly string RegisterStateMethod = "RegisterState";
        public static readonly string SetInitialStateMethod = "SetInitialState";
    }
}
