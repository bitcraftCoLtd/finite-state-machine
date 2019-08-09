using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bitcraft.StateMachine;
using Bitcraft.StateMachine.UnitTests.HandWritten.States;

namespace Bitcraft.StateMachine.UnitTests.HandWritten
{
    [TestClass]
    public class HandWrittenStateMachineUnitTests
    {
        [TestMethod]
        public void TestMethod01()
        {
            var context = new StateMachineTestContext();

            var sm = new StateManager(context);

            sm.StateChanged += (ss, ee) => System.Diagnostics.Debug.WriteLine(string.Format("State changed from '{0}' to '{1}'", ee.OldState, ee.NewState));
            sm.Completed += (ss, ee) => ((StateMachineTestContext)sm.Context).TestStatus++;

            sm.RegisterState(new BeginState());
            sm.RegisterState(new UpdateState());
            sm.RegisterState(new EndState());

            sm.SetInitialState(HandWrittenStateTokens.BeginStateToken);

            sm.PerformAction(HandWrittenActionTokens.InitDoneAction);
            sm.PerformAction(HandWrittenActionTokens.UpdateAction);
            sm.PerformAction(HandWrittenActionTokens.UpdateAction);
            sm.PerformAction(HandWrittenActionTokens.UpdateAction);
            sm.PerformAction(HandWrittenActionTokens.UpdateAction);
            sm.PerformAction(HandWrittenActionTokens.UpdateAction);
            sm.PerformAction(HandWrittenActionTokens.TerminateAction);

            Assert.AreEqual(5, context.TestStatus);

            sm.PerformAction(HandWrittenActionTokens.FinalizeAction);

            Assert.AreEqual(6, context.TestStatus);
        }

        [TestMethod]
        public void TestMethod02()
        {
            var context = new StateMachineTestContext();

            var sm = new StateManager(context);

            sm.StateChanged += (ss, ee) => System.Diagnostics.Debug.WriteLine(string.Format("State changed from '{0}' to '{1}'", ee.OldState, ee.NewState));
            sm.Completed += (ss, ee) => ((StateMachineTestContext)sm.Context).TestStatus++;

            sm.RegisterState(new BeginState());
            sm.RegisterState(new UpdateState());
            sm.RegisterState(new TransitionState());
            sm.RegisterState(new TransitionTargetState());

            EndState endState = new EndState();
            sm.RegisterState(endState);

            sm.SetInitialState(HandWrittenStateTokens.BeginStateToken);
            sm.PerformAction(HandWrittenActionTokens.InitDoneAction);
            sm.PerformAction(HandWrittenActionTokens.UpdateAction);
            sm.PerformAction(HandWrittenActionTokens.TransitionAction);
            sm.PerformAction(HandWrittenActionTokens.TerminateAction);

            Assert.AreEqual(endState, sm.CurrentState);
        }
    }
}
