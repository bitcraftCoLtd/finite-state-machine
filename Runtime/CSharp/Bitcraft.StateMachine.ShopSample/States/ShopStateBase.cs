using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bitcraft.StateMachine.ShopSample.States
{
    public abstract class ShopStateBase : StateBase
    {
        protected ShopStateBase(StateToken token)
            : base(token)
        {
        }

        protected override void OnEnter(StateEnterEventArgs e)
        {
            base.OnEnter(e);
            PrintMenu();
        }

        protected virtual void PrintMenu()
        {
            Console.Clear();

            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write("You are on the ");

            var color2 = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(Token.ToString());
            Console.ForegroundColor = color2;

            Console.WriteLine(" page");
            Console.WriteLine("Press [Esc] to cancel or go back, [Enter] to validate");
            Console.WriteLine();

            Console.ForegroundColor = color;
        }
    }
}
