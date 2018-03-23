using UnityEngine;
using System.Collections.Generic;

public class PlayerInput : MonoBehaviour
{
    private float moveVertical;
    private float moveHorizontal;

    private AnimationManager animationManager;
    private ActorObject actorObject;
    private Transform trans;

    void Awake()
    {
        trans = transform;
        animationManager = GetComponent<AnimationManager>();
        actorObject = GetComponent<ActorObject>();
    }

    void Update()
    {
        moveVertical = Input.GetAxis("Vertical");
        moveHorizontal = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        if (actorObject.IsDead) return;
        if (!actorObject.attackState)
        {
            if (moveHorizontal > 0)
            {
                trans.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (moveHorizontal < 0)
            {
                trans.eulerAngles = new Vector3(0, 180, 0);
            }
        }
        Vector3 moveDirect  = new Vector2(moveHorizontal, moveVertical);
        if (moveDirect.magnitude > 0)
        {
            transform.position += moveDirect * 0.01f;
        }
    }
}