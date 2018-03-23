using UnityEngine;

public class ModifyPos : MonoBehaviour
{
    public bool combine;
    public bool saveobj;

    [HideInInspector]
    public Rect spriteRect;
    [HideInInspector]
    public Vector3 offsetPos;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        Vector3 position = transform.position;
        Vector3 grid = MapManager.GetGrid(position.x, position.y); ;
        transform.position = MapManager.GetPos((int)grid.x , (int)grid.y);

        SpriteRenderer render = GetComponentInChildren<SpriteRenderer>();
        if (render != null)
        {
            spriteRect = render.sprite.rect;
            offsetPos = render.transform.localPosition;
        }
    }
#endif
}
