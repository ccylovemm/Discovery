using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextEx : MonoBehaviour
{
    private Text textDesc;

    private string str;

    private bool show;
    private int count;

    private float deTime = 0;

    private void Awake()
    {
        textDesc = GetComponent<Text>();
    }

    private void Update()
    {
        if (show)
        {
            if (Time.time < deTime) return;
            deTime = Time.time + 0.05f;

            count++;
            if (count < str.Length)
            {
                textDesc.text = str.Substring(0, count + 1);
            }
            else
            {
                show = false;
            }
        }
    }

    public string text
    {
        set
        {
            textDesc.text = "";
            str = value;
            show = true;
            count = 0;
        }
        get
        {
            return textDesc.text;
        }
    }
}
