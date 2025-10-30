using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionComparer : IEqualityComparer<Resolution>
{
    public bool Equals(Resolution x, Resolution y)
    {
        return x.width == y.width && x.height == y.height;
    }

    public int GetHashCode(Resolution obj)
    {
        return obj.width.GetHashCode() ^ obj.height.GetHashCode();
    }
}
