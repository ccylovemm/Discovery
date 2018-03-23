using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public static class LanguageManager
{
    public static string GetText(string key)
    {
        if (LanguageCFG.items.ContainsKey(key))
        {
            return LanguageCFG.items[key].ContentEN;
        }
        return key;
    }
}
