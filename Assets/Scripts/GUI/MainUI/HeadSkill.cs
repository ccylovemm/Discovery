using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadSkill : MonoBehaviour
{
    public Grid grid;
    public Transform target;
    public List<Image> skillSprites;

    void Awake()
    {
        Clear();
    }

    void OnEnable()
    {
        EventCenter.AddEvent(EventEnum.UseSkill, OnUseSkill);
        EventCenter.AddEvent(EventEnum.ClearUseSkill, OnClearUseSkill);
    }

    void OnDisable()
    {
        EventCenter.RemoveEvent(EventEnum.UseSkill, OnUseSkill);
        EventCenter.RemoveEvent(EventEnum.ClearUseSkill, OnClearUseSkill);
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 pos = CameraManager.Instance.mainCamera.WorldToScreenPoint(target.position + Vector3.left * 0.01f);
            transform.position = pos;
        }
    }


    void OnUseSkill(EventCenterData data)
    {
        for (int i = 0; i < skillSprites.Count; i ++)
        {
            if (i >= GameData.elements.Count)
            {
                skillSprites[i].gameObject.SetActive(false);
            }
            else
            {
                skillSprites[i].gameObject.SetActive(true);
                ResourceManager.Instance.LoadIcon("Icon_Element_" + GameData.elements[i], icon =>
                {
                    skillSprites[i].sprite = icon;
                });
            }
        }
        grid.ResetPosition();
    }

    void OnClearUseSkill(EventCenterData data)
    {
        Clear();
    }

    void Clear()
    {
        for (int i = 0; i < skillSprites.Count; i++)
        {
            skillSprites[i].gameObject.SetActive(false);
        }
    }
}
