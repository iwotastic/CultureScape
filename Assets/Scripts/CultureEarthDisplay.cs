using System;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

public class CultureEarthDisplay : MonoBehaviour
{
    public UnityEvent OnCultureSelect;

    private void Awake()
    {
        BindInteractableEvents();
    }

    private void BindInteractableEvents()
    {
        var eventWrappers = GetComponentsInChildren<InteractableUnityEventWrapper>();
        foreach (var wrapper in eventWrappers)
        {
            wrapper.WhenSelect.AddListener(OnCultureSelect.Invoke);
        }
    }
    
}
