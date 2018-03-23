using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestory : MonoBehaviour
{
    void Awake()
    {
        GameObject.DontDestroyOnLoad(gameObject);
    }
}
