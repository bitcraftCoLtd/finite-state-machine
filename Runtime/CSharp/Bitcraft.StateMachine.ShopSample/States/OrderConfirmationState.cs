using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bitcraft.StateMachine.ShopSample.States
{
    public class OrderConfirmationState : ShopStateBase
    {
        public OrderConfirmationState()
            : base(ShopStateTokens.OrderConfirmation)
        {
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            RegisterActionHandler(ShopActionTokens.Back, OnBack);
            RegisterActionHandler(ShopActionTokens.Input, OnInput);
            RegisterActionHandler(ShopActionTokens.Next, OnNext);
        }

        private void OnBack(object data, Action<StateToken> cb)
        {
            cb(ShopStateTokens.PaymentInformation);
        }

        private void OnInput(object data, Action<StateToken> cb)
        {
            var inputData = (InputInfo)data;

            cb(Token); // remain on the same state
        }

        private void OnNext(object data, Action<StateToken> cb)
        {
            cb(ShopStateTokens.ThankYou);
        }

        protected override void PrintMenu()
        {
            base.PrintMenu();

            var basket = GetContext<Basket>();
            basket.Print(Console.Out);

            Console.WriteLine();

            var sm = (ShopStateMachine)StateManager;
            Console.WriteLine("You are going to pay by {0}.", sm.PaymentMean);

            Console.WriteLine();

            Console.WriteLine("Please confirm your order.");
            Console.WriteLine("Once you confirm, the order will be placed.");
            Console.WriteLine();
        }
    }
}
