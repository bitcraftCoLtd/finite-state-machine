using System.Threading.Tasks;
using Xunit;

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
            RegisterActionHandler(HandWrittenActionTokens.InitDoneAction, OnInitDone);
        }

        private Task<HandlerResult> OnInitDone(object data)
        {
            return Task.FromResult(new HandlerResult(HandWrittenStateTokens.UpdateStateToken, data));
        }

        protected override void OnEnter(StateEnterEventArgs e)
        {
            base.OnEnter(e);

            var context = (StateMachineTestContext)Context;

            Assert.Equal(0, context.TestStatus);
            context.TestStatus++;
        }

        protected override void OnExit(StateExitEventArgs e)
        {
            base.OnExit(e);

            var context = (StateMachineTestContext)Context;

            Assert.Equal(1, context.TestStatus);
            context.TestStatus++;
        }
    }
}
