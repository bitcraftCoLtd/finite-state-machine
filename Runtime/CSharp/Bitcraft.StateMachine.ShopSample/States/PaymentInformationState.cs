using System;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly PaymentMeans[] paymentValues;

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
                e.Redirect.TargetState = ShopStateTokens.ThankYou;
            }
        }

        private Task<HandlerResult> OnBack(object _)
        {
            return Task.FromResult(new HandlerResult(ShopStateTokens.ConfirmBasket));
        }

        private Task<HandlerResult> OnInput(object data)
        {
            var inputData = (InputInfo)data;

            if (inputData.Number >= 1 && inputData.Number <= paymentValues.Length)
            {
                var sm = (ShopStateMachine)StateManager;
                sm.PaymentMean = paymentValues[inputData.Number - 1];
                PrintMenu();
            }

            return Task.FromResult(new HandlerResult(Token)); // Remain on the same state.
        }

        private Task<HandlerResult> OnNext(object _)
        {
            var sm = (ShopStateMachine)StateManager;

            if (sm.PaymentMean == PaymentMeans.Undefined)
            {
                PrintMenu();
                Console.WriteLine();
                Console.WriteLine("You must select a valid payment mean.");

                return Task.FromResult(new HandlerResult(Token)); // Remain on the same state.
            }

            return Task.FromResult(new HandlerResult(ShopStateTokens.OrderConfirmation));
        }

        protected override void PrintMenu()
        {
            base.PrintMenu();

            var sm = (ShopStateMachine)StateManager;

            string paymentMean = sm.PaymentMean == PaymentMeans.Undefined ? "-" : sm.PaymentMean.ToString();

            Console.WriteLine($"Select your payment mean: (current: {paymentMean})");
            Console.WriteLine();

            for (int i = 0; i < paymentValues.Length; i++)
                Console.WriteLine($"   {i + 1}. {paymentValues[i]}");
        }
    }
}
