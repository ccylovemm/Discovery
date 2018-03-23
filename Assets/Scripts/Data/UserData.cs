using System.Collections.Generic;

[System.Serializable]
public class UserData
{
    private int version;
    private uint actorId = 1;
    private bool isDead = true;
    private bool isSound = true;

    //短线删除
    private uint hp = 0;
    private uint carryId = 0;
    private uint employId = 0;
    private uint employHp = 0;
    private int levelCoin = 0;
    private uint group = 1;
    private uint groupLevel = 1;
    private List<uint> elements = new List<uint>();
    private List<uint> decorations = new List<uint>();
    private Dictionary<uint, int> skillLevels = new Dictionary<uint, int>();
    private Dictionary<SkillElement, string> skills = new Dictionary<SkillElement, string>();

    //长线保存
    private int diamond = 0;
    private int goldCoin = 0;
    private List<uint> unlockActorIds = new List<uint>();
    public Dictionary<uint, int> decorationLevel = new Dictionary<uint, int>();
    private Dictionary<uint, int> monsterDeathCount = new Dictionary<uint, int>();
    private Dictionary<uint, ActorSerializableData> actorData = new Dictionary<uint, ActorSerializableData>();

    public int Version
    {
        get
        {
            return version;
        }
        set
        {
            version = value;
            DataManager.Save();
        }
    }

    public int LevelCoin
    {
        get
        {
            return levelCoin;
        }
        set
        {
            levelCoin = value;
            DataManager.Save();
        }
    }

    public uint Group
    {
        get
        {
            return group;
        }
        set
        {
            group = value;
            DataManager.Save();
        }
    }

    public uint GroupLevel
    {
        get
        {
            return groupLevel;
        }
        set
        {
            groupLevel = value;
            DataManager.Save();
        }
    }

    public uint ActorId
    {
        get
        {
            return actorId;
        }
        set
        {
            actorId = value;
            DataManager.Save();
        }
    }

    public bool IsDead
    {
        get
        {
            return isDead;
        }
        set
        {
            isDead = value;
            DataManager.Save();
        }
    }

    public bool IsSound
    {
        get
        {
            return isSound;
        }
        set
        {
            isSound = value;
            DataManager.Save();
        }
    }

    public uint EmployId
    {
        get
        {
            return employId;
        }
        set
        {
            employId = value;
            DataManager.Save();
        }
    }

    public uint EmployHp
    {
        get
        {
            return employHp;
        }
        set
        {
            employHp = value;
            DataManager.Save();
        }
    }

    public uint Hp
    {
        get
        {
            return hp;
        }
        set
        {
            hp = value;
            DataManager.Save();
        }
    }

    public uint CarryId
    {
        get
        {
            return carryId;
        }
        set
        {
            carryId = value;
            DataManager.Save();
        }
    }

    public int GoldCoin
    {
        get
        {
            return goldCoin;
        }
        set
        {
            goldCoin = value;
            DataManager.Save();
            EventCenter.DispatchEvent(EventEnum.UpdateMoney);
        }
    }

    public int Diamond
    {
        get
        {
            return diamond;
        }
        set
        {
            diamond = value;
            DataManager.Save();
            EventCenter.DispatchEvent(EventEnum.UpdateMoney);
        }
    }

    public void SetDecorationLevel(uint id, int level)
    {
        if (decorationLevel.ContainsKey(id))
        {
            decorationLevel[id] = level;
        }
        else
        {
            decorationLevel.Add(id, level);
        }
        DataManager.Save();
    }

    public int GetDecorationLevel(uint id)
    {
        if (decorationLevel.ContainsKey(id))
        {
            return decorationLevel[id];
        }
        return 1;
    }

    public void MonsterDie(uint id)
    {
        if (monsterDeathCount.ContainsKey(id))
        {
            monsterDeathCount[id] += 1;
        }
        else
        {
            monsterDeathCount.Add(id, 1);
        }
        DataManager.Save();
    }

    public int GetMonsterDieCount(uint id)
    {
        if (monsterDeathCount.ContainsKey(id))
        {
            return monsterDeathCount[id];
        }
        return 0;
    }

    public void UnlockActor(uint id)
    {
        if (unlockActorIds.Contains(id))
        {
            return;
        }
        else
        {
            unlockActorIds.Add(id);
        }
        DataManager.Save();
    }

    public bool HasUnlockActor(uint id)
    {
        return unlockActorIds.Contains(id);
    }

    public Dictionary<SkillElement, string> GetSkill
    {
        get
        {
            return skills;
        }
    }

    public void SetSkill(SkillElement skillElement , string skill)
    {
        if (skills.ContainsKey(skillElement))
        {
            skills[skillElement] = skill;
        }
        else
        {
            skills.Add(skillElement, skill);
        }
        DataManager.Save();
    }

    public int GetSkillLevel(uint id)
    {
        if (skillLevels.ContainsKey(id))
        {
            return skillLevels[id];
        }
        return 0;//未获得该技能
    }

    public void SetSkillLevel(uint id, int level)
    {
        if (skillLevels.ContainsKey(id))
        {
            skillLevels[id] = level;
        }
        else
        {
            skillLevels.Add(id, level);
        }
        DataManager.Save();
    }

    public List<uint> Decorations
    {
        get
        {
            return decorations;
        }
    }

    public void SetDecoration(uint id)
    {
        if (!decorations.Contains(id))
        {
           decorations.Add(id);
        }
        DataManager.Save();
    }

    public List<uint> Elements
    {
        get
        {
            return elements;
        }
        set
        {
            elements = value;
            SkillManager.LoadAllSkill();
            DataManager.Save();
        }
    }

    public ActorSerializableData actor
    {
        get
        {
            return actorData[actorId];
        }
    }

    public void UpgradeActorLevel()
    {
        actor.level += 1;
        DataManager.Save();
    }

    public void FreshData(bool fresh = false)
    {
        UnlockActor(actorId);
        if (!actorData.ContainsKey(actorId))
        {
            actorData.Add(actorId, new ActorSerializableData());
        }
        GameData.FreshData(fresh);
    }

    public void Clear()
    {
        hp = GameData.myData.cfgVo.MaxHp;
        isDead = false;
        carryId = 0;
        employId = 0;
        employHp = 0;
        levelCoin = 0;
        group = 1;
        groupLevel = 1;
        elements = GameData.myData.elements;
        decorations.Clear();
        skills.Clear();
        skillLevels.Clear();
        DataManager.Save();
    }
}

[System.Serializable]
public class ActorSerializableData
{
    public uint level = 1;
}
