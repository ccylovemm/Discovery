using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public Text dialogueName;
    public TextEx dialogueTxt;
    public GameObject obj;

    private void Start()
    {
        obj.SetActive(false);
    }

    private void OnEnable()
    {
        EventCenter.AddEvent(EventEnum.ShowDialogue , OnShowDialogue);
    }

    private void OnDisable()
    {
        EventCenter.RemoveEvent(EventEnum.ShowDialogue, OnShowDialogue);
        obj.SetActive(false);
    }

    public void OnShowDialogue(EventCenterData data)
    {
        object[] datas = data.data as object[];
        if ((bool)datas[0])
        {
            obj.SetActive(true);
            dialogueName.text = datas[2] as string;
            dialogueTxt.text = datas[3] as string;
            transform.position = CameraManager.Instance.mainCamera.WorldToScreenPoint((Vector3)datas[1]) + new Vector3(0, 130, 0);
        }
        else
        {
            obj.SetActive(false);
        }
    }
}
