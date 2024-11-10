using System;
using UnityEngine;

public class CultureController : MonoBehaviour
{
    [SerializeField] private Bubble[] bubbles;

    private Bubble _currentSelectedBubble;
    
    private void Awake()
    {
        ConfigureBubbles();
    }
    private void ConfigureBubbles()
    {
        foreach (var bubble in bubbles)
        {
            bubble.onPoke.AddListener(() => OnBubblePoke(bubble));
        }
    }

    private void OnBubblePoke(Bubble bubble)
    {
        if (bubble == _currentSelectedBubble)
        {
            bubble.Open();
        }
        else
        {
            if (_currentSelectedBubble)
            {
                _currentSelectedBubble.Close();
                _currentSelectedBubble.IsFloating = false;
            }

            _currentSelectedBubble = bubble;
            _currentSelectedBubble.IsFloating = true;
        }
    }
}
