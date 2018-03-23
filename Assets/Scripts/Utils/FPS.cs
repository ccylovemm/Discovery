using UnityEngine;

public class FPS : MonoBehaviour
{
    public float UpdateInterval = 0.5F;
    private float _lastInterval;
    private int _frames = 0;
    private float _fps;

    void Start()
    {
        UpdateInterval = Time.realtimeSinceStartup;
        _frames = 0;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(100, 100, 200, 200), "FPS:" + _fps.ToString("f2"));
    }

    void Update()
    {
        ++_frames;
        if (Time.realtimeSinceStartup > _lastInterval + UpdateInterval)
        {
            _fps = _frames / (Time.realtimeSinceStartup - _lastInterval);
            _frames = 0;
            _lastInterval = Time.realtimeSinceStartup;
        }
    }
}
