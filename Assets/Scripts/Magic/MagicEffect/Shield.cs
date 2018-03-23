using System.Collections;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public GameObject effect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Shield" && LayerUtil.IsNotAvailable(collision.gameObject.layer))
        {
            effect.SetActive(false);
        }
    }
}
