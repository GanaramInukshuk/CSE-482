using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Functionality:
// - A bicounter consists of two counters wherein the count of one counter is the max of the other counter, but
//   unlike an earlier implentation of the bicounter, there isn't necessarily a one-to-one correspondence between
//   the two counters
// - Example with a force field: one counter represents a force field and the second counter represents its equivalent
//   healthpoints (in hearts); suppose one unit of force field represents an effective bonus health of 3 
//   hearts and a player accrues two units of force field, giving the player 6 bonus hearts; these hearts take
//   precedence over regular healthpoints (IE, bonus hearts deplete before regular hearts do), so the count
//   of bonus hearts can decrement independently of the shield's count
// - Another example with containers: suppose one counter represents crates and the second counter represents
//   the number of packaged product you wish to ship; one crate can hold 200 product and you have 3 crates, meaning
//   the max amount of product you can pack (the max of the second counter) is 600

// Note: the two counts are incremented independenly of one another, so if _ctr1 is incremented by 1 (and ratio is
// 10), _ctr2 will NOT be incremented by 10; however, if _ctr1 is decremented and its new count is smaller than the
// max of _ctr2, then _ctr2 will clamp its count accordingly

// If there is a situation where incrementing _ctr1 must be followed up by incrementing _ctr2, there is an extra
// increment function for that

public class Bicounter {

    private Counter _ctr1;
    private Counter _ctr2;
    private readonly int _ratio;        // For the 1-to-n correspondence explained in the examples

    // Setter-getter for a datavector
    // A datavector consists of the following: { _ctr1.Max, _ctr1.Count, _ctr2.Count }
    // _ctr2.Max is generated from _ctr1.Count, so there's no need to save _ctr2.Max
    public int[] DataVector {
        set {
            if (value.Length == 3) {
                Max1   = value[0];
                Count1 = value[1];
                Count2 = value[2];
            }
        }
        get => new int[] { Max1, Count1, Count2 };
    }

    // Setter-getter for the max (_ctr1.Max)
    public int Max1 {
        set { _ctr1.Max = value; }
        get => _ctr1.Max;
    }

    // Setter-getter for count 1 (_ctr1.Count; the same as _ctr2.Max)
    // When setting, the value given to _ctr1.Count will become the max for _ctr2
    public int Count1 {
        set {
            _ctr1.Count = value;
            _ctr2.Max = _ctr1.Count * _ratio;
        }
        get => _ctr1.Count;
    }

    // Getter for the other max (_ctr2.Max)
    // The difference between _ctr2.Max and _ctr1.Count is all down to the ratio, and there is no
    // setter for this value since this max is determined by _ctr1.Count
    public int Max2 => _ctr2.Max;

    // Setter-getter for count 2 (_ctr2.Count)
    public int Count2 {
        set { _ctr2.Count = value; }
        get => _ctr2.Count;
    }

    // Constructor; this takes in a preset max and a ratio
    // Negative max and negative/zero ratio default to int.MaxValue and 1 respectively
    public Bicounter(int presetMax, int ratio) {
        _ratio = (ratio < 1) ? 1 : ratio;
        _ctr1  = (presetMax < 0) ? new Counter() : new Counter(presetMax);
        _ctr2  = new Counter(0);
    }

    // Default constructor; assumes a max of int.MaxValue and a ratio of 1
    public Bicounter() {
        _ratio = 1; 
        _ctr1 = new Counter( );
        _ctr2 = new Counter(0);
    }

    // Increment the max, but if the count of _ctr1 has changed, have that change be reflected in _ctr2
    // This isn't really needed if _ctr1.Max is already "infinity"
    public void IncrementMax(int amt) {
        _ctr1.IncrementMax(amt);
        _ctr2.Max = _ctr1.Count * _ratio;
    }

    // Increment the count of _ctr1 by amt, THEN increment the count of _ctr2 by amt * _ratio
    // Caveat: the order of operations for _ctr2 is different depending on whether the increment amt is positive
    // or negative; if positive, change the max THEN increment; if negative, "decrement" THEN change the max 
    public void IncrementCount(int amt) {
        _ctr1.IncrementCount(amt);
        int incrementAmt = amt * _ratio;
        if (amt < 0) {
            _ctr2.IncrementCount(incrementAmt);
            _ctr2.IncrementMax  (incrementAmt);
        } else {
            _ctr2.IncrementMax  (incrementAmt);
            _ctr2.IncrementCount(incrementAmt);
        }
    }

    // Increment the count of _ctr1 only; this change needs to be reflected in _ctr2.Max if decrementing
    public void IncrementCount1(int amt) { 
        _ctr1.IncrementCount(amt);
        _ctr2.IncrementMax(amt * _ratio);
    }

    // Increment the count of _ctr2 only; this is just one function
    public void IncrementCount2(int amt) {
        _ctr2.IncrementCount(amt);
    }

    // Zero out count 1 (which zeros out count 2 along with it)
    public void ZeroOutCount1() {
        _ctr1.ZeroOutCount();
        _ctr2.ZeroOutCount();
    }

    // Zero out count 2
    public void ZeroOutCount2() {
        _ctr2.ZeroOutCount();
    }

    // Max out both counts
    public void MaxOutCount() {
        MaxOutCount1();
        MaxOutCount2();
    }

    // Max out count 1 only
    public void MaxOutCount1() {
        _ctr1.MaxOutCount();
        _ctr2.Max = _ctr1.Max * _ratio;
    }

    // Max out count 2 only
    public void MaxOutCount2() {
        _ctr2.MaxOutCount();
    }
}