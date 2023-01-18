using Bitcraft.StateMachine.ShopSample.States;

namespace Bitcraft.StateMachine.ShopSample
{
    public class ShopStateMachine : StateManager
    {
        public const PaymentMeans DefaultPaymentMean = PaymentMeans.Bitcoins;

        public bool IsFastBuyActive { get; set; }
        public PaymentMeans PaymentMean { get; set; }

        public ShopStateMachine(Basket basket)
            : base(basket)
        {
            RegisterState(new ProductListState());
            RegisterState(new ConfirmBasketState());
            RegisterState(new PaymentInformationState());
            RegisterState(new OrderConfirmationState());
            RegisterState(new ThankYouState());

            SetInitialState(ShopStateTokens.ProductList);
        }
    }
}
