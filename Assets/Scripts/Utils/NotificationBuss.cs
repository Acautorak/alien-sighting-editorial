using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public static class NotificationBuss
{
    private static readonly Dictionary<string, Action<object>> _eventTable = new Dictionary<string, Action<object>>();


    /// Subscribes to an event with the given event name.
    public static void Subscribe(string eventName, Action<object> listener)
    {
        if (!_eventTable.ContainsKey(eventName))
        {
            _eventTable[eventName] = null;
        }
        _eventTable[eventName] += listener;
    }

    /// Unsubscribes from an event with the given event name.
    public static void Unsubscribe(string eventName, Action<object> listener)
    {
        if (_eventTable.ContainsKey(eventName))
        {
            _eventTable[eventName] -= listener;
        }
    }

    /// Publishes an event to all subscribed listeners
    public static void Publish(string eventName, object parameter = null)
    {
        if(_eventTable.ContainsKey(eventName))
        {
            _eventTable[eventName]?.Invoke(parameter);
        }
    }
}
