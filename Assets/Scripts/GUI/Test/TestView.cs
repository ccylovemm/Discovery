using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestView : MonoBehaviour
{
    public Text text;

    public List<Image> elements = new List<Image>();

    public RectTransform testGridTransform;
    public Grid testGrid;
    public GameObject testItemPrefab;
    public List<TestItem> testList = new List<TestItem>();

    public RectTransform testRecodrTransform;
    public Grid testRecodrGrid;
    public GameObject testRecordItemPrefab;
    public List<TestRecordItem> testRecordList = new List<TestRecordItem>();

    private List<SkillVo> selectSkillVo = new List<SkillVo>();

    private uint count = 0;
    private uint currSelectElement = 0;

    public void SelectElement1()
    {
        SelectElement(GameData.myData.elements[0]);
    }

    public void SelectElement2()
    {
        SelectElement(GameData.myData.elements[1]);
    }

    public void SelectElement3()
    {
        SelectElement(GameData.myData.elements[2]);
    }

    public void SelectElement4()
    {
        SelectElement(GameData.myData.elements[3]);
    }

    private void OnEnable()
    {
        text.text = count.ToString();
        SelectElement(GameData.myData.elements[0]);
        UpdateElement();
        UpdateRecord();
    }

    void UpdateElement()
    {
        for (int i = 0; i < elements.Count; i ++)
        {
            if (i >= GameData.myData.elements.Count)
            {
                elements[i].gameObject.SetActive(false);
            }
            else
            {
                elements[i].gameObject.SetActive(true);
                ResourceManager.Instance.LoadIcon("Icon_Element_" + GameData.myData.elements[i], icon =>
                {
                    elements[i].sprite = icon;
                });
            }
        }
    }

    public void SelectElement(uint id)
    {
        currSelectElement = id;

        List<SkillVo> skillList1 = new List<SkillVo>();
        List<SkillVo> skillList2 = new List<SkillVo>();

        SkillCFG.items.Foreach(vo =>
        {
            string[] elements = vo.Key.Split(',');
            List<uint> elements_ = new List<uint>();
            for (int i = 0; i < elements.Length; i++)
            {
                elements_.Add(uint.Parse(elements[i]));
            }
            if (elements_.Contains(id))
            {
                bool has = true;
                for (int i = 0; i < elements_.Count; i++)
                {
                    if (!GameData.myData.elements.Contains(elements_[i]) || !SkillLevelCFG.items.ContainsKey(vo.Value.SkillId + "" + (DataManager.userData.GetSkillLevel(vo.Value.SkillId) + 1)))
                    {
                        has = false;
                        break;
                    }
                }
                if (has)
                {
                    if (DataManager.userData.GetSkillLevel(vo.Value.SkillId) > 0)
                    {
                        skillList1.Add(vo.Value);
                    }
                    else
                    {
                        skillList2.Add(vo.Value);
                    }
                }
            }
        });

        List<SkillVo> skillList3 = new List<SkillVo>();

        for (int i = 0; i < 3; i ++)
        {
            if (skillList1.Count == 0) break;
            int index = Random.Range(0 , skillList1.Count);
            skillList3.Add(skillList1[index]);
            skillList1.RemoveAt(index);
        }

        if (skillList2.Count > 0)
        {
            if (Random.Range(0, 10000) < 500)
            {
                int index = Random.Range(0, skillList2.Count);
                skillList3.Add(skillList2[index]);
            }
        }
        if (skillList3.Count < 4 && skillList1.Count > 0)
        {
            int index = Random.Range(0, skillList1.Count);
            skillList3.Add(skillList1[index]);
        }

        for (int i = 0; i < testList.Count; i++)
        {
            testList[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < skillList3.Count; i++)
        {
            if (i >= testList.Count)
            {
                testList.Add(GameObject.Instantiate(testItemPrefab, testGrid.transform).GetComponent<TestItem>());
            }
            testList[i].gameObject.SetActive(true);
            testList[i].SetData(skillList3[i]);
        }
        testGridTransform.sizeDelta = new Vector2(1, Mathf.CeilToInt((float)skillList3.Count / testGrid.lineCount) * testGrid.width);
        testGrid.ResetPosition();
    }

    public void SelectItem(SkillVo skillVo)
    {
        if(!selectSkillVo.Contains(skillVo))
        {
            selectSkillVo.Add(skillVo);
        }

        count++;

        text.text = count.ToString();

        DataManager.userData.SetSkillLevel(skillVo.SkillId , DataManager.userData.GetSkillLevel(skillVo.SkillId) + 1);
        UpdateRecord();
        SelectElement(currSelectElement);
    }

    private void UpdateRecord()
    {
        for (int i = 0; i < testRecordList.Count; i++)
        {
            testRecordList[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < selectSkillVo.Count; i++)
        {
            if (i >= testRecordList.Count)
            {
                testRecordList.Add(GameObject.Instantiate(testRecordItemPrefab, testRecodrGrid.transform).GetComponent<TestRecordItem>());
            }
            testRecordList[i].gameObject.SetActive(true);
            testRecordList[i].SetData(selectSkillVo[i]);
        }
        testRecodrTransform.sizeDelta = new Vector2(1, Mathf.CeilToInt((float)selectSkillVo.Count / testRecodrGrid.lineCount) * testRecodrGrid.width);
        testRecodrGrid.ResetPosition();
    }

    public void Close()
    {
        WindowManager.Instance.CloseWindow(WindowKey.TestView);
    }
}
