using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillData
{
    public float cdTime;
    public SkillLevelVo skillVo;

    public SkillData(SkillLevelVo vo)
    {
        skillVo = vo;
    }
}
