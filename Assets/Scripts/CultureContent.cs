using System;
using UnityEngine;
using UnityEngine.Events;

public class CultureContent : MonoBehaviour
{
    public UnityEvent onAppear;
    public UnityEvent onDisappear;

    private void OnEnable()
    {
        onAppear.Invoke();
    }

    private void OnDisable()
    {
        onDisappear.Invoke();
    }
}
