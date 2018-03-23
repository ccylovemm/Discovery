using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectAvatarView : MonoBehaviour
{
    public Image npcIcon;
    public Text npcName;
    public Text npcDialog;
    public Text actorDesc;
    public Text elementTxt;
    public Text skillTxt;
    public Text skillDesc;
    public Text actorProfieText;
    public Text actorProfieDesc;
    public Text selectBtnTxt;
    public Text employBtnTxt;
    public List<Image> elements = new List<Image>();
    public Text employCostNum;
    public Text unlockCostNum;
    public Image unlockCostIcon;
    public Button unlockBtn;
    public GameObject lockIcon;

    private NpcVo npcVo;
    private ActorVo selectActorVo;

    private void SetParameters(object[] args)
    {
        npcVo = NpcCFG.items[(string)args[0]];

        uint id = (uint)args[1];
        int count = DataManager.userData.GetMonsterDieCount(id);
        ActorCFG.items.Foreach(vo =>
        {
            if (vo.Value.Id == id && count >= selectActorVo.LevelUp)
            {
                if (selectActorVo == null || vo.Value.LevelUp > selectActorVo.LevelUp)
                {
                    selectActorVo = vo.Value;
                }
            }
        });

        UpdateData();
    }

    private void OnEnable()
    {
        elementTxt.text = LanguageManager.GetText("210010");
        skillTxt.text = LanguageManager.GetText("210011");
        actorProfieText.text = LanguageManager.GetText("210013");
        employBtnTxt.text = LanguageManager.GetText("210028");
    }

    private void UpdateData()
    {
        employCostNum.text = selectActorVo.EmployCoin.ToString();
        if (DataManager.userData.HasUnlockActor(selectActorVo.Id))
        {
            lockIcon.SetActive(false);
            unlockCostNum.gameObject.SetActive(false);
            unlockCostIcon.gameObject.SetActive(false);
            selectBtnTxt.text = LanguageManager.GetText("210027");
            if (selectActorVo.Id == GameData.myData.cfgId)
            {
                unlockBtn.image.color = Color.gray;
                unlockBtn.enabled = false;
            }
            else
            {
                unlockBtn.image.color = Color.white;
                unlockBtn.enabled = true;
            }
        }
        else
        {
            lockIcon.SetActive(true);
            unlockBtn.image.color = Color.white;
            unlockBtn.enabled = true;
            unlockCostNum.gameObject.SetActive(true);
            unlockCostIcon.gameObject.SetActive(true);
            selectBtnTxt.text = LanguageManager.GetText("210052");
            unlockCostNum.text = selectActorVo.UnlockValue;
            ResourceManager.Instance.LoadIcon(ItemCFG.items[selectActorVo.UnlockType.ToString()].ItemIcon, icon =>
            {
                unlockCostIcon.sprite = icon;
                unlockCostIcon.gameObject.SetActive(true);
            });
        }
        npcName.text = LanguageManager.GetText(npcVo.Name);
        npcDialog.text = LanguageManager.GetText(npcVo.Dialogue);
        actorDesc.text = LanguageManager.GetText(selectActorVo.Desc);
        actorProfieDesc.text = LanguageManager.GetText(selectActorVo.Characteristic);

        string[] s = selectActorVo.Elements.Split(',');
        for (int i = 0; i < elements.Count; i ++)
        {
            if (i >= s.Length)
            {
                elements[i].gameObject.SetActive(false);
            }
            else
            {
                ResourceManager.Instance.LoadIcon("Skill" + s[i], icon =>
                {
                    elements[i].sprite = icon;
                    elements[i].gameObject.SetActive(true);
                });
            }
        }
        string str = "";
        s = selectActorVo.Skills.Split(',');
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == "") continue;
            str += (str== "" ? "" : ",") + LanguageManager.GetText(SkillLevelCFG.items[s[i].Replace("_" , "")].Name);
        }
        skillDesc.text = str;
    }

    public void SelectAvatar()
    {
        if (DataManager.userData.HasUnlockActor(selectActorVo.Id))
        {
            DataManager.userData.Clear();
            DataManager.userData.ActorId = selectActorVo.Id;
            DataManager.userData.FreshData(true);
            EventCenter.DispatchEvent(EventEnum.UpdateElement);
            EventCenter.DispatchEvent(EventEnum.UpdateAvatar);
            Close();
        }
        else
        {
            if (ItemUtil.GetItemNum(selectActorVo.UnlockType) >= uint.Parse(selectActorVo.UnlockValue))
            {
                ItemUtil.CostItem(selectActorVo.UnlockType , int.Parse(selectActorVo.UnlockValue));
                DataManager.userData.UnlockActor(selectActorVo.Id);
                EventCenter.DispatchEvent(EventEnum.ShowMsg, LanguageManager.GetText("210053"));
                UpdateData();
            }
            else
            {
                EventCenter.DispatchEvent(EventEnum.ShowMsg , LanguageManager.GetText(ItemCFG.items[selectActorVo.UnlockType.ToString()].Name.ToString()) + LanguageManager.GetText("210050"));
            }
        }
            
    }

    public void EmployAvatar()
    {
        if (DataManager.userData.EmployId != 0)
        {
            EventCenter.DispatchEvent(EventEnum.ShowMsg, LanguageManager.GetText("210055"));
        }
        else
        {
            if (DataManager.userData.GoldCoin >= selectActorVo.EmployCoin)
            {
                Vector2 pos = MapManager.FindPosByRange(GameData.myself.transform.position, 2 * MapManager.textSize, MapManager.textSize);
                DataManager.userData.EmployId = selectActorVo.Id;
                DataManager.userData.EmployHp = selectActorVo.MaxHp;
                EventCenter.DispatchEvent(EventEnum.ShowMsg, LanguageManager.GetText("210054"));
                DataManager.userData.GoldCoin -= (int)selectActorVo.EmployCoin;
                EventCenter.DispatchEvent(EventEnum.UpdateAvatar);
                Close();
            }
            else
            {
                EventCenter.DispatchEvent(EventEnum.ShowMsg, LanguageManager.GetText("210046"));
            }
        }
    }

    public void Close()
    {
        WindowManager.Instance.CloseWindow(WindowKey.SelectAvatarView);
    }
}
