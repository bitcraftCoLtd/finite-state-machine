using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bitcraft.StateMachine.ShopSample.States
{
    public enum PaymentMeans
    {
        Undefined,
        Cash,
        CreditCard,
        Bitcoins
    }

    public class PaymentInformationState : ShopStateBase
    {
        private PaymentMeans[] paymentValues;

        public PaymentInformationState()
            : base(ShopStateTokens.PaymentInformation)
        {
            paymentValues = Enum.GetValues(typeof(PaymentMeans))
                .Cast<PaymentMeans>()
                .Where(x => x != PaymentMeans.Undefined)
                .ToArray();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            RegisterActionHandler(ShopActionTokens.Back, OnBack);
            RegisterActionHandler(ShopActionTokens.Input, OnInput);
            RegisterActionHandler(ShopActionTokens.Next, OnNext);
        }

        protected override void OnEnter(StateEnterEventArgs e)
        {
            base.OnEnter(e);

            var sm = (ShopStateMachine)StateManager;
            if (sm.IsFastBuyActive)
            {
                sm.PaymentMean = ShopStateMachine.DefaultPaymentMean;
                e.Redirect.TargetStateToken = ShopStateTokens.ThankYou;
            }
        }

        private void OnBack(object data, Action<StateToken> cb)
        {
            cb(ShopStateTokens.ConfirmBasket);
        }

        private void OnInput(object data, Action<StateToken> cb)
        {
            var inputData = (InputInfo)data;

            if (inputData.Number >= 1 && inputData.Number <= paymentValues.Length)
            {
                var sm = (ShopStateMachine)StateManager;
                sm.PaymentMean = paymentValues[inputData.Number - 1];
                PrintMenu();
            }

            cb(Token); // remain on the same state
        }

        private void OnNext(object data, Action<StateToken> cb)
        {
            var sm = (ShopStateMachine)StateManager;

            if (sm.PaymentMean == PaymentMeans.Undefined)
            {
                PrintMenu();
                Console.WriteLine();
                Console.WriteLine("You must select a valid payment mean.");

                cb(Token); // remain on the same state

                return;
            }

            cb(ShopStateTokens.OrderConfirmation);
        }

        protected override void PrintMenu()
        {
            base.PrintMenu();

            var sm = (ShopStateMachine)StateManager;

            Console.WriteLine("Select your payment mean: (current: {0})", sm.PaymentMean == PaymentMeans.Undefined ? "-" : sm.PaymentMean.ToString());
            Console.WriteLine();

            for (int i = 0; i < paymentValues.Length; i++)
                Console.WriteLine("   {0}. {1}", i + 1, paymentValues[i]);
        }
    }
}
