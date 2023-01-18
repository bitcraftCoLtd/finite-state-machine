using System;
using System.IO;
using System.Threading.Tasks;

namespace Bitcraft.StateMachine.ShopSample.States
{
    public class ConfirmBasketState : ShopStateBase
    {
        public ConfirmBasketState()
            : base(ShopStateTokens.ConfirmBasket)
        {
            RegisterActionHandler(ShopActionTokens.Back, OnBack);
            RegisterActionHandler(ShopActionTokens.Next, OnNext);
        }

        private Task<HandlerResult> OnBack(object _)
        {
            return Task.FromResult(new HandlerResult(ShopStateTokens.ProductList));
        }

        private Task<HandlerResult> OnNext(object _)
        {
            var basket = GetContext<Basket>();

            if (basket.IsEmpty)
                return Task.FromResult(new HandlerResult(Token)); // Remain on the same state.
            else
                return Task.FromResult(new HandlerResult(ShopStateTokens.PaymentInformation));
        }

        protected override void PrintMenu()
        {
            base.PrintMenu();

            var basket = GetContext<Basket>();

            basket.Print(Console.Out);
        }
    }
}
