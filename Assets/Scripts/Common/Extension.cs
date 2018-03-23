using System;
using System.Collections;
using System.Collections.Generic;

public static class Extension
{
    static public void Foreach<T>(this IEnumerable<T> source , Action<T> action)
    {
        foreach(var item in source)
        {
            action(item);
        }
    }
}
