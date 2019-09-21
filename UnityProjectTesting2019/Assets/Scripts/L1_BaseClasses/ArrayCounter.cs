using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a counter whose range is [0, some_arbitrary_max]
// Setting the max really high mimics a range of [0, infinity); this is also the default

// Unlike the regular counter that keeps track of a single value, this keeps track of an
// array of values in which the values can be manipulated independently of one another
// or all at once

// Each element is effectively its own counter with a range of [0, max] wherein one element
// does not influence the max of other elements; in other words, the combined range of each
// element is not [0, max] but instead [0, max * (n-1)]

// For this to make sense, all the elements in the array have to be related in some way;
// EG a store that keeps track of three different fruits (bananas, apples, pineapples)
// where a running total for each fruit is needed as well as a cumulative total

public class ArrayCounter {

    private readonly int _vectorSize;
    private int[]        _count;
    private int          _max;

    // Setter-getter for the max; this can also force the max to be any non-negative value
    // Negative values get converted to zeros
    public int Max {
        set {
            _max = Mathf.Max(value, 0);
            ClampCount();
        }
        get => _max;
    }

    // Setter-getter for the count; this can force the count to be any non-negative value
    public int[] Count {
        set { if (VerifyVector(value)) for (int i = 0; i < _vectorSize; i++) _count[i] = Mathf.Clamp(value[i], 0, _max); }
        get => _count;
    }

    // Getter for the total; these are the individual counts added together
    //public int Total => DistributionGen.Histogram.SumOfElements(_count); 

    // Constructor with a preset max
    public ArrayCounter(int presetMax, int vectorSize) {
        Max = presetMax;
        _count = new int[vectorSize];
        _vectorSize = _count.Length;
    }

    // Default constructor
    public ArrayCounter(int vectorSize) {
        _max = int.MaxValue;
        _count = new int[vectorSize];
        _vectorSize = _count.Length;
    }

    // Copy constructor
    public ArrayCounter(ArrayCounter ac) {
        _vectorSize = ac._vectorSize;
        _count      = ac._count;
        _max        = ac._max;
    }

    // Indexer; like overloading the [] operator in C++
    // This effectively allows accessing and changing any one value in the count array
    // Obviates the need to do some_array_counter.Count[i]
    public int this[int i] {
        set { _count[i] = Mathf.Clamp(value, 0, _max); }
        get => _count[i];
    }

    // Increment a count by an arbitrary value; decrement using a negative value
    public void IncrementCount(int amt, int index) {
        if (VerifyIndex(index)) _count[index] = Mathf.Clamp(_count[index] + amt, 0, _max);
    }

    // Increment all the counts at once by using an array of values
    public void IncrementCount(int[] amt) {
        if (!VerifyVector(amt)) return;
        for (int i = 0; i < _vectorSize; i++) _count[i] += amt[i];
        ClampCount();
    }

    // Increment the max by an arbitrary value; decrement using a negative value
    public void IncrementMax(int amt) {
        if (_max + amt < 0) _max = 0;
        else _max += amt;
        ClampCount();
    }

    // Additional functions for zeroing or maxing out the counter
    public void ZeroOutCount() { for (int i = 0; i < _vectorSize; i++) _count[i] = 0;    }
    public void MaxOutCount()  { for (int i = 0; i < _vectorSize; i++) _count[i] = _max; }

    // Helper functions
    protected bool VerifyIndex(int index) {
        return 0 <= index && index < _vectorSize;
    }

    protected bool VerifyVector(int[] vector) {
        return vector.Length == _vectorSize;
    }

    // Ensure value doesn't exceed range
    private void ClampCount() {
        for (int i = 0; i < _vectorSize; i++) _count[i] = Mathf.Clamp(_count[i], 0, _max);
    }

    // Debug functions
    public void PrintDebugString() {
        string outputString = GetDebugString();
        Debug.Log(outputString);
    }

    // Function can be overridden if needed
    public virtual string GetDebugString() {
        return "[ArrayCounter]: " + DistributionGen.Debug.HistToString(_count);
    }
}