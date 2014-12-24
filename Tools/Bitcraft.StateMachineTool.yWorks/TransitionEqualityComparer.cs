using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.StateMachineTool.yWorks
{
    public class TransitionEqualityComparer : IEqualityComparer<Transition>
    {
        public bool Equals(Transition x, Transition y)
        {
            if (x == null || y == null)
                return false;

            if (string.IsNullOrWhiteSpace(x.Description) ||
                string.IsNullOrWhiteSpace(y.Description) ||
                string.IsNullOrWhiteSpace(x.Source) ||
                string.IsNullOrWhiteSpace(y.Source) ||
                string.IsNullOrWhiteSpace(x.Target) ||
                string.IsNullOrWhiteSpace(y.Target))
                return false;

            return x.Description == y.Description &&
                x.Target == y.Target &&
                x.Source == y.Source;
        }

        public int GetHashCode(Transition obj)
        {
            if (obj == null ||
                string.IsNullOrWhiteSpace(obj.Source) ||
                string.IsNullOrWhiteSpace(obj.Target))
                return 0;

            return string.Format("{0}:{1}:{2}", obj.Description, obj.Source, obj.Target).GetHashCode();
        }
    }
}
