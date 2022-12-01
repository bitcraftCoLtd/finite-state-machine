using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bitcraft.StateMachine.UnitTests.HandWritten.States
{
    public class TransitionTargetState : StateBase
    {
        public TransitionTargetState()
            : base(HandWrittenStateTokens.TransitionTargetStateToken)
        {
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            RegisterActionHandler(HandWrittenActionTokens.UpdateAction, (d, cb) => cb(HandWrittenStateTokens.TransitionStateToken));
        }
        
        protected override void OnEnter(StateEnterEventArgs e)
        {
            (Context as StateMachineTestContext).TestStatus++;

            e.Redirect.TargetState = HandWrittenStateTokens.TransitionStateToken;
            e.Redirect.TriggeringAction = HandWrittenActionTokens.InitDoneAction;
        }
    }
}
