using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventListener : MonoBehaviour
{
    public UnityEvent<string> OnEvent = new UnityEvent<string>();

    public void CallEvent(string eventName)
    {
        OnEvent?.Invoke(eventName);
    }
}
