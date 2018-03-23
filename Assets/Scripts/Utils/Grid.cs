using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    public bool vertical;
    public int lineCount = 1;
    public bool center = true;
    public float width = 1.0f;
    public float height = 1.0f;

#if UNITY_EDITOR
    private void OnValidate()
    {
        ResetPosition();
    }
#endif

    public void ResetPosition()
    {
        List<Transform> transList = new List<Transform>();
        for (int i = 0; i < transform.childCount; i ++)
        {
            Transform trans = transform.GetChild(i);
            if (trans.gameObject.activeSelf)
            {
                transList.Add(trans);
            }
        }

        int count2 = Mathf.CeilToInt((float)transList.Count / lineCount);
        for (int i = 0; i < transList.Count; i++)
        {
            if (vertical)
            {
                transList[i].localPosition = center ? new Vector3((i % lineCount) * height, (-1 * ((count2 - 1) / 2.0f) + (int)(i / lineCount)) * width, 0) : new Vector3((i % lineCount) * height, -1 * (int)(i / lineCount) * width, 0);
            }
            else
            {
                transList[i].localPosition = center ? new Vector3((-1 * ((count2 - 1) / 2.0f) + (int)(i / lineCount)) * width, -1 * (i % lineCount) * height, 0) : new Vector3((int)(i / lineCount) * width, -1 * (i % lineCount) * height, 0);
            }
        }
    }
}
