using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager
{
    static public void AddElement(uint element)
    {
        if (GameData.elements.Contains(element)) return;

        GameData.myself.currSkillData = null;
        GameData.elements.Add(element);

        foreach (var elementConflict in ElementConflictCFG.items)
        {
            string[] ids = elementConflict.Id.Split(',');
            bool match = true;
            for (int i = 0; i < ids.Length; i++)
            {
                if (GameData.elements.IndexOf(uint.Parse(ids[i])) == -1)
                {
                    match = false;
                    break;
                }
            }
            if (match)
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    DeleteElement(uint.Parse(ids[i]));
                }
                break;
            }
        }


        GameData.myself.currSkillData = null;
        foreach (var skillVo in SkillCFG.items)
        {
            string[] ids = skillVo.Key.Split(',');
            if (ids.Length == GameData.elements.Count)
            {
                bool match = true;
                for (int i = 0; i < ids.Length; i++)
                {
                    if (!HasElement(uint.Parse(ids[i])))
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    uint skillId = skillVo.Value.SkillId;
                    SkillVo skill = null;
                    DataManager.userData.GetSkill.Foreach(vo => { if (vo.Value == skillVo.Value.Command) { skill = skillVo.Value; } });
                    if (skill != null)
                    {
                        GameData.myself.currSkillData = new SkillData(SkillLevelCFG.items[skillId + "" + 1]);
                    }
                    else
                    {
                        int level = DataManager.userData.GetSkillLevel(skillId);
                        if (level > 0)
                        {
                            GameData.myself.currSkillData = new SkillData(SkillLevelCFG.items[skillId + "" + level]);
                        }
                    }
                    break;
                }
            }
        }
        EventCenter.DispatchEvent(EventEnum.UpdateMainUIAttackBtn);
    }

    static private bool HasElement(uint id)
    {
        for (int i = 0; i < GameData.elements.Count; i++)
        {
            if (GameData.elements[i] == id)
            {
                return true;
            }
        }
        return false;
    }


    static public void Instantiate(ActorObject caster , SkillLevelVo skillVo)
    {
        caster.hasCreateMagic = true;
        ResourceManager.Instance.LoadAsset("resourceassets/prefabs/magic/" + skillVo.ResName + ".assetbundle", skill =>
        {
            if (GameData.myself != caster || caster.attackState)
            {
                MagicBase magicBase = GameObject.Instantiate((GameObject)skill.LoadAsset(skillVo.ResName + ".prefab")).GetComponent<MagicBase>();
                caster.magicBase = magicBase;
                magicBase.caster = caster;
                magicBase.skillVo = skillVo;
            }
        });
    }

    static public void LoadAllSkill()
    {
        List<uint> skills = new List<uint>();
        foreach (var skill in SkillCFG.items)
        {
            string[] elements = skill.Value.Command.Split(',');
            bool has = true;
            for (int i = 0; i < elements.Length; i ++)
            {
                if (!GameData.myData.elements.Contains(uint.Parse(elements[i])))
                {
                    has = false;
                    break;
                }
            }
            if(has)
            {
                skills.Add(skill.Value.SkillId);
            }
        }
        for (int i = 0; i < skills.Count; i ++)
        {
            int level = DataManager.userData.GetSkillLevel(skills[i]);
            if (level > 0)
            {
                ResourceManager.Instance.LoadAsset("resourceassets/prefabs/magic/" + SkillLevelCFG.items[skills[i] + "" + level].ResName + ".assetbundle" , null);
            }
        }
    }

    static private void DeleteElement(uint id)
    {
        for (int i = 0; i < GameData.elements.Count; i++)
        {
            if (GameData.elements[i] == id)
            {
                GameData.elements.RemoveAt(i);
                break;
            }
        }
    }

    static public void FreshSkillLevel()
    {
        SkillCFG.items.Foreach(vo =>
        {
            int level = DataManager.userData.GetSkillLevel(vo.Value.SkillId);
            if ((SkillElement)vo.Value.ComboType == SkillElement.OneElement)
            {
                DataManager.userData.SetSkillLevel(vo.Value.SkillId, level == 0 ? 1 : level);
            }
            else
            {
                DataManager.userData.SetSkillLevel(vo.Value.SkillId, level);
            }
        });
    }
}
