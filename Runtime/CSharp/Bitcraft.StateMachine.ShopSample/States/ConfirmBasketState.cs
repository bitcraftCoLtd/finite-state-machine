using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        private void OnBack(object data, Action<StateToken> cb)
        {
            cb(ShopStateTokens.ProductList);
        }

        private void OnNext(object data, Action<StateToken> cb)
        {
            var basket = GetContext<Basket>();

            if (basket.IsEmpty)
                cb(Token); // remain on the same state
            else
                cb(ShopStateTokens.PaymentInformation);
        }

        protected override void PrintMenu()
        {
            base.PrintMenu();

            var basket = GetContext<Basket>();

            basket.Print(Console.Out);
        }
    }
}
