using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SdkMessageHandler : MonoBehaviour
{
    private AndroidJavaClass androidJavaClass;
    private AndroidJavaObject androidJavaObject;

    void Start ()
    {
        androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
    }

    public void Pay()
    {
        androidJavaClass.Call("Pay");
    }

    public void PayCallBack()
    {

    }
}
