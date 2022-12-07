using System.Threading.Tasks;
using Xunit;

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

            RegisterActionHandler(HandWrittenActionTokens.TerminateAction, OnTerminate);
        }

        private async Task<HandlerResult> OnTerminate(object data)
        {
            var testContext = GetContext<StateMachineTestContext>();

            Assert.Equal(5, testContext.TestStatus);

            testContext.TestStatus--;

            await Task.Yield();

            return new HandlerResult(HandWrittenStateTokens.EndStateToken, data);
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
