using System.Threading.Tasks;
using Xunit;

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

            RegisterActionHandler(HandWrittenActionTokens.UpdateAction, OnUpdate);
            RegisterActionHandler(HandWrittenActionTokens.TransitionAction, OnTransition);
            RegisterActionHandler(HandWrittenActionTokens.TerminateAction, OnTerminate);
        }

        private Task<HandlerResult> OnUpdate(object _)
        {
            return Task.FromResult(new HandlerResult(HandWrittenStateTokens.UpdateStateToken));
        }

        private Task<HandlerResult> OnTransition(object _)
        {
            return Task.FromResult(new HandlerResult(HandWrittenStateTokens.TransitionStateToken));
        }

        private Task<HandlerResult> OnTerminate(object _)
        {
            return Task.FromResult(new HandlerResult(HandWrittenStateTokens.EndStateToken));
        }

        protected override void OnEnter(StateEnterEventArgs e)
        {
            base.OnEnter(e);

            var context = (StateMachineTestContext)Context;

            Assert.Equal(2, context.TestStatus);
            context.TestStatus++;
        }

        protected override void OnExit(StateExitEventArgs e)
        {
            base.OnExit(e);

            var context = (StateMachineTestContext)Context;

            Assert.Equal(3, context.TestStatus);
            context.TestStatus++;
        }
    }
}
