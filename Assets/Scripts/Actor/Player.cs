using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : ActorObject
{
    private Vector2 moveDirect;
    private bool moveState = false;

    private void Update()
    {
        if (IsDead) return;

        if (moveState)
        {
            Vector3 speedVec = (Vector3)moveDirect.normalized * (moveDirect.magnitude / 50.0f);
            Vector3 offsetPos = speedVec * 0.018f * actorData.cfgVo.MoveSpeed;
            transform.position += offsetPos;
        }
    }

    public void MoveState(bool value)
    {
        moveState = !IsDead && value;

        if (IsDead)
        {
            return;
        }
        animationManager.Play(moveState ? AnimationName.Run : AnimationName.Idle);
    }

    public void MoveDirect(Vector2 value)
    {
        if (IsDead)
        {
            return;
        }
        if (moveState)
        {
            moveDirect = value;
            UpdateDirect(moveDirect);
        }
    }
}
