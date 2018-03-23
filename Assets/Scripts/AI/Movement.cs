using UnityEngine;
using System.Collections.Generic;

public class Movement : MonoBehaviour
{
    public bool arrive = true;
    private Transform trans;
    private Vector2 targetPos;
    private List<Vector3> paths;

    private ActorObject actorObject;
    private bool isPatrol;
    private float pathTime;

    void Awake()
    {
        trans = transform;
        actorObject = GetComponent<ActorObject>();
    }

    void Update()
    {
        if (actorObject.IsDead || actorObject.isFrozen || actorObject.isDizzy) return;

        if (paths != null && paths.Count > 0)
        {
            if (Time.time - pathTime > 10.0f)
            {
                paths = null;
                arrive = true;
                actorObject.animationManager.Play(AnimationName.Idle);
                return;
            }
            Vector3 direct = paths[0] - trans.position;
            if (direct.magnitude > 0.05f)
            {
                if (paths[0].x > trans.position.x)
                {
                    trans.eulerAngles = new Vector3(0, 0, 0);
                }
                else if (paths[0].x < trans.position.x)
                {
                    trans.eulerAngles = new Vector3(0, 180, 0);
                }
                Vector3 vect3 = direct.normalized * 0.018f * (actorObject.attackState ? actorObject.actorData.cfgVo.AtkMovingSpeed : (!isPatrol ? actorObject.actorData.cfgVo.MovingSpeed : actorObject.actorData.cfgVo.MoveSpeed)) * actorObject.speed;
                trans.position += vect3;
            }
            else
            {
                paths.RemoveAt(0);
                if(paths.Count == 0)
                {
                    arrive = true;
                    actorObject.animationManager.Play(AnimationName.Idle);
                }
            }
        }
    }
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (paths != null && paths.Count > 0)
        {
            Gizmos.color = new Color(1f, 1f, 1f, .2f);
            Gizmos.DrawLine(trans.position , paths[0]);
            for (int i = 0; i < paths.Count - 1; i++)
            {
                Gizmos.DrawLine(paths[i], paths[i + 1]);
            }
            Gizmos.DrawLine(paths[paths.Count - 1], targetPos);
        }
    }
#endif

    public void FlyTo(Vector2 pos)
    {
        targetPos = pos;
        paths = new List<Vector3>();
        paths.Add(pos);
        pathTime = Time.time;
        arrive = false;
        actorObject.animationManager.Play(AnimationName.Run);
    }

    public void MoveTo(Vector2 pos, bool Patrol = false)
    {
        isPatrol = Patrol;
        targetPos = pos;
        Vector2 originGridPos = MapManager.GetGrid(trans.position.x, trans.position.y);
        Vector2 targetGridPos = MapManager.GetGrid(pos.x, pos.y);
        paths = MapManager.FindPath(originGridPos , targetGridPos , actorObject.isFly);
        if (paths != null)
        {
            if (paths.Count > 0) paths.RemoveAt(paths.Count - 1);
            paths.Add(targetPos);
            pathTime = Time.time;
            arrive = false;
            actorObject.animationManager.Play(AnimationName.Run);
        }
    }

    public void Stop()
    {
        arrive = true;
        paths = null;
      //  actorObject.animationManager.Play(AnimationName.Idle);
    }
}

