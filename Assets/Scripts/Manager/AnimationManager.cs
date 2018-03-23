using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private bool isDestory = false;
    private Animator animator_;
    public Animator animator
    {
        get
        {
            if (animator_ == null)
            {
                animator_ = GetComponentInChildren<Animator>();
            }
            return animator_;
        }
    }

    private void OnDestroy()
    {
        isDestory = true;
    }

    public void Play(string name)
    {
        if (isDestory || animator == null) return;
        animator.Play(name);
    }

    public void SetSpeed(float speed)
    {
        animator.speed = speed;
    }

    public float GetCurrTime()
    {
        float time = animator.GetCurrentAnimatorStateInfo(0).length;
        if (float.IsInfinity(time))
        {
            time = 0.5f;
        }
        return time;
    }
}
