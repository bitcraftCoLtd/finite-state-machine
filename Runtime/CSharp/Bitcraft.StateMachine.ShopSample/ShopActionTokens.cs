using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.StateMachine.ShopSample
{
    public static class ShopActionTokens
    {
        public static readonly ActionToken Back = new ActionToken("Back");
        public static readonly ActionToken Input = new ActionToken("Input");
        public static readonly ActionToken Next = new ActionToken("Next");
    }
}
