using System.Threading.Tasks;
using Bitcraft.StateMachine.UnitTests.HandWritten;
using Bitcraft.StateMachine.UnitTests.HandWritten.States;
using Xunit;

namespace Bitcraft.StateMachine.UnitTests
{
    public class HandWrittenStateMachineUnitTests
    {
        [Fact]
        public async Task TestMethod01()
        {
            var context = new StateMachineTestContext();

            var sm = new StateManager(context);

            sm.StateChanged += (ss, ee) => System.Diagnostics.Debug.WriteLine($"State changed from '{ee.OldState}' to '{ee.NewState}'");
            sm.Completed += (ss, ee) =>
            {
                Assert.Equal(6, context.TestStatus);
                ((StateMachineTestContext)sm.Context).TestStatus++;
            };

            sm.RegisterState(new BeginState());
            sm.RegisterState(new UpdateState());
            sm.RegisterState(new EndState());

            sm.SetInitialState(HandWrittenStateTokens.BeginStateToken);

            await sm.PerformAction(HandWrittenActionTokens.InitDoneAction);
            await sm.PerformAction(HandWrittenActionTokens.UpdateAction);
            await sm.PerformAction(HandWrittenActionTokens.UpdateAction);
            await sm.PerformAction(HandWrittenActionTokens.UpdateAction);
            await sm.PerformAction(HandWrittenActionTokens.UpdateAction);
            await sm.PerformAction(HandWrittenActionTokens.UpdateAction);
            await sm.PerformAction(HandWrittenActionTokens.TerminateAction);

            Assert.Equal(5, context.TestStatus);

            await sm.PerformAction(HandWrittenActionTokens.FinalizeAction);

            Assert.Equal(7, context.TestStatus);
        }

        [Fact]
        public async Task TestMethod02()
        {
            var context = new StateMachineTestContext();

            var sm = new StateManager(context);

            sm.StateChanged += (ss, ee) => System.Diagnostics.Debug.WriteLine($"State changed from '{ee.OldState}' to '{ee.NewState}'");
            sm.Completed += (ss, ee) => ((StateMachineTestContext)sm.Context).TestStatus++;

            sm.RegisterState(new BeginState());
            sm.RegisterState(new UpdateState());
            sm.RegisterState(new TransitionState());
            sm.RegisterState(new TransitionTargetState());

            EndState endState = new EndState();
            sm.RegisterState(endState);

            sm.SetInitialState(HandWrittenStateTokens.BeginStateToken);
            await sm.PerformAction(HandWrittenActionTokens.InitDoneAction);
            await sm.PerformAction(HandWrittenActionTokens.UpdateAction);
            await sm.PerformAction(HandWrittenActionTokens.TransitionAction);
            await sm.PerformAction(HandWrittenActionTokens.TerminateAction);

            Assert.Equal(endState, sm.CurrentState);
        }
    }
}
