using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ActorData
{
    public uint uniqueId;

    public uint cfgId;
    public uint currHp;
    public ActorVo cfgVo;
    public List<uint> buffList = new List<uint>();
    public List<SkillData> skills = new List<SkillData>();

    public ActorData(uint id)
    {
        cfgId = id;
        cfgVo = ActorCFG.items[id.ToString()];
        uniqueId = (uint)Random.Range(1, 1000000);

        currHp = cfgVo.MaxHp;
        string[] s = cfgVo.Skills.Split(',');
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == "") continue;
            skills.Add(new SkillData(SkillLevelCFG.items[s[i].Replace("_", "")]));
        }
    }
}
