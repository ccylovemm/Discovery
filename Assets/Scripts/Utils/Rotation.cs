using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
	void Update ()
    {
        transform.eulerAngles += new Vector3(0 , 0 , -4);
	}
}
