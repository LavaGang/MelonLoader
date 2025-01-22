using System;
using System.Collections;
using System.Collections.Generic;

namespace MelonLoader.TinyJSON;

[Obsolete("Please use Newtonsoft.Json or System.Text.Json instead. This will be removed in a future version.", true)]
public sealed class ProxyArray : Variant, IEnumerable<Variant>
{
    private readonly List<Variant> list;

    public ProxyArray()
    {
        list = [];
    }

    IEnumerator<Variant> IEnumerable<Variant>.GetEnumerator()
    {
        return list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return list.GetEnumerator();
    }

    public void Add(Variant item)
    {
        list.Add(item);
    }

    public override Variant this[int index]
    {
        get
        {
            return list[index];
        }
        set
        {
            list[index] = value;
        }
    }

    public int Count
    {
        get
        {
            return list.Count;
        }
    }

    internal bool CanBeMultiRankArray(int[] rankLengths)
    {
        return CanBeMultiRankArray(0, rankLengths);
    }

    private bool CanBeMultiRankArray(int rank, int[] rankLengths)
    {
        var count = list.Count;
        rankLengths[rank] = count;

        if (rank == rankLengths.Length - 1)
        {
            return true;
        }

        if (list[0] is not ProxyArray firstItem)
        {
            return false;
        }

        var firstItemCount = firstItem.Count;

        for (var i = 1; i < count; i++)
        {
            if (list[i] is not ProxyArray item)
            {
                return false;
            }

            if (item.Count != firstItemCount)
            {
                return false;
            }

            if (!item.CanBeMultiRankArray(rank + 1, rankLengths))
            {
                return false;
            }
        }

        return true;
    }
}
