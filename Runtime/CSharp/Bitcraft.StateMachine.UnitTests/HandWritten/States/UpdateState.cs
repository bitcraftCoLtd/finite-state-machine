using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bitcraft.StateMachine.UnitTests.HandWritten.States
{
    public class UpdateState : StateBase
    {
        public UpdateState()
            : base(HandWrittenStateTokens.UpdateStateToken)
        {
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            RegisterActionHandler(HandWrittenActionTokens.UpdateAction, (d, cb) => cb(HandWrittenStateTokens.UpdateStateToken));
            RegisterActionHandler(HandWrittenActionTokens.TerminateAction, (d, cb) => cb(HandWrittenStateTokens.EndStateToken));
        }

        protected override void OnEnter(StateEnterEventArgs e)
        {
            base.OnEnter(e);

            var context = (StateMachineTestContext)Context;

            Assert.AreEqual(context.TestStatus, 2);
            context.TestStatus++;
        }

        protected override void OnExit()
        {
            base.OnExit();

            var context = (StateMachineTestContext)Context;

            Assert.AreEqual(context.TestStatus, 3);
            context.TestStatus++;
        }
    }
}
