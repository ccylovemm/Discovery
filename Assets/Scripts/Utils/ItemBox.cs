using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void SetBoxStatus(bool bol)
    {
        animator.Play(bol ? "Open" : "Close");
    }
}
