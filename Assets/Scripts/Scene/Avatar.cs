using System.Collections;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class Avatar : MonoBehaviour
{
    public int actorId;

    private Npc npc;
    private Movement movement;
    private ActorObject actorObject;

    private Vector3 initPos;

    private void Start()
    {
        initPos = transform.position;
        npc = GetComponent<Npc>();
        movement = GetComponent<Movement>();
        actorObject = GetComponent<ActorObject>();
        actorObject.actorData = new ActorData((uint)actorId);
        UpdateAvatar();
    }

    private void OnEnable()
    {
        EventCenter.AddEvent(EventEnum.UpdateAvatar, UpdateAvatar);
    }

    private void OnDisable()
    {
        EventCenter.RemoveEvent(EventEnum.UpdateAvatar, UpdateAvatar);
    }

    public void UpdateAvatar(EventCenterData data = null)
    {
        gameObject.layer = LayerUtil.LayerToActor();
        if (actorId == DataManager.userData.ActorId)
        {
            npc.enabled = false;
            BecomePlayer();
            gameObject.layer = LayerUtil.LayerToPlayer();
        }
        else if (actorId == DataManager.userData.EmployId)
        {
            npc.enabled = false;
            BecomeEmploy();
        }
        else
        {
            npc.enabled = true;
            if (actorObject.headSkill != null)
            {
                actorObject.headSkill.gameObject.SetActive(false);
            }

            if (actorObject.headBar != null)
            {
                actorObject.headBar.gameObject.SetActive(false);
            }

            if (actorObject.behaviorTree != null)
            {
                actorObject.behaviorTree.DisableBehavior();
            }
            if (transform.position != initPos)
            {
                movement.MoveTo(initPos);
            }
        }
    }

    private void BecomePlayer()
    {
        actorObject.actorData = GameData.myData;
        gameObject.AddComponent<PlayerInput>();

        GameData.myself = actorObject;
        EventCenter.DispatchEvent(EventEnum.UpdateMainUIAttackBtn);
        CameraManager.Instance.Init(actorObject.transform);
        GameData.friends.Add(actorObject);
        GameData.allUnits.Add(actorObject);
        ResourceManager.Instance.LoadAsset("resourceassets/gui.assetbundle", headbar =>
        {
            if (actorObject.headSkill == null)
            {
                actorObject.headSkill = GameObject.Instantiate((GameObject)headbar.LoadAsset("HeadSkill.prefab"), HeadRoot.root).GetComponent<HeadSkill>();
                actorObject.headSkill.target = transform.Find("SkillPos");
            }
            else
            {
                actorObject.headSkill.gameObject.SetActive(true);
            }
            if (actorObject.headBar == null)
            {
                actorObject.headBar = GameObject.Instantiate((GameObject)headbar.LoadAsset("PlayerHeadBar.prefab"), HeadRoot.root).GetComponent<HeadBar>();
                actorObject.headBar.target = transform.Find("HeadPos");
            }
            else
            {
                actorObject.headBar.gameObject.SetActive(true);
            }
            actorObject.UpdateHp();
        });
    }

    private void BecomeEmploy()
    {
        ActorData actorData = new ActorData((uint)actorId);
        actorData.currHp = DataManager.userData.EmployHp;
        actorObject.actorData = actorData;
        actorObject.isAI = true;
        actorObject.master = GameData.myself;
        GameData.friends.Add(actorObject);
        GameData.allUnits.Add(actorObject);
        GameData.employ = actorObject;
        StartCoroutine(DelayAddAI(actorData.cfgVo.AIRes));
        ResourceManager.Instance.LoadAsset("resourceassets/gui.assetbundle", headbar =>
        {
            if (actorObject.headBar == null)
            {
                actorObject.headBar = GameObject.Instantiate((GameObject)headbar.LoadAsset("ActorHeadBar.prefab"), HeadRoot.root).GetComponent<HeadBar>();
                actorObject.headBar.target = transform.Find("HeadPos");
            }
            else
            {
                actorObject.headBar.gameObject.SetActive(true);
            }
            actorObject.UpdateHp();
        });
    }

    IEnumerator DelayAddAI(string AIRes)
    {
        yield return new WaitForSeconds(1.0f);
        if (!string.IsNullOrEmpty(AIRes))
        {
            if (actorObject.behaviorTree == null)
            {
                ResourceManager.Instance.LoadAsset("resourceassets/configAssets.assetbundle", config =>
                {
                    actorObject.behaviorTree = gameObject.AddComponent<BehaviorTree>();
                    actorObject.behaviorTree.ExternalBehavior = config.LoadAsset<ExternalBehavior>(AIRes + ".asset");
                    actorObject.behaviorTree.StartWhenEnabled = true;
                    actorObject.behaviorTree.PauseWhenDisabled = true;
                    actorObject.behaviorTree.RestartWhenComplete = true;
                    actorObject.behaviorTree.ResetValuesOnRestart = true;
                });
            }
            else
            {
                actorObject.behaviorTree.EnableBehavior();
            }
        }
    }
}
