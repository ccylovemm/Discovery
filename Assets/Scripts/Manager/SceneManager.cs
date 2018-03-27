using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class SceneManager : Singleton<SceneManager>
{
    public void EnterMap()
    {
        ResourceManager.Instance.LoadScene("resourceassets/map/World1.unity3d", assets =>
        {
            GameData.myData = new ActorData(1);
            CreateActor(GameData.myData , new Vector2(10 , 5) * MapManager.textSize);
            Loading.Instance.FadeDisable(() => { WindowManager.Instance.OpenWindow(WindowKey.MainUI); });
        });
    }

    public void CreateActor(ActorData actorData, Vector2 pos)
    {
        ResourceManager.Instance.LoadAsset("resourceassets/avatar.assetbundle", model =>
        {
            GameObject actor = GameObject.Instantiate((GameObject)model.LoadAsset(System.IO.Path.GetFileNameWithoutExtension(actorData.cfgVo.ResName) + ".prefab"));
            actor.transform.position = new Vector3(pos.x, pos.y, 0);
            if ((ActorType)actorData.cfgVo.Type == ActorType.Player)
            {
                Player actorObject = actor.GetComponent<Player>();
                actorObject.actorData = actorData;
                if (GameData.myData.uniqueId == actorData.uniqueId)
                {
                    GameData.myself = actorObject;
                    EventCenter.DispatchEvent(EventEnum.UpdateMainUIAttackBtn);
                    CameraManager.Instance.Init(actorObject.transform);
                }
            }
            else
            {
                Monster actorObject = actor.GetComponent<Monster>();
                actorObject.actorData = actorData;
                GameData.enemys.Add(actorObject);
            }
        });
    }
}
