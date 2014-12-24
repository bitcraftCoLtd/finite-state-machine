using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            // no Back support on Thank You screen
            RegisterActionHandler(ShopActionTokens.Next, OnNext);
        }

        private void OnNext(object data, Action<StateToken> cb)
        {
            var basket = GetContext<Basket>();
            basket.Clear();

            var sm = (ShopStateMachine)StateManager;
            sm.PaymentMean = PaymentMeans.Undefined;

            cb(ShopStateTokens.ProductList);
        }
    }
}
