using System;
using System.Collections.Generic;

namespace Anagramimation
{

public static class ExtensionMethods
{
    public static T GetRandom<T>(this IReadOnlyList<T> list, Random? random = null)
    {
        random ??= new Random();
        var r = list[random.Next(list.Count)];
        return r;
    }
}

}
