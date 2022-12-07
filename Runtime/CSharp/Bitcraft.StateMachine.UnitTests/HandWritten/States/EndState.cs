using System.Threading.Tasks;
using Xunit;

namespace Bitcraft.StateMachine.UnitTests.HandWritten.States
{
    public class EndState : StateBase
    {
        public EndState()
            : base(HandWrittenStateTokens.EndStateToken)
        {
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            RegisterActionHandler(HandWrittenActionTokens.FinalizeAction, OnFinalize);
        }

        private Task<HandlerResult> OnFinalize(object data)
        {
            return Task.FromResult(new HandlerResult(null, data));
        }

        protected override void OnEnter(StateEnterEventArgs e)
        {
            base.OnEnter(e);

            var context = (StateMachineTestContext)Context;

            Assert.Equal(4, context.TestStatus);
            context.TestStatus++;
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
