using System;
using System.Threading.Tasks;

namespace Bitcraft.StateMachine.ShopSample
{
    class Program
    {
        static async Task Main()
        {
            await new Program().Run();
        }

        private async Task Run()
        {
            var basket = new Basket();

            var sm = new ShopStateMachine(basket);

            bool isRunning = true;
            sm.Completed += (ss, ee) => isRunning = false;

            while (isRunning)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Escape)
                    await sm.PerformAction(ShopActionTokens.Back);
                else if (key.Key == ConsoleKey.Enter)
                    await sm.PerformAction(ShopActionTokens.Next);
                else
                {
                    var input = GetNumber(key.Key);
                    if (input > -1)
                        await sm.PerformAction(ShopActionTokens.Input, new InputInfo { Number = input, Modifier = key.Modifiers });
                }
            }
        }

        private int GetNumber(ConsoleKey key)
        {
            var k = (int)key;

            if (k >= (int)ConsoleKey.D0 && k <= (int)ConsoleKey.D9)
                return k - (int)ConsoleKey.D0;
            else if (k >= (int)ConsoleKey.NumPad0 && k <= (int)ConsoleKey.NumPad9)
                return k - (int)ConsoleKey.NumPad0;

            return -1;
        }
    }
}
