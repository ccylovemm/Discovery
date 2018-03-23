using UnityEngine;
using System.Collections;
public class CameraManager : Singleton<CameraManager>
{
    public int cameraWidth;
    public int cameraHeight;
    public Camera mainCamera;

    private bool canCameraMove = true;
    private Transform playerTransform;

    private Transform trans;

    private float boundMinX;
    private float boundMaxX;
    private float boundMinY;
    private float boundMaxY;

    private float leftBound;
    private float rightBound;
    private float bottomBound;
    private float topBound;

    private float camX;
    private float camY;


    public void Init(Transform value)
    {
        playerTransform = value;
        trans = gameObject.transform;
        RefreshCameraBounds();
    }

    void Update()
    {
        if (playerTransform == null) return;
        if (canCameraMove)
        {
            camX = Mathf.Clamp(playerTransform.position.x, -1000, 1000);
            camY = Mathf.Clamp(playerTransform.position.y, -1000, 1000);
            //  camX = Mathf.Clamp(playerTransform.position.x, leftBound, rightBound);
            // camY = Mathf.Clamp(playerTransform.position.y, bottomBound, topBound);
        }
    }

    void LateUpdate()
    {
        if (canCameraMove && trans)
        {
            trans.position = new Vector3(camX, camY, trans.position.z);
        }
    }

    void SetXBounds(float minX, float maxX)
    {
        boundMinX = minX;
        boundMaxX = maxX;
    }

    void SetYBounds(float minY, float maxY)
    {
        boundMinY = minY;
        boundMaxY = maxY;
    }

    void RefreshCameraBounds()
    {
        if (mainCamera == null) return;
        float camVertExtent = mainCamera.orthographicSize;
        float camHorzExtent = mainCamera.aspect * camVertExtent;

        leftBound = boundMinX + camHorzExtent;
        rightBound = boundMaxX - camHorzExtent;
        bottomBound = boundMinY + camVertExtent;
        topBound = boundMaxY - camVertExtent;
    }

    public void SetCameraBounds(float bottomY, float topY, float leftX, float rightX)
    {
        SetXBounds(leftX, rightX);
        SetYBounds(bottomY, topY);
        RefreshCameraBounds();
    }
}
