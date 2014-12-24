using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.StateMachine.ShopSample
{
    public enum AvailableItems
    {
        Potion,
        Bombs,
        Arrows,
        Sword,
        Bow,
        Shield,
        Armor
    }

    public class Basket : Dictionary<AvailableItems, int>
    {
        public bool IsEmpty
        {
            get
            {
                return Keys.All(k => this[k] <= 0);
            }
        }

        public void Print(TextWriter w)
        {
            if (IsEmpty)
                Console.WriteLine("Your basket is empty.");
            else
            {
                Console.WriteLine("Content of your basket:");
                Console.WriteLine();
                foreach (var key in Keys.Where(k => this[k] > 0))
                    Console.WriteLine("   {0} x{1}", key, this[key]);
            }
        }
    }
}
