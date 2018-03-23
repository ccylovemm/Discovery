using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateOrderDynamic : MonoBehaviour
{
    public int offset = 0;

    private Renderer spriteRender;
    private Transform trans;

    void AnimationAttack(string str)
    {
        Debug.Log("aaaaa");
    }

    void Start()
    {
        trans = transform.root;
        spriteRender = GetComponent<Renderer>();
    }

    void Update()
    {
        spriteRender.sortingOrder = (int)(trans.localPosition.y * -1000 + offset);
    }
}
