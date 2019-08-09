using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bitcraft.StateMachine.UnitTests.HandWritten
{
    public static class HandWrittenActionTokens
    {
        public static readonly ActionToken InitDoneAction = new ActionToken("InitDone");
        public static readonly ActionToken UpdateAction = new ActionToken("Update");
        public static readonly ActionToken TransitionAction = new ActionToken("Transition");
        public static readonly ActionToken TerminateAction = new ActionToken("Terminate");
        public static readonly ActionToken FinalizeAction = new ActionToken("Finalize");
    }
}
