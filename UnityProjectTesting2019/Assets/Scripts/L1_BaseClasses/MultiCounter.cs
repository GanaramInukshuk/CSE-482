using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is just like the ArrayCounter except each index has its own maximum
// This is effectively the same as having Counter[] except it's easier to extract
// the count and max vectors this way (IE, no iterating through Counter[])

public class MultiCounter {

    private readonly int _vectorSize;
    private int[]        _count;
    private int[]        _max;

    // Setter-getter for each of the maxes (maxis? maximum : maxima :: max : maxa? max : max :: sheep : sheep?)
    // This can force the maxima to be any non-negative value
    // Negative values get converted to zeros
    public int[] Max {
        set {
            if (VerifyVector(value)) _max = value;
            ClampMax();         // Any wrongly entered negative values get convereted to zeros
            ClampCount();       // Any counts that exceed the new max are clamped
        }
        get => _max;
    }

    // Setter-getter for the count; this can force the count to be any non-negative value
    public int[] Count {
        set { if (VerifyVector(value)) for (int i = 0; i < _vectorSize; i++) _count[i] = Mathf.Clamp(value[i], 0, _max[i]); }
        get => _count;
    }

    // Constructor with a preset max; vector size is obtained from presetMax.Length;
    public MultiCounter(int[] presetMax) {
        _vectorSize = presetMax.Length;
        _count = new int[_vectorSize];
        Max = presetMax;        // Using the setter calls the clamp helper functions
        for (int i = 0; i < _vectorSize; i++) {
            _count[i] = 0;
        }
    }

    // Default constructor; sets all counts' max to be the same (or int.MaxValue if not specified)
    public MultiCounter(int vectorSize, int presetMax = int.MaxValue) {
        _vectorSize = vectorSize;
        _max   = new int[vectorSize];
        _count = new int[vectorSize];
        for (int i = 0; i < vectorSize; i++) {
            _max[i] = presetMax;
            _count[i] = 0;
        }
    }

    // Copy constructor (wouldn't hurt)
    public MultiCounter(MultiCounter mc) {
        _vectorSize = mc._vectorSize;
        _count      = mc._count;
        _max        = mc._max;
    }

    //// Indexer; effectively allows accessing and changing any one value in the count array
    //// Obviates the need to do multiCounterInstance.Count[i]
    //public int this[int i] {
    //    set { _count[i] = Mathf.Clamp(value, 0, _max[i]); }
    //    get => _count[i];
    //}

    // Increment a count by an arbitrary value; decrement using a negative value
    public void IncrementCount(int amt, int index) {
        if (VerifyIndex(index)) _count[index] = Mathf.Clamp(_count[index] + amt, 0, _max[index]);
    }

    // Increment all the counts at once by using an array of values
    public void IncrementCount(int[] amt) {
        if (!VerifyVector(amt)) return;
        for (int i = 0; i < _vectorSize; i++) {
            _count[i] += amt[i];
            _count[i] = Mathf.Clamp(_count[i], 0, _max[i]);
        }
    }

    // Increment a max by an arbitrary value; decrement using a negative value
    public void IncrementMax(int amt, int index) {
        if (VerifyIndex(index)) {
            _max[index] = Mathf.Max(_max[index] + amt, 0);
            _count[index] = Mathf.Clamp(_count[index], 0, _max[index]);
        }
    }

    // Increment all max at once by using an array of values
    public void IncrementMax(int[] amt) {
        if (VerifyVector(amt)) {
            for (int i = 0; i < _vectorSize; i++) {
                _max[i] = Mathf.Max(_max[i] + amt[i], 0);
                _count[i] = Mathf.Clamp(_count[i], 0, _max[i]);
            }
        }
    }

    // Zeroing out or maxing counts
    public void ZeroOutCount(int index) {
        if (VerifyIndex(index)) _count[index] = 0;
    }

    public void ZeroOutCount() {
        for (int i = 0; i < _vectorSize; i++) _count[i] = 0;
    }

    public void MaxOutCount(int index) {
        if (VerifyIndex(index)) _count[index] = _max[index];
    }

    public void MaxOutCount() {
        for (int i = 0; i < _vectorSize; i++) _count[i] = _max[i];
    }

    // Helper functions
    // Ensures index is valid, within the range [0, _vectorSize)
    protected bool VerifyIndex(int index) {
        return 0 <= index && index < _vectorSize;
    }

    // Ensures the size of the vector is valid
    protected bool VerifyVector(int[] vector) {
        return vector.Length == _vectorSize;
    }

    // Ensure counts doesn't exceed range
    private void ClampCount() {
        for (int i = 0; i < _vectorSize; i++) _count[i] = Mathf.Clamp(_count[i], 0, _max[i]);
    }

    // Ensure max are valid, too; will default to 0 if not
    private void ClampMax() {
        for (int i = 0; i < _vectorSize; i++) _max[i] = Mathf.Max(0, _max[i]);
    }

    // Debug functions
    public void PrintDebugString() {
        string outputString = GetDebugString();
        Debug.Log(outputString);
    }

    // Function can be overridden if needed
    public virtual string GetDebugString() {
        return "[MultiCounter]: Max: " + DistributionGen.Debug.HistToString(_max) + " Count: " + DistributionGen.Debug.HistToString(_count);
    }
}