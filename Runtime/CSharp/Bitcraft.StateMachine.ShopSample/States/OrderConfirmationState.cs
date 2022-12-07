using System;
using System.Threading.Tasks;

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

        private Task<HandlerResult> OnBack(object _)
        {
            return Task.FromResult(new HandlerResult(ShopStateTokens.PaymentInformation));
        }

        private Task<HandlerResult> OnInput(object data)
        {
            _ = (InputInfo)data;

            return Task.FromResult(new HandlerResult(Token)); // Remain on the same state.
        }

        private Task<HandlerResult> OnNext(object _)
        {
            return Task.FromResult(new HandlerResult(ShopStateTokens.ThankYou));
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
