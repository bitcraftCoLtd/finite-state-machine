using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Bitcraft.StateMachine.UnitTests
{
    public class AsyncStateMachineUnitTests
    {
        private static class States
        {
            public static readonly StateToken Begin = new StateToken();
            public static readonly StateToken Update = new StateToken();
            public static readonly StateToken End = new StateToken();
        }

        private static class Actions
        {
            public static readonly ActionToken MoveOn = new ActionToken();
            public static readonly ActionToken Update = new ActionToken();
            public static readonly ActionToken Finalize = new ActionToken();
        }

        private class BeginState : StateBase
        {
            public BeginState()
                : base(States.Begin)
            {
            }

            protected override void OnInitialized()
            {
                base.OnInitialized();

                OnInitializedStatusUpdate(GetContext<StringBuilder>());

                RegisterActionHandler(Actions.MoveOn, OnMoveOn);
            }

            protected override void OnEnter(StateEnterEventArgs e)
            {
                base.OnEnter(e);

                OnEnterStatusUpdate(GetContext<StringBuilder>());
            }

            private async Task<HandlerResult> OnMoveOn()
            {
                OnMoveOnStatusUpdate(GetContext<StringBuilder>());

                await Task.Yield();

                return new HandlerResult(States.Update);
            }

            protected override void OnExit(StateExitEventArgs e)
            {
                base.OnExit(e);

                OnExitStatusUpdate(GetContext<StringBuilder>());
            }
        }

        private class UpdateState : StateBase
        {
            public UpdateState()
                : base(States.Update)
            {
            }

            protected override void OnInitialized()
            {
                base.OnInitialized();

                OnInitializedStatusUpdate(GetContext<StringBuilder>());

                RegisterActionHandler(Actions.MoveOn, OnMoveOn);
                RegisterActionHandler(Actions.Update, OnUpdate);
            }

            protected override void OnEnter(StateEnterEventArgs e)
            {
                base.OnEnter(e);

                OnEnterStatusUpdate(GetContext<StringBuilder>());
            }

            private async Task<HandlerResult> OnMoveOn()
            {
                OnMoveOnStatusUpdate(GetContext<StringBuilder>());

                await Task.Yield();

                return new HandlerResult(States.End);
            }

            private async Task<HandlerResult> OnUpdate()
            {
                OnUpdateStatusUpdate(GetContext<StringBuilder>());

                await Task.Yield();

                return new HandlerResult(States.Update);
            }

            protected override void OnExit(StateExitEventArgs e)
            {
                base.OnExit(e);

                OnExitStatusUpdate(GetContext<StringBuilder>());
            }
        }

        private class EndState : StateBase
        {
            public EndState()
                : base(States.End)
            {
            }

            protected override void OnInitialized()
            {
                base.OnInitialized();

                OnInitializedStatusUpdate(GetContext<StringBuilder>());

                RegisterActionHandler(Actions.Finalize, OnFinalize);
            }

            protected override void OnEnter(StateEnterEventArgs e)
            {
                base.OnEnter(e);

                OnEnterStatusUpdate(GetContext<StringBuilder>());
            }

            private async Task<HandlerResult> OnFinalize()
            {
                OnFinalizeStatusUpdate(GetContext<StringBuilder>());

                await Task.Yield();

                return new HandlerResult(null, 51);
            }

            protected override void OnExit(StateExitEventArgs e)
            {
                base.OnExit(e);

                OnExitStatusUpdate(GetContext<StringBuilder>());
            }
        }

        private static void OnInitializedStatusUpdate(StringBuilder context)
        {
            context.Append("I");
        }

        private static void OnUpdateStatusUpdate(StringBuilder context)
        {
            context.Append("U");
        }

        private static void OnMoveOnStatusUpdate(StringBuilder context)
        {
            context.Append("M");
        }

        private static void OnFinalizeStatusUpdate(StringBuilder context)
        {
            context.Append("F");
        }

        private static void OnEnterStatusUpdate(StringBuilder context)
        {
            context.Append(">");
        }

        private static void OnExitStatusUpdate(StringBuilder context)
        {
            context.Append("<");
        }

        [Fact]
        public async Task TestMethod01()
        {
            var context = new StringBuilder();

            var sm = new StateManager(context);

            sm.RegisterState(new BeginState());
            sm.RegisterState(new UpdateState());
            sm.RegisterState(new EndState());

            sm.SetInitialState(States.Begin);

            Assert.Equal(ActionResultType.Success, await sm.PerformAction(Actions.MoveOn));
            Assert.Equal(ActionResultType.Success, await sm.PerformAction(Actions.Update));
            Assert.Equal(ActionResultType.Success, await sm.PerformAction(Actions.Update));
            Assert.Equal(ActionResultType.Success, await sm.PerformAction(Actions.Update));
            Assert.Equal(ActionResultType.Success, await sm.PerformAction(Actions.MoveOn));
            Assert.Equal(ActionResultType.Success, await sm.PerformAction(Actions.Finalize));

            Assert.Equal("III>M<>UUUM<>F<", context.ToString());
        }
    }
}
