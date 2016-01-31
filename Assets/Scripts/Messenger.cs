using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void Callback();
public delegate void Callback<T>(T arg);

static public class Messenger
{
    private static Dictionary<string, Callback> eventTable = new Dictionary<string, Callback>();

    static public void AddListener(string eventType, Callback handler)
    {
        if (!eventTable.ContainsKey(eventType))
        {
            eventTable.Add(eventType, null);
        }

        eventTable[eventType] += handler;
    }

    static public void RemoveListener(string eventType, Callback handler)
    {
        if (eventTable.ContainsKey(eventType))
        {
            eventTable[eventType] -= handler;

            if (eventTable[eventType] == null)
            {
                eventTable.Remove(eventType);
            }
        }
    }

    static public void Invoke(string eventType)
    {
        if (eventTable.ContainsKey(eventType))
        {
            Callback func = eventTable[eventType];

            if (func != null)
            {
                func();
            }
        }
    }
}

static public class Messenger<T>
{
    private static Dictionary<string, Callback<T>> eventTable = new Dictionary<string, Callback<T>>();

    static public void AddListener(string eventType, Callback<T> handler)
    {
        if (!eventTable.ContainsKey(eventType))
        {
            eventTable.Add(eventType, null);
        }

        eventTable[eventType] += handler;
    }

    static public void RemoveListener(string eventType, Callback<T> handler)
    {
        if (eventTable.ContainsKey(eventType))
        {
            eventTable[eventType] -= handler;

            if (eventTable[eventType] == null)
            {
                eventTable.Remove(eventType);
            }
        }
    }

    static public void Invoke(string eventType, T arg)
    {
        if (eventTable.ContainsKey(eventType))
        {
            Callback<T> func = eventTable[eventType];

            if (func != null)
            {
                func(arg);
            }
        }
    }
}
