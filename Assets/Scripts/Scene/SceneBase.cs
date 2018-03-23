using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBase : MonoBehaviour
{
    static public SceneBase actionItem;

    public float size = 0.1f;

    virtual public void CancelAction()
    {

    }
}
