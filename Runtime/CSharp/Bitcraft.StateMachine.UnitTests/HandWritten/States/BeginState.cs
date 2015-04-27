using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bitcraft.StateMachine.UnitTests.HandWritten.States
{
    public class BeginState : StateBase
    {
        public BeginState()
            : base(HandWrittenStateTokens.BeginStateToken)
        {
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            RegisterActionHandler(HandWrittenActionTokens.InitDoneAction, (d, cb) => cb(HandWrittenStateTokens.UpdateStateToken));
        }

        protected override void OnEnter(StateEnterEventArgs e)
        {
            base.OnEnter(e);

            var context = (StateMachineTestContext)Context;

            Assert.AreEqual(context.TestStatus, 0);
            context.TestStatus++;
        }

        protected override void OnExit(StateExitEventArgs e)
        {
            base.OnExit(e);

            var context = (StateMachineTestContext)Context;

            Assert.AreEqual(context.TestStatus, 1);
            context.TestStatus++;
        }
    }
}
