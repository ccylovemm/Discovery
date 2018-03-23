using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerHeadBar : HeadBar
{
    public Image energyBar1;
    public Image energyBar2;

    public override void UpdateEnergy(uint curr, uint max , bool isFull)
    {
        energyBar1.fillAmount = (float)curr / (float)max;
        energyBar2.fillAmount = (float)curr / (float)max;
        energyBar1.gameObject.SetActive(!isFull);
        energyBar2.gameObject.SetActive(isFull);
    }
}
