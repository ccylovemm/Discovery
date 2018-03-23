using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public Image hpBar;
    public Image addBar;
    public Image reduceBar;

    private float m_fillCount1 = 0;
    private float m_fillCount2 = 0;
    private float currFillCount;
    private bool add;

    private bool init = true;

    private void Update()
    {
        if (m_fillCount1 == currFillCount && m_fillCount2 == currFillCount) return;
        m_fillCount1 += (add ? 0.01f : -0.01f);
        m_fillCount2 += (add ? 0.04f : -0.04f);
        if (add && m_fillCount1 >= currFillCount || !add && m_fillCount1 <= currFillCount)
        {
            m_fillCount1 = currFillCount;
        }
        if (add && m_fillCount2 >= currFillCount || !add && m_fillCount2 <= currFillCount)
        {
            m_fillCount2 = currFillCount;
        }
        if (add)
        {
            hpBar.fillAmount = m_fillCount1;
            addBar.fillAmount = m_fillCount2;
        }
        else
        {
            reduceBar.fillAmount = m_fillCount1;
            hpBar.fillAmount = m_fillCount2;
        }
    }

    public void SetData(float fillCount)
    {
        if (currFillCount == fillCount) return;
        add = fillCount >= m_fillCount1;
        currFillCount = fillCount;
        if (init)
        {
            init = false;
            hpBar.fillAmount = fillCount;
            addBar.fillAmount = fillCount;
            reduceBar.fillAmount = fillCount;
            m_fillCount1 = fillCount;
            m_fillCount2 = fillCount;
        }
        else
        {
            if (add)
            {
                addBar.gameObject.SetActive(true);
                reduceBar.gameObject.SetActive(false);
            }
            else
            {
                addBar.gameObject.SetActive(false);
                reduceBar.gameObject.SetActive(true);
            }
        }
    }
}
