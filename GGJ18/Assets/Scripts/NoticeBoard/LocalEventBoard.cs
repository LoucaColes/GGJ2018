using UnityEngine;
using System;
using System.Collections.Generic;

public class LocalEventBoard : MonoBehaviour
{
    public delegate void EventTrigger(object data = null);

    [Serializable]
    public class EventList
    {
        public EventTrigger m_eventTrigger;
        public string m_eventName;
    }

    [Serializable]
    public class EventQueue
    {
        public string m_eventName;
        public object m_data;
    }

    public bool m_useDelayedEvents = true;

    public List<EventList> m_eventList = new List<EventList>();
    public List<EventQueue> m_eventBuffer = new List<EventQueue>();
    public List<EventQueue> m_tempBuffer = new List<EventQueue>();

    public void Initialise(List<string> _events, bool _useDelayedEvents)
    {
        m_eventList.Clear();

        foreach(string eventName in _events)
        {
            EventList tempList = new EventList();
            tempList.m_eventName = eventName;
            tempList.m_eventTrigger = null;

            m_eventList.Add(tempList);
        }

        m_useDelayedEvents = _useDelayedEvents;
    }

    void Update()
    {
        if (m_useDelayedEvents)
        {
            if (m_tempBuffer != null)
            {
                foreach (EventQueue events in m_tempBuffer)
                {
                    m_eventBuffer.Add(events);
                }

                m_tempBuffer.Clear();
            }
        }
    }

    void LateUpdate()
    {
        if (m_useDelayedEvents)
        {
            BufferHandler();
        }
    }

    void BufferHandler()
    {
        foreach (EventQueue iterEvent in m_eventBuffer)
        {
            TriggerEvent(iterEvent);
        }

        if (m_eventBuffer != null)
        {
            m_eventBuffer.Clear();
        }
    }

    /// <summary>Calls all functions subscribe to this event, using the EventQueue struct </summary>
    void TriggerEvent(EventQueue _queue)
    {
        int current = GetEventIndex(_queue.m_eventName);
        if (current != -1)
        {
            if (m_eventList[current].m_eventTrigger != null)
            {
                if (_queue.m_data == null)
                {
                    m_eventList[current].m_eventTrigger();
                }
                else
                {
                    m_eventList[current].m_eventTrigger(_queue.m_data);
                }
            }
        }
    }

    int GetEventIndex(string _eventName)
    {
        for (int iter = 0; iter < m_eventList.Count; iter++)
        {
            if (m_eventList[iter].m_eventName == _eventName)
            {
                return iter;
            }
        }

        return -1;
    }

    void SetEventList(string _eventName, EventTrigger _trigger, bool _add)
    {
        for (int iter = 0; iter <= m_eventList.Count - 1; iter++)
        {
            if (m_eventList[iter].m_eventName == _eventName)
            {
                if (_add)
                {
                    m_eventList[iter].m_eventTrigger += _trigger;
                }
                else
                {
                    m_eventList[iter].m_eventTrigger -= _trigger;
                }
            }
        }
    }

    public void CreateEvent(string _eventName)
    {
        EventList list = new EventList();
        list.m_eventName = _eventName;
        list.m_eventTrigger = null;
        m_eventList.Add(list);
    }

    public void AddDelayEvent(string _eventName, object _data = null)
    {
        if (m_useDelayedEvents)
        {
            EventQueue queue = new EventQueue();
            queue.m_eventName = _eventName;
            queue.m_data = _data;
            m_tempBuffer.Add(queue);
        }
    }

    public void AddRapidEvent(string _eventName, object _data = null)
    {
        EventQueue queue = new EventQueue();
        queue.m_eventName = _eventName;
        queue.m_data = _data;
        TriggerEvent(queue);
    }

    /// <summary>Adds function that requires a data object to call list when this event is triggered</summary>
    public void SubscribeToEvent(string _event, EventTrigger _trigger)
    {
        SetEventList(_event, _trigger, true);
    }

    /// <summary>Removes function that requires a data object from call list of this event </summary>
    public void UnsubscribeToEvent(string _event, EventTrigger _trigger)
    {
        SetEventList(_event, _trigger, false);
    }
}