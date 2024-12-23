using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public static class NotificationBuss
{
    private static readonly Dictionary<EventNames, Action<object>> _eventTable = new Dictionary<EventNames, Action<object>>();


    /// Subscribes to an event with the given event name.
    public static void Subscribe(EventNames eventName, Action<object> listener)
    {
        if (_eventTable.TryGetValue(eventName, out Action<object> thisEvent))
        {
            thisEvent += listener;
            _eventTable[eventName] = thisEvent;
        }
        else
        {
            thisEvent += listener;
            _eventTable.Add(eventName, thisEvent);
        }
    }

    /// Unsubscribes from an event with the given event name.
    public static void Unsubscribe(EventNames eventName, Action<object> listener)
    {
        if (_eventTable.TryGetValue(eventName, out Action<object> thisEvent))
        {
            thisEvent -= listener;
            _eventTable[eventName] = thisEvent;
        }
    }

    /// Publishes an event to all subscribed listeners
    public static void Publish(EventNames eventName, object parameter = null)
    {
        if (_eventTable.TryGetValue(eventName, out Action<object> thisEvent))
        {
            thisEvent?.Invoke(parameter);
        }
    }
}

public enum EventNames
{
    OnCaseButtonClicked,
    OnCasePublished,
    OnCaseDenied,
    OnCaseDropped,

}
