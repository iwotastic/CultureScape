using System;
using UnityEngine;

/// <summary>
/// Looks at a target.
/// </summary>
public class LookAt: MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 forward;

    private void Awake()
    {
        if (!target)
        {
            target = Camera.main ? Camera.main.transform : null;
        }
    }

    private void Update()
    {
        if (!target) return;

        var dir = (target.position - transform.position).normalized;
        var rot = Quaternion.LookRotation(dir) * Quaternion.LookRotation(forward);

        transform.rotation = rot;
    }
}
