using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : Singleton<Tooltip>
{
    public Text text;

    public void ShowTip(string str)
    {
        text.text = str;
    }
}
