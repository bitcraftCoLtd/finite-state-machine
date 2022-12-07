namespace Bitcraft.StateMachine.ShopSample
{
    public static class ShopStateTokens
    {
        public static readonly StateToken ProductList = new StateToken("Product List");
        public static readonly StateToken ConfirmBasket = new StateToken("Confirm Basket Content");
        public static readonly StateToken PaymentInformation = new StateToken("Payment Information");
        public static readonly StateToken OrderConfirmation = new StateToken("Order Confirmation");
        public static readonly StateToken ThankYou = new StateToken("Thank You!");
    }
}
