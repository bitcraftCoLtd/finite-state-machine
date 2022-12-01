using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bitcraft.StateMachine.UnitTests.HandWritten.States
{
    public class TransitionState : StateBase
    {
        public TransitionState()
            : base(HandWrittenStateTokens.TransitionStateToken)
        {
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            RegisterActionHandler(HandWrittenActionTokens.TerminateAction, (d, cb) =>
            {
                var testContext = GetContext<StateMachineTestContext>();

                Assert.AreEqual(5, testContext.TestStatus);

                testContext.TestStatus--;
                cb(HandWrittenStateTokens.EndStateToken);
            });
        }

        protected override void OnEnter(StateEnterEventArgs e)
        {
            base.OnEnter(e);

            if (e.TriggeringAction != HandWrittenActionTokens.InitDoneAction)
            {
                e.Redirect.TargetState = HandWrittenStateTokens.TransitionTargetStateToken;
                e.Redirect.TriggeringAction = HandWrittenActionTokens.UpdateAction;
            }
        }
    }
}
