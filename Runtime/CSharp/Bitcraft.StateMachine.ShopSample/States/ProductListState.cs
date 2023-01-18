using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bitcraft.StateMachine.ShopSample.States
{
    public class ProductListState : ShopStateBase
    {
        public ProductListState()
            : base(ShopStateTokens.ProductList)
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
            return Task.FromResult(new HandlerResult(null)); // Move to terminal state.
        }

        private Task<HandlerResult> OnInput(object data)
        {
            var inputData = (InputInfo)data;
            int num = inputData.Number;

            var basket = GetContext<Basket>();

            var index = num - 1;
            var keys = Enum.GetValues(typeof(AvailableItems)).Cast<AvailableItems>().ToArray();

            if (num == 0)
            {
                var sm = (ShopStateMachine)StateManager;
                sm.IsFastBuyActive = !sm.IsFastBuyActive;

                PrintMenu();
            }
            else if (index >= 0 && index < keys.Length)
            {
                var key = keys[index];

                if (basket.ContainsKey(key) == false)
                    basket[key] = 0;

                if ((inputData.Modifier & ConsoleModifiers.Control) == ConsoleModifiers.Control)
                    basket[key] = Math.Max(0, basket[key] - 1);
                else
                    basket[key]++;

                PrintMenu();
            }

            return Task.FromResult(new HandlerResult(Token)); // Remain on the same state.
        }

        private Task<HandlerResult> OnNext(object _)
        {
            return Task.FromResult(new HandlerResult(ShopStateTokens.ConfirmBasket));
        }

        protected override void PrintMenu()
        {
            base.PrintMenu();

            var basket = GetContext<Basket>();

            var values = Enum.GetValues(typeof(AvailableItems)).Cast<AvailableItems>().ToArray();

            string activePrefix = ((ShopStateMachine)StateManager).IsFastBuyActive ? string.Empty : "in";

            Console.WriteLine("Selection: (Ctrl + key to decrase)");
            Console.WriteLine();
            Console.WriteLine($"   0. Toggle fast buy (currently {activePrefix}active)");
            Console.WriteLine();
            for (int i = 0; i < values.Length; i++)
            {
                basket.TryGetValue(values[i], out int amount);
                Console.WriteLine($"   {i + 1}. {values[i]} (amount: {amount})");
            }
        }
    }
}
