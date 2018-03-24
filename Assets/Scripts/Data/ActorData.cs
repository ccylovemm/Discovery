using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ActorData
{
    public uint uniqueId;

    public uint cfgId;
    public uint currHp;
    public bool angerFull = false;
    public uint currAnger;//怒气值
    public float addAngePercent = 1.0f;
    public uint currShield;//护盾
    public ActorVo cfgVo;
    public List<uint> buffList = new List<uint>();
    public List<uint> elements = new List<uint>();
    public List<TalentVo> talents = new List<TalentVo>();
    public List<SkillData> skills = new List<SkillData>();
    public List<DecorationVo> decorations = new List<DecorationVo>();

    public ActorData(uint id)
    {
        cfgId = id;
        uniqueId = (uint)Random.Range(1, 1000000);

        cfgVo =  ActorCFG.items[id.ToString()];

        Reset();
    }


    public void Reset()
    {
        currHp = cfgVo.MaxHp;
        skills.Clear();
        elements.Clear();
        string[] s = cfgVo.Skills.Split(',');
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == "") continue;
            skills.Add(new SkillData(SkillLevelCFG.items[s[i].Replace("_", "")]));
        }

        s = cfgVo.Talent.Split(',');
        for(int i = 0; i < s.Length; i ++)
        {
            if (string.IsNullOrEmpty(s[i]) || s[i] == "0") continue;
            TalentVo talentVo = TalentCFG.items[s[i]];
            if((TalentType)talentVo.Type == TalentType.AngerAddSpeed)
            {
                addAngePercent = talentVo.ParamValue;
            }
            talents.Add(talentVo);
        }

        if (!string.IsNullOrEmpty(cfgVo.Elements))
        {
            s = cfgVo.Elements.Split(',');
            for (int i = 0; i < s.Length; i++)
            {
                elements.Add(uint.Parse(s[i]));
            }
        }
    }
}
