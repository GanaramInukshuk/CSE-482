using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a counter whose range is [0, some_arbitrary_max]
// Setting the max really high (int.MaxValue) mimics a range of [0, infinity); this is also the default
// This can be used for all sorts of applications, including a healthbar

public class Counter {

    private int _count = 0;
    private int _max   = int.MaxValue;

    // Setter-getter for the max; this can also force the max to be any non-negative value
    public int Max {
        set {
            _max = Mathf.Max(0, value);
            _count = Mathf.Min(_count, _max);
        }
        get { return _max; }
    }

    // Setter-getter for the count; this can force the count to be any non-negative value
    public int Count {
        set { _count = Mathf.Clamp(value, 0, _max); }
        get { return _count; }
    }

    // Constructor with a preset max
    public Counter(int presetMax) {
        Max = presetMax;
        _count = 0;
    }

    // Default constructor
    public Counter() { }

    // Copy constructor
    public Counter(Counter c) {
        _count = c._count;
        _max   = c._max;
    }

    // Increment the count by an arbitrary value; decrement using a negative value
    public void IncrementCount(int amt) {
        _count = Mathf.Clamp(_count + amt, 0, _max);
    }

    // Increment the max by an arbitrary value; decrement using a negative value
    public void IncrementMax(int amt) {
        _max   = Mathf.Max(_max + amt, 0);
        _count = Mathf.Clamp(_count, 0, _max);
    }

    // Additional functions for zeroing or maxing out the counter
    public void ZeroOutCount() { Count = 0;   }
    public void MaxOutCount()  { Count = Max; }
}