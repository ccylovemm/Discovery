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

        if (id > 100) //怪物
        {
            int count = DataManager.userData.GetMonsterDieCount(id);

            ActorCFG.items.Foreach(vo =>
            {
                if (vo.Value.Id == id && count >= vo.Value.LevelUp)
                {
                    if (cfgVo == null || vo.Value.LevelUp > cfgVo.LevelUp)
                    {
                        cfgVo = vo.Value;
                    }
                }
            });
        }
        else //角色
        {
            cfgVo = ActorCFG.items[id + "" + DataManager.userData.actor.level];
        }

        Reset();
    }

    public uint Anger
    {
        get
        {
            return currAnger;
        }
        set
        {
            currAnger = value;
            if (currAnger > cfgVo.RageMax)
            {
                currAnger = cfgVo.RageMax;
            }
            if (currAnger == 0)
            {
                angerFull = false;
            }
            else if (currAnger >= cfgVo.RageMax)
            {
                angerFull = true;
            }
        }
    }

    public void Reset()
    {
        currHp = cfgVo.MaxHp;
        Anger = 0;
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

        FreshDecorations();
    }

    public void FreshDecorations()
    {
        decorations.Clear();
        for (int i = 0; i < DataManager.userData.Decorations.Count; i++)
        {
            uint id = DataManager.userData.Decorations[i];
            decorations.Add(DecorationCFG.items[id + "" + DataManager.userData.GetDecorationLevel(id)]);
        }
    }
}
