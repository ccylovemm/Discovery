using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageAnimation : MonoBehaviour
{
    public List<Sprite> sprites = new List<Sprite>();

    private Image image;
    private int index;
    private int count;

    private void Awake()
    {
        image = GetComponent<Image>();
        count = sprites.Count;
    }

    private void Update()
    {
        if (Time.frameCount % 4 != 0) return;
        image.sprite = sprites[index];
        index++;
        index = index % count;
    }
}
