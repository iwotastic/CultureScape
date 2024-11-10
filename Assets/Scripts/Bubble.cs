using System;
using Cysharp.Threading.Tasks;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

public class Bubble : MonoBehaviour
{
    [SerializeField, Header("Transforms")] private Transform globeParent;
    [SerializeField] private Transform floatParent;
    [SerializeField] private Transform contentsParent;
    [SerializeField] private Transform bubble;
    [SerializeField] private GameObject contentPrefab;
    
    [SerializeField, Header("State")] private bool isFloating = false;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private float transitionTime = 0.5f;
    [SerializeField] private float openTime = 0.5f;
    [SerializeField] private InteractableUnityEventWrapper eventWrapper;
    [SerializeField, Range(0, 1)] private float expansionPivot = 0.5f;
    
    [Header("Events")] public UnityEvent onPoke;

    private GameObject _contents;

    public bool IsFloating
    {
        get => isFloating;
        set => SetIsFloating(value);
    }

    public bool IsOpen => isOpen;

    private void Awake()
    {
        eventWrapper.WhenSelect.AddListener(onPoke.Invoke);

        _contents = Instantiate(contentPrefab, contentsParent);
        _contents.transform.localPosition = Vector3.zero;
        _contents.transform.localRotation = Quaternion.identity;
        _contents.transform.localScale = Vector3.one;
        _contents.gameObject.SetActive(false);
    }

    public void Open()
    {
        if (!isOpen)
        {
            SetIsOpen(true);
        }
    }

    public void Close()
    {
        if (isOpen)
        {
            SetIsOpen(false);
        }
    }

    private async void SetIsOpen(bool isNowOpen)
    {
        isOpen = isNowOpen;

        var start = Time.time;
        var elapsed = 0f;
        while (elapsed < openTime)
        {
            var t = (Time.time - start) / openTime;
            SetBubbleExpandedPercentage(isOpen ? t : 1 - t);
            
            await UniTask.NextFrame();
            elapsed = Time.time - start;
        }
        
        SetBubbleExpandedPercentage(isOpen ? 1 : 0);
    }

    private void OnValidate()
    {
       SetIsFloating(isFloating);
    }

    private void SetIsFloating(bool isNowFloating)
    {
        isFloating = isNowFloating;
        MoveToTransform(isFloating ? floatParent : globeParent);
    }

    private void SetBubbleExpandedPercentage(float t)
    {
        var isContracting = t < expansionPivot;
        bubble.gameObject.SetActive(isContracting);
        _contents.SetActive(!isContracting);

        if (isContracting)
        {
            bubble.localScale = (1f - t / expansionPivot) * Vector3.one;
        }
        else
        {
            var contentsT = (t - expansionPivot) / (1f - expansionPivot);
            _contents.transform.localScale = contentsT * Vector3.one;
        }
        
    }
    
    private async void MoveToTransform(Transform newTransform)
    {
        if (newTransform == transform.parent)
            return;
            
        transform.SetParent(newTransform);
        var startPos = transform.localPosition;
        var startRot = transform.localRotation;
        var startScale = transform.localScale;

        var targetPos = Vector3.zero;
        var targetScale = Vector3.one;
        var targetRot = Quaternion.identity;
        
        var start = Time.time;
        var elapsed = 0f;
        while (elapsed < transitionTime)
        {
            var t = (Time.time - start) / transitionTime;
            t = transitionCurve.Evaluate(t);
            
            var newPos = Vector3.Lerp(startPos, targetPos, t);
            var newRot = Quaternion.Lerp(startRot, targetRot, t);
            var newScale = Vector3.Lerp(startScale, targetScale, t);
            
            transform.localPosition = newPos;
            transform.localRotation = newRot;
            transform.localScale = newScale;
            
            await UniTask.NextFrame();
            elapsed = Time.time - start;
        }

        transform.localPosition = targetPos;
        transform.localRotation = targetRot;
        transform.localScale = targetScale;
    }
}
