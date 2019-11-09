using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// https://www.youtube.com/watch?v=J1ng1zA3-Pk

[ExecuteAlways()]
public class ProgressBar : MonoBehaviour {

    // Values
    [Header("Main Parameters")]
    public int  _min;         // In case the progress bar has a starting value that isn't zero; otherwise it's zero
    public int  _max;         // Self explanatory...
    public int  _currValue;
    public bool _loadBackwards;       // Makes the progress bar load right-to-left

    // Aesthetic properties
    [Header("Aesthetic Properties")]
    public Image _progressBarBackground;
    public Image _progressBarMask;
    public Image _progressBarFill;
    public Color _emptyBarColor;
    public Color _filledBarColor;

    // Start function; sets the 
    private void Start() {
        // For loading the progress bar left-to-right or right-to-left
        // The defualt is LTR; set this property in the inspector
        if (_loadBackwards) _progressBarMask.fillOrigin = 1;

        _progressBarBackground.color = _emptyBarColor;
        _progressBarFill.color = _filledBarColor;
    }

    private void Update() {
        UpdateFill();
    }

    // Updates the size of the progress bar mask; uses stored values
    public void UpdateFill() {
        float currOffset = _currValue - _min;
        float maxOffset  = _max - _min;
        float fillAmount = (maxOffset != 0) ? currOffset / maxOffset : 0;   // Divide by zero safeguard
        _progressBarMask.fillAmount = fillAmount;
    }

    public void FillForward() {
        _loadBackwards = false;
        _progressBarMask.fillOrigin = 0;
    }

    public void FillBackward() {
        _loadBackwards = true;
        _progressBarMask.fillOrigin = 1;
    }

    // Updates the size of the progress bar mask and all values
    public void UpdateFill(int min, int max, int currValue) {
        _min = Mathf.Max(0, min);
        _max = Mathf.Max(_min, max);
        _currValue = Mathf.Clamp(currValue, _min, _max);
        UpdateFill();
    }

    // Updates the size of the progress bar mask, max, and currValue; assumes no change to min
    public void UpdateFill(int max, int currValue) {
        _max = Mathf.Max(_min, max);
        _currValue = Mathf.Clamp(currValue, _min, _max);
        UpdateFill();
    }

    // Updates the size of the progress bar mask and currValue; assumes no change to min and max
    public void UpdateFill(int currValue) {
        _currValue = Mathf.Clamp(currValue, _min, _max);
        UpdateFill();
    }
}
