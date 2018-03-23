using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadRoot : MonoBehaviour
{
    static public Transform root;

    void Awake()
    {
        root = transform;
    }
}
