using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : ActorObject
{
    public uint actorId;

    private float findTime;
    private float attackTime;
    private Vector2 originPos;

    protected override void Awake()
    {
        base.Awake();
        if (actorId != 0)
        {
            actorData = new ActorData(actorId);
            GameData.enemys.Add(this);
        }
    }

    private void Update()
    {
        if (navMeshAgent2D.remainingDistance > 0)
        {
            UpdateDirect(navMeshAgent2D.velocity);
        }
    }

    protected override void Start()
    {
        base.Start();
        originPos = transform.position;
        StartCoroutine(StartAI());
    }

    IEnumerator StartAI()
    {
        while (true)
        {
            yield return null;

            if (Vector2.Distance(transform.position , originPos) > actorData.cfgVo.PatrolRange * MapManager.textSize)
            {
                if (actionStatus != ActionStatus.Back)
                {
                    actionStatus = ActionStatus.Back;
                    navMeshAgent2D.Resume();
                    animationManager.Play(AnimationName.Run);
                    navMeshAgent2D.SetDestination(originPos);
                }

                if (navMeshAgent2D.remainingDistance > 0)
                {
                    animationManager.Play(AnimationName.Run);
                }
                else
                {
                    animationManager.Play(AnimationName.Idle);
                }
            }
            else
            {
                if(actionStatus == ActionStatus.Back)
                {
                    if (navMeshAgent2D.remainingDistance == 0)
                    {
                        actionStatus = ActionStatus.Idle;
                    }
                }
                else
                {
                    if (targetObject != null && (targetObject.IsDead || Vector3.Distance(transform.position, targetObject.transform.position) > actorData.cfgVo.FindRange * MapManager.textSize))
                    {
                        targetObject = null;
                    }
                    else if (targetObject == null && Time.time > findTime)
                    {
                        findTime = Time.time + Random.Range(0.3f, 0.5f);
                        if (GameData.myself != null && !GameData.myself.IsDead)
                        {
                            float distance = Vector3.Distance(transform.position, GameData.myself.transform.position);
                            if (distance < actorData.cfgVo.FindRange * MapManager.textSize)
                            {
                                targetObject = GameData.myself;
                            }
                        }
                    }

                    if (targetObject == null)
                    {
                        if (actionStatus != ActionStatus.Back)
                        {
                            actionStatus = ActionStatus.Back;
                            navMeshAgent2D.Resume();
                            navMeshAgent2D.SetDestination(originPos);
                        }

                        if (navMeshAgent2D.remainingDistance > 0)
                        {
                            animationManager.Play(AnimationName.Run);
                        }
                        else
                        {
                            animationManager.Play(AnimationName.Idle);
                        }
                    }
                    else
                    {
                        float distance = Vector3.Distance(transform.position, targetObject.transform.position);
                        if (distance > actorData.cfgVo.AttackDistance * MapManager.textSize)
                        {
                            actionStatus = ActionStatus.Follow;
                            navMeshAgent2D.Resume();
                            animationManager.Play(AnimationName.Run);
                            navMeshAgent2D.SetDestination(targetObject.transform.position);
                        }
                        else
                        {
                            UpdateDirect(targetObject.transform.position - transform.position);
                            actionStatus = ActionStatus.Attack;
                            if (Time.time > attackTime)
                            {
                                navMeshAgent2D.Stop();
                                animationManager.Play(AnimationName.Attack);
                                attackTime = Time.time + actorData.cfgVo.AttackInterval;
                                yield return new WaitForSeconds(Mathf.Min(actorData.cfgVo.AttackInterval, animationManager.GetCurrTime()));
                                animationManager.SetSpeed(1);
                            }
                        }
                    }
                }
            }
        }      
    }
}
