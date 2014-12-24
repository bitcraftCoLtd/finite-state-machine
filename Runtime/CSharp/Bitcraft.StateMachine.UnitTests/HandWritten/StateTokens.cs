using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bitcraft.StateMachine.UnitTests.HandWritten
{
    public static class HandWrittenStateTokens
    {
        public static readonly StateToken BeginStateToken = new StateToken("Begin");
        public static readonly StateToken UpdateStateToken = new StateToken("Update");
        public static readonly StateToken EndStateToken = new StateToken("End");
    }
}
