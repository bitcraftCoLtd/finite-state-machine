using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                w.WriteLine("Your basket is empty.");
            else
            {
                w.WriteLine("Content of your basket:");
                w.WriteLine();
                foreach (var key in Keys.Where(k => this[k] > 0))
                    w.WriteLine($"   {key} x{this[key]}");
            }
        }
    }
}
