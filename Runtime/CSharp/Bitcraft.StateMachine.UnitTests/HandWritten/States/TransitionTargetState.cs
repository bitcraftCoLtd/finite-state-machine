using System.Threading.Tasks;

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

            RegisterActionHandler(HandWrittenActionTokens.UpdateAction, OnUpdate);
        }

        private Task<HandlerResult> OnUpdate(object _)
        {
            return Task.FromResult(new HandlerResult(HandWrittenStateTokens.TransitionStateToken, null));
        }

        protected override void OnEnter(StateEnterEventArgs e)
        {
            GetContext<StateMachineTestContext>().TestStatus++;

            e.Redirect.TargetState = HandWrittenStateTokens.TransitionStateToken;
            e.Redirect.TriggeringAction = HandWrittenActionTokens.InitDoneAction;
        }
    }
}
