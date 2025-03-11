using System;
using System.Collections.Generic;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

    public static void Enqueue(Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    void Update()
    {
        while (true)
        {
            Action action = null;
            lock (_executionQueue)
            {
                if (_executionQueue.Count > 0)
                    action = _executionQueue.Dequeue();
            }
            if (action == null)
                break;

            action.Invoke();
        }
    }
}
