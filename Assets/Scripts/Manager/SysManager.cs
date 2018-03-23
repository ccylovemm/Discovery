using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SysManager : Singleton<SysManager>
{
    public Dictionary<uint, ActorObject> sceneObjs = new Dictionary<uint, ActorObject>();

    private void OnEnable()
    {
        EventCenter.AddEvent(EventEnum.SysMovement , OnSysMovement);
    }

    private void OnDisable()
    {
        EventCenter.RemoveEvent(EventEnum.SysMovement, OnSysMovement);
    }

    private void OnSysMovement(EventCenterData data)
    {
        object[] datas = data.data as object[];
        ActorObject actorBase = null;
        if (sceneObjs.TryGetValue((uint)datas[0] , out actorBase))
        {
            actorBase.transform.position = (Vector3)datas[1];
        }
    }
}
