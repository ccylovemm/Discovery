using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using BehaviorDesigner.Runtime;

public class SceneManager : Singleton<SceneManager>
{
    public SceneType sceneType;
    public bool levelOver = false;
    private Vector2 playerPos = Vector2.zero;


    public void Enter()
    {

    }

    public void EnterHome()
    {

    }

    public void EnterScene()
    {

    }

    public void EnterBoss()
    {

    }

    public void RandHolePoint()
    {
        Vector2 pos = MapManager.FindGridByRange(GameData.myself.transform.position, 2, 1);
        ResourceManager.Instance.LoadAsset("resourceassets/map.assetbundle", hole =>
        {
            GameObject.Instantiate((GameObject)hole.LoadAsset("Hole.prefab"), MapManager.GetPos((int)pos.x, (int)pos.y), Quaternion.identity);
        });
    }

    public void CreateMonster(Vector2 grid , string groupStr)
    {
        if (groupStr == "") return;
        string[] groups = groupStr.Split(',');
        for (int i = 0; i < groups.Length; i++)
        {
            CreateMonster(new ActorData(uint.Parse(groups[i])), MapManager.GetPos((int)grid.x, (int)grid.y));
        }
    }

    public void CreateActor(ActorData actorData, Vector2 pos)
    {
        ResourceManager.Instance.LoadAsset("resourceassets/avatar.assetbundle", model =>
        {
            GameObject actor = GameObject.Instantiate((GameObject)model.LoadAsset(System.IO.Path.GetFileNameWithoutExtension(actorData.cfgVo.ResName) + ".prefab"));
            actor.transform.position = new Vector3(pos.x, pos.y, 0);
            actor.transform.localScale = Vector3.one * actorData.cfgVo.Scale;
            ActorObject actorObject = actor.GetComponent<ActorObject>();
            actorObject.actorData = actorData;
          
            if (GameData.myData.uniqueId == actorData.uniqueId)
            {
                actor.layer = LayerUtil.LayerToPlayer();

                if (GameData.myself != null)
                {
                    GameObject.Destroy(GameData.myself.gameObject);
                }

                actor.AddComponent<PlayerInput>();

                GameData.myself = actorObject;
                EventCenter.DispatchEvent(EventEnum.UpdateMainUIAttackBtn);
                CameraManager.Instance.Init(actorObject.transform);
            }
            GameData.friends.Add(actorObject);
            GameData.allUnits.Add(actorObject);
            ResourceManager.Instance.LoadAsset("resourceassets/gui.assetbundle", headbar =>
            {
                actorObject.headBar = GameObject.Instantiate((GameObject)headbar.LoadAsset("PlayerHeadBar.prefab"), HeadRoot.root).GetComponent<HeadBar>();
                actorObject.headBar.target = actor.transform.Find("HeadPos");
                actorObject.UpdateHp();
                if (GameData.myData.uniqueId == actorData.uniqueId)
                {
                    actorObject.headSkill = GameObject.Instantiate((GameObject)headbar.LoadAsset("HeadSkill.prefab"), HeadRoot.root).GetComponent<HeadSkill>();
                    actorObject.headSkill.target = actor.transform.Find("SkillPos");
                }
            });
        });
    }

    public void CreateMonster(ActorData actorData, Vector2 pos)
    {
        ResourceManager.Instance.LoadAsset("resourceassets/avatar.assetbundle", model =>
        {
            GameObject actor = GameObject.Instantiate((GameObject)model.LoadAsset(System.IO.Path.GetFileNameWithoutExtension(actorData.cfgVo.ResName) + ".prefab"));
            actor.transform.position = new Vector3(pos.x, pos.y, 0);
            actor.transform.localScale = Vector3.one * actorData.cfgVo.Scale;
            ActorObject actorObject = actor.GetComponent<ActorObject>();
            actorObject.actorData = actorData;
            actorObject.isAI = true;
            if ((ActorType)actorData.cfgVo.Type == ActorType.Boss)
            {
                GameData.boss.Add(actorObject);
            }

            if(!string.IsNullOrEmpty(actorData.cfgVo.AIRes))
            {
                StartCoroutine(DelayAddAI(actorObject, actorData.cfgVo.AIRes));
            }

            GameData.enemys.Add(actorObject);
            GameData.allUnits.Add(actorObject);
            ResourceManager.Instance.LoadAsset("resourceassets/gui.assetbundle", headbar =>
            {
                Transform trans = actor.transform.Find("HeadPos");
                if (trans != null)
                {
                    actorObject.headBar = GameObject.Instantiate((GameObject)headbar.LoadAsset("ActorHeadBar.prefab"), HeadRoot.root).GetComponent<HeadBar>();
                    actorObject.headBar.target = actor.transform.Find("HeadPos");
                    actorObject.UpdateHp();
                    actorObject.headBar.hpBar.gameObject.SetActive((ActorType)actorData.cfgVo.Type != ActorType.Boss);
                }
            });
        });
    }

    IEnumerator DelayAddAI(ActorObject actor , string AIRes)
    {
        yield return new WaitForSeconds(1.0f);
        if (!string.IsNullOrEmpty(AIRes))
        {
            ResourceManager.Instance.LoadAsset("resourceassets/configAssets.assetbundle", config =>
            {
                actor.behaviorTree = actor.gameObject.AddComponent<BehaviorTree>();
                actor.behaviorTree.ExternalBehavior = config.LoadAsset<ExternalBehavior>(AIRes + ".asset");
                actor.behaviorTree.StartWhenEnabled = true;
                actor.behaviorTree.PauseWhenDisabled = true;
                actor.behaviorTree.RestartWhenComplete = true;
                actor.behaviorTree.ResetValuesOnRestart = true;
            });
        }
    }

    public void RandomDrop(uint id, Vector2 originPos)
    {
        if (id == 0) return;
        DropVo dropVo = DropCFG.items[id.ToString()];
        Vector3 targetPos = MapManager.FindDropPos(originPos);
        if (dropVo.Type1 == 1)
        {
            string[] items = dropVo.Reward.Split(',');
            string[] nums = dropVo.Num.Split(',');
            string[] rates = dropVo.Rate.Split(',');

            if (dropVo.Type2 == 1)
            {
                int index = -1;
                int rand = Random.Range(0, 10000);
                for (int i = 0; i < items.Length; i++)
                {
                    if (rand < int.Parse(rates[i]))
                    {
                        index = i;
                        break;
                    }
                }
                if (index != -1)
                {
                   RandomDropItem(uint.Parse(items[index]), uint.Parse(nums[index]), originPos);
                }
            }
            else if (dropVo.Type2 == 2)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    int rand = Random.Range(0, 10000);
                    if (rand < int.Parse(rates[i]))
                    {
                        RandomDropItem(uint.Parse(items[i]), uint.Parse(nums[i]), originPos);
                    }
                }
            }
        }
        else
        {
            ResourceManager.Instance.LoadAsset("resourceassets/item.assetbundle", asset =>
            {
                DropItem item = GameObject.Instantiate((GameObject)asset.LoadAsset(dropVo.ResName + ".prefab"), originPos , Quaternion.identity).AddComponent<DropItem>();
                item.dropVo = dropVo;
                Vector3 pos = new Vector3((originPos.x + targetPos.x) / 2.0f, Mathf.Max(originPos.y, targetPos.y) + 0.2f, 0);
                DOTween.To(() => 0.0f, t => item.transform.position = Bezier.CalculateCubicBezierPoint(t, originPos, pos, targetPos), 1.0f, 0.3f);
            });
        }
    }

    public void RandomDropItem(uint itemId , uint num , Vector3 originPos)
    {
        Vector3 targetPos = MapManager.FindDropPos(originPos);

        ItemVo itemVo = ItemCFG.items[itemId.ToString()];
        ResourceManager.Instance.LoadAsset("resourceassets/item.assetbundle", asset =>
        {
            DropItem item = null;
            if (itemVo.Id == 1)//是金币
            {
                item = GameObject.Instantiate((GameObject)asset.LoadAsset("Item_Gold" + Random.Range(1 , 4) + ".prefab"), originPos, Quaternion.identity).AddComponent<DropItem>();
                item.itemVo = itemVo;
                item.dropNum = num;
            }
            else
            {
                item = GameObject.Instantiate((GameObject)asset.LoadAsset(itemVo.ItemResources + ".prefab"), originPos, Quaternion.identity).AddComponent<DropItem>();
                item.itemVo = itemVo;
                item.dropNum = num;
            }

            Vector3 pos = new Vector3((originPos.x + targetPos.x) / 2.0f , Mathf.Max(originPos.y , targetPos.y) + 0.3f , 0);
            DOTween.To(() => 0.0f, t => item.transform.position = Bezier.CalculateCubicBezierPoint(t, originPos, pos , targetPos), 1.0f, 0.3f);
        });
    }

    private void Clear()
    {
        levelOver = false;
        GameData.allUnits.Clear();
        GameData.friends.Clear();
        GameData.enemys.Clear();
        GameData.boss.Clear();
        SysManager.Instance.sceneObjs.Clear();
        Loading.Instance.Reset();
        WindowManager.Instance.CloseAllWindow();
    }
}
