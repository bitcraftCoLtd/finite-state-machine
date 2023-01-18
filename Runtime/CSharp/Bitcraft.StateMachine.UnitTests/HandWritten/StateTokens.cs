namespace Bitcraft.StateMachine.UnitTests.HandWritten
{
    public static class HandWrittenStateTokens
    {
        public static readonly StateToken BeginStateToken = new StateToken("Begin");
        public static readonly StateToken UpdateStateToken = new StateToken("Update");
        public static readonly StateToken TransitionStateToken = new StateToken("Transition");
        public static readonly StateToken TransitionTargetStateToken = new StateToken("TransitionTarget");
        public static readonly StateToken EndStateToken = new StateToken("End");
    }
}
