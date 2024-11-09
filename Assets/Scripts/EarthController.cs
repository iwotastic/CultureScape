using System;
using UnityEngine;

public class EarthController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private CultureEarthDisplay[] cultures;

    private void Awake()
    {
        ConfigureCultures();
    }

    private void ConfigureCultures()
    {
        foreach (var culture in cultures)
        {
            culture.OnCultureSelect.AddListener(() => OnCultureSelect(culture));
        }
    }

    private void OnCultureSelect(CultureEarthDisplay display)
    {
        animator.SetBool("Visible", false);
    }
}
