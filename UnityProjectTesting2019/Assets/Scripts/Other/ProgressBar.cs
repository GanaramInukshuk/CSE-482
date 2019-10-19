using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// https://www.youtube.com/watch?v=J1ng1zA3-Pk

[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour {

    // Values
    [Header("Main Parameters")]
    [SerializeField] private int _min;         // If case the progress bar has a starting value that isn't zero; otherwise it's zero
    [SerializeField] private int _max;         // Self explanatory...
    [SerializeField] private int _currValue;

    // Aesthetic properties
    [Header("Aesthetic Properties")]
    [SerializeField] private Image _progressBarBackground;
    [SerializeField] private Image _progressBarMask;
    [SerializeField] private Image _progressBarFill;
    [SerializeField] private Color _emptyBarColor;
    [SerializeField] private Color _filledBarColor;

    // Start function; sets the 
    private void Start() {
        _progressBarBackground.color = _emptyBarColor;
        _progressBarFill.color = _filledBarColor;
    }

    public void UpdateFill() {
        float currOffset = _currValue - _min;
        float maxOffset  = _max - _min;
        float fillAmount = currOffset / maxOffset;
        _progressBarMask.fillAmount = fillAmount;
    }

    public void UpdateFill(int min, int max, int currValue) {
        _min = Mathf.Max(0, min);
        _max = Mathf.Max(_min, max);
        _currValue = Mathf.Clamp(currValue, _min, _max);
        UpdateFill();
    }

    public void UpdateFill(int max, int currValue) {
        _max = Mathf.Max(_min, max);
        _currValue = Mathf.Clamp(currValue, _min, _max);
        UpdateFill();
    }

    public void UpdateFill(int currValue) {
        _currValue = Mathf.Clamp(currValue, _min, _max);
        UpdateFill();
    }
}
