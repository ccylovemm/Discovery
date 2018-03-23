using System.Collections;
using System.Collections.Generic;

public class EventCenter
{
    private static Dictionary<string, DelegateEvent> eventTypeListeners = new Dictionary<string, DelegateEvent>();

    public static void AddEvent(string type, DelegateEvent.EventHandler listenerFunc)
    {
        DelegateEvent delegateEvent;
        if (eventTypeListeners.ContainsKey(type))
        {
            delegateEvent = eventTypeListeners[type];
        }
        else
        {
            delegateEvent = new DelegateEvent();
            eventTypeListeners[type] = delegateEvent;
        }
        delegateEvent.AddListener(listenerFunc);
    }

    public static void RemoveEvent(string type, DelegateEvent.EventHandler listenerFunc)
    {
        if (listenerFunc == null)
        {
            return;
        }
        if (!eventTypeListeners.ContainsKey(type))
        {
            return;
        }
        DelegateEvent delegateEvent = eventTypeListeners[type];
        delegateEvent.RemoveListener(listenerFunc);
    }

    public static void DispatchEvent(string type, object data = null)
    {
        if (!eventTypeListeners.ContainsKey(type))
        {
            return;
        }

        EventCenterData eventData = new EventCenterData();
        eventData.type = type;
        eventData.data = data;

        DelegateEvent delegateEvent = eventTypeListeners[type];
        delegateEvent.Handle(eventData);
    }

}

public class DelegateEvent
{
    public delegate void EventHandler(EventCenterData data);
    public event EventHandler eventHandle;

    public void Handle(EventCenterData data)
    {
        if (eventHandle != null)
            eventHandle(data);
    }

    public void RemoveListener(EventHandler removeHandle)
    {
        if (eventHandle != null)
            eventHandle -= removeHandle;
    }

    public void AddListener(EventHandler addHandle)
    {
        eventHandle += addHandle;
    }
}

public class EventCenterData
{
    public string type;
    public object data;
}