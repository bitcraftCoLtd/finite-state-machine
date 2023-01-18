﻿using System.Threading.Tasks;
using Xunit;

namespace Bitcraft.StateMachine.UnitTests.AutoGenerated
{
    partial class TestAutoEndState
    {
        protected override void OnEnter(StateEnterEventArgs e)
        {
            base.OnEnter(e);

            var context = (StateMachineTestContext)Context;

            Assert.Equal(4, context.TestStatus);
            context.TestStatus++;
        }

        private Task<HandlerResult> OnTestAutoFinalizeAction(object _)
        {
            return Task.FromResult(new HandlerResult(null));
        }

        protected override void OnExit(StateExitEventArgs e)
        {
            base.OnExit(e);

            var context = (StateMachineTestContext)Context;

            Assert.Equal(5, context.TestStatus);
            context.TestStatus++;
        }
    }
}
