using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DropItem : SceneBase
{
    public ItemVo itemVo;
    public DropVo dropVo;
    public uint dropNum;

    private bool isGenerate = false;
    private float dropTime = 0;

    private void Awake()
    {
        dropTime = Time.time + 0.6f;
    }

    private void OnDisable()
    {
        CancelAction();
    }

    private void Update()
    {
        if (Time.time > dropTime && itemVo != null && itemVo.Id == 1)
        {
            float distance = Vector2.Distance(transform.position, GameData.myself.currPos);
            if (distance < 0.4f)
            {
                if (distance >= 0.06f)
                {
                    transform.position += (GameData.myself.currPos - transform.position).normalized * 0.05f;
                }
                else
                {
                    DataManager.userData.GoldCoin += (int)dropNum;
                    DataManager.userData.LevelCoin += (int)dropNum;
                    GameObject.Destroy(gameObject);
                }
            }
        }

        if (Time.frameCount % 8 != 0) return;
        if (GameData.myself == null) return;
        if (itemVo != null && itemVo.Id == 1) return;
        if (dropVo != null && dropVo.Type1 == 2 && isGenerate) return;
        if (Vector2.Distance(transform.position, GameData.myself.currPos) - size> 0.1f)
        {
            if (actionItem == this)
            {
                CancelAction();
            }
        }
        else
        {
            if (actionItem == null || Vector2.Distance(actionItem.transform.position, GameData.myself.currPos) - actionItem.size > Vector2.Distance(transform.position, GameData.myself.currPos) - size)
            {
                if (actionItem != null) actionItem.CancelAction();
                actionItem = this;
                EventCenter.DispatchEvent(EventEnum.ActionEvent, new object[] { SceneEventType.PickUp, this });
            }
        }
    }

    public override void CancelAction()
    {
        if (actionItem == this)
        {
            actionItem = null;
            EventCenter.DispatchEvent(EventEnum.ActionEvent, new object[] { SceneEventType.None });
        }
    }

    public void Generate()
    {
        if (isGenerate) return;

        isGenerate = true;

        if (dropVo.Type1 == 2)
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
                    SceneManager.Instance.RandomDropItem(uint.Parse(items[index]), uint.Parse(nums[index]), transform.position);
                }
            }
            else if (dropVo.Type2 == 2)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    int rand = Random.Range(0, 10000);
                    if (rand < int.Parse(rates[i]))
                    {
                        SceneManager.Instance.RandomDropItem(uint.Parse(items[i]), uint.Parse(nums[i]), transform.position);
                    }
                }
            }
        }
        transform.SendMessage("SetBoxStatus", true);
    }
}
