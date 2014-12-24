using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bitcraft.StateMachine.UnitTests.HandWritten
{
    public static class HandWrittenActionTokens
    {
        public static readonly ActionToken InitDoneAction = new ActionToken();
        public static readonly ActionToken UpdateAction = new ActionToken();
        public static readonly ActionToken TerminateAction = new ActionToken();
        public static readonly ActionToken FinalizeAction = new ActionToken();
    }
}
