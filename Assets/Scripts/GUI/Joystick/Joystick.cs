using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour
{
	public delegate void TouchDelegate(Vector2 value);
	public event TouchDelegate TouchEvent;

	public delegate void TouchStateDelegate(bool touchPresent);
	public event TouchStateDelegate TouchStateEvent;

    public bool isDynamic = false;
    public GameObject joystick;
    public RectTransform joystickPoint;

    public bool lockJoystick = false;

    private Rect joystickRect;
    private bool touchPresent = false;

    private Vector3 tempPos;

    private void Awake()
    {
        joystickRect = new Rect(0 , 0 , Screen.width * 0.4f , Screen.height * 0.8f);
    }

    public void Lock(bool bol)
    {
        lockJoystick = bol;
        joystick.SetActive(!bol);
    }

    private void Update()
    {
        if (!lockJoystick && isDynamic)
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            if (Input.GetMouseButtonDown(0) && joystickRect.Contains(Input.mousePosition))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;
                touchPresent = true;
                joystick.SetActive(true);
                joystick.transform.position = Input.mousePosition;
                if (TouchStateEvent != null)
                {
                    TouchStateEvent(touchPresent);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                touchPresent = false;
           //     joystick.SetActive(false);
                joystickPoint.anchoredPosition = Vector2.zero;
                if (TouchStateEvent != null)
                {
                    TouchStateEvent(touchPresent);
                }
            }

            if (touchPresent)
            {
                Vector3 pos = Input.mousePosition - joystick.transform.position;
                joystickPoint.anchoredPosition = pos.normalized * Mathf.Min(50, pos.magnitude);
                if (TouchEvent != null)
                {
                    TouchEvent(joystickPoint.anchoredPosition);
                }
            }
#else
            if (Input.touchCount > 0 && !touchPresent && InRange)
            {
                touchPresent = true;
                joystick.SetActive(true);
                joystick.transform.position = TouchPos;
                tempPos = TouchPos;
                if (TouchStateEvent != null)
                {
                    TouchStateEvent(touchPresent);
                }
            }

            if ((Input.touchCount == 0 || !InRange) && touchPresent)
            {
                touchPresent = false;
             //   joystick.SetActive(false);
                joystickPoint.anchoredPosition = Vector2.zero;
                if (TouchStateEvent != null)
                {
                    TouchStateEvent(touchPresent);
                }
            }

            if (touchPresent)
            {
                Vector3 pos = TouchPos - joystick.transform.position;
                joystickPoint.anchoredPosition = pos.normalized * Mathf.Min(50.0f, pos.magnitude);
                if (TouchEvent != null)
                {
                    TouchEvent(joystickPoint.anchoredPosition);
                }
            }
#endif
        }
    }


    /*
    private void Update()
    {
        if (!lockJoystick && isDynamic)
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            if (Input.GetMouseButtonDown(0) && joystickRect.Contains(Input.mousePosition))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;
                touchPresent = true;
                joystick.SetActive(true);
                joystick.transform.position = Input.mousePosition;
                if (TouchStateEvent != null)
                {
                    TouchStateEvent(touchPresent);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                touchPresent = false;
                joystick.SetActive(false);
                joystickPoint.anchoredPosition = Vector2.zero;
                if (TouchStateEvent != null)
                {
                    TouchStateEvent(touchPresent);
                }
            }

            if (touchPresent)
            {
                Vector3 pos = Input.mousePosition - joystick.transform.position;
                joystickPoint.anchoredPosition = pos.normalized * Mathf.Min(50, pos.magnitude);
                if (TouchEvent != null)
                {
                    TouchEvent(joystickPoint.anchoredPosition);
                }
            }
#else
            if (Input.touchCount > 0 && !touchPresent && InRange)
            {
                touchPresent = true;
                joystick.SetActive(true);
                joystick.transform.position = TouchPos;
                tempPos = TouchPos;
                if (TouchStateEvent != null)
                {
                    TouchStateEvent(touchPresent);
                }
            }

            if ((Input.touchCount == 0 || !InRange) && touchPresent)
            {
                touchPresent = false;
                joystick.SetActive(false);
                joystickPoint.anchoredPosition = Vector2.zero;
                if (TouchStateEvent != null)
                {
                    TouchStateEvent(touchPresent);
                }
            }

            if (touchPresent)
            {
                tempPos += DeltaPos * 0.7f;
                Vector3 pos = tempPos - joystick.transform.position;
                tempPos = Mathf.Min(50.0f, pos.magnitude) * pos.normalized + joystick.transform.position;
                joystickPoint.anchoredPosition = pos.normalized * Mathf.Min(50.0f, pos.magnitude);
                if (TouchEvent != null)
                {
                    TouchEvent(joystickPoint.anchoredPosition);
                }
            }
#endif
        }
    }
    */

    public void BeginDrag()
	{
		touchPresent = true;
        if (TouchStateEvent != null)
        {
            TouchStateEvent(touchPresent);
        }
	}

    public void Drag()
    {
        if (touchPresent)
        {
            joystickPoint.anchoredPosition = joystickPoint.anchoredPosition.normalized * Mathf.Min(25.0f, joystickPoint.anchoredPosition.magnitude);
            if (TouchEvent != null)
            {
                TouchEvent(joystickPoint.anchoredPosition.normalized);
            }
        }
        else
        {
            joystickPoint.anchoredPosition = Vector2.zero;
        }
    }

	public void EndDrag()
	{
		touchPresent = false;
        joystickPoint.anchoredPosition = Vector2.zero;
        if (TouchStateEvent != null)
        {
            TouchStateEvent(touchPresent);
        }
    }

    private bool InRange
    {
        get
        {
            int count = Input.touchCount;
            for (int i = 0; i < count; i++)
            {
                if (joystickRect.Contains(Input.GetTouch(i).position) && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                {
                    return true;
                }
            }
            return false;
        }
    }

    private Vector3 TouchPos
    {
        get
        {
            int count = Input.touchCount;
            for (int i = 0; i < count; i++)
            {
                if (joystickRect.Contains(Input.GetTouch(i).position))
                {
                    return Input.GetTouch(i).position;
                }
            }
            return Vector3.zero;
        }
    }

    private Vector3 DeltaPos
    {
        get
        {
            int count = Input.touchCount;
            for (int i = 0; i < count; i++)
            {
                if (joystickRect.Contains(Input.GetTouch(i).position))
                {
                    return Input.GetTouch(i).deltaPosition;
                }
            }
            return Vector3.zero;
        }
    }
}
