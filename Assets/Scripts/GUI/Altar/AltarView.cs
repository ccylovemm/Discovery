using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarView : MonoBehaviour
{
    public List<AltarItem> altars = new List<AltarItem>();

    private void SetParameters(object[] args)
    {
        List<SkillVo> skillList = args[0] as List<SkillVo>;
        for(int i = 0; i < skillList.Count; i ++)
        {
            altars[i].SetData(skillList[i]);
        }
    }

    public void Close()
    {
        WindowManager.Instance.CloseWindow(WindowKey.AltarView);
    }
}
