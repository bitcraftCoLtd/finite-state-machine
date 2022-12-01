namespace Bitcraft.StateMachineTool.yWorks;

public class TransitionEqualityComparer : IEqualityComparer<Transition>
{
    public bool Equals(Transition? x, Transition? y)
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

    public int GetHashCode(Transition? obj)
    {
        if (obj == null ||
            string.IsNullOrWhiteSpace(obj.Source) ||
            string.IsNullOrWhiteSpace(obj.Target))
            return 0;

        return $"{obj.Description}:{obj.Source}:{obj.Target}".GetHashCode();
    }
}
