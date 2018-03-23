using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateOrderStatic : MonoBehaviour
{
    public int offset = 0;
    private Renderer spriteRender;

    void Start()
    {
        spriteRender = GetComponent<Renderer>();
        spriteRender.sortingOrder = (int)(transform.root.localPosition.y * -1000 + offset);
    }
}
