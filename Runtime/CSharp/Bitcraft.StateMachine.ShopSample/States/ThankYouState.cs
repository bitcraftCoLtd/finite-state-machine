using System.Threading.Tasks;

namespace Bitcraft.StateMachine.ShopSample.States
{
    public class ThankYouState : ShopStateBase
    {
        public ThankYouState()
            : base(ShopStateTokens.ThankYou)
        {
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            // No Back support on Thank You screen.
            RegisterActionHandler(ShopActionTokens.Next, OnNext);
        }

        private Task<HandlerResult> OnNext(object _)
        {
            var basket = GetContext<Basket>();
            basket.Clear();

            var sm = (ShopStateMachine)StateManager;
            sm.PaymentMean = PaymentMeans.Undefined;

            return Task.FromResult(new HandlerResult(ShopStateTokens.ProductList));
        }
    }
}
