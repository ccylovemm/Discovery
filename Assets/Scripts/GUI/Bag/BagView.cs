using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagView : MonoBehaviour
{
    public void Close()
    {
        WindowManager.Instance.CloseWindow(WindowKey.BagView);
    }
}
