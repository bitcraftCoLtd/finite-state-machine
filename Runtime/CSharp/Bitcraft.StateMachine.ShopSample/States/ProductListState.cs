using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        private void OnBack(object data, Action<StateToken> cb)
        {
            cb(null); // move to terminal state
        }

        private void OnInput(object data, Action<StateToken> cb)
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

            cb(Token); // remain on the same state
        }

        private void OnNext(object data, Action<StateToken> cb)
        {
            cb(ShopStateTokens.ConfirmBasket);
        }

        protected override void PrintMenu()
        {
            base.PrintMenu();

            var basket = GetContext<Basket>();

            var values = Enum.GetValues(typeof(AvailableItems)).Cast<AvailableItems>().ToArray();

            Console.WriteLine("Selection: (Ctrl + key to decrase)");
            Console.WriteLine();
            Console.WriteLine("   0. Toggle fast buy (currently {0}active)", ((ShopStateMachine)StateManager).IsFastBuyActive ? "" : "in");
            Console.WriteLine();
            for (int i = 0; i < values.Length; i++)
            {
                int amount;
                basket.TryGetValue(values[i], out amount);
                Console.WriteLine("   {0}. {1} (amount: {2})", i + 1, values[i], amount);
            }
        }
    }
}
