using System;
using System.Collections.Generic;
using System.Linq;

public static class ArrayUtils
{
    public static IEnumerable<T> GetEnumValues<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }

    public static T GetRandomEntry<T>(this List<T> currentList)
    {
        int randomIndex = UnityEngine.Random.Range(0, currentList.Count);
        return currentList[randomIndex];
    }
}
