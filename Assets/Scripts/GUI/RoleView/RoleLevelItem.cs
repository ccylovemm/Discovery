using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleLevelItem : MonoBehaviour
{
    public Text levelDesc;
    public List<GameObject> stars;

    private ActorVo currActorVo;
    private GameObject selectIcon;

    public void SetData(ActorVo actorVo, GameObject select)
    {
        currActorVo = actorVo;
        selectIcon = select;

        uint level = DataManager.userData.actor.level;
        levelDesc.text = LanguageManager.GetText(currActorVo.Desc);
        levelDesc.color = level > currActorVo.Level ? new Color(0.86f , 0.76f , 0.5f , 1.0f) : (level == currActorVo.Level ? new Color(0.99f , 0.76f , 0.06f , 1.0f) : new Color(0.36f , 0.34f , 0.28f , 1.0f));
        for(int i = 0; i < stars.Count; i ++)
        {
            stars[i].SetActive(currActorVo.Level > i);
        }
    }

    public void SelectItem()
    {
        if (currActorVo.Level == DataManager.userData.actor.level)
        {
            selectIcon.transform.parent = transform;
            selectIcon.transform.SetAsLastSibling();
            selectIcon.transform.localPosition = new Vector3(111, -43, 0);
        }
    }
}
