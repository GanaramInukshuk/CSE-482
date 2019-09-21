//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//// This is the same as the regular counter but instead of having a count
//// within the range [0, max], there are two counts (count1 and count2) such that
//// 0 >= count1 >= count2 >= max
//public class Bicounter {

//    private Counter _ctr2;       // Effective range of [0, max]
//    private Counter _ctr1;       // Effective range of [0, ctr2.Count]

//    // Constructor with a preset max; this calls each counter's own constructors,
//    // setting one to the max and the other to (presumably) zero
//    public Bicounter(int presetMax) {
//        _ctr2 = new Counter(presetMax);
//        _ctr1 = new Counter(0);
//    }

//    // Default constructor
//    public Bicounter() {
//        _ctr2 = new Counter();
//        _ctr1 = new Counter(0);
//    }

//    // Increment count1 by an arbitrary value; decrement using a negative value;
//    // count1 is within the interval [0, ctr2.Count]
//    public void IncrementCount1(int amt) {
//        _ctr1.IncrementCount(amt);
//    }

//    // Increment count1 by an arbitrary value; decrement using a negative value;
//    // If ctr2.Count ever becomes less than ctr1.Count1, ctr1.Max should be reassigned
//    // such that count1 stays within [0, ctr2.Count]; the setters and getters of the
//    // individual counters are designed to determine whether to adjust those values
//    public void IncrementCount2(int amt) {
//        _ctr2.IncrementCount(amt);
//        _ctr1.Max = _ctr2.Count;
//    }

//    // Increment the max by an arbitrary value; decrement using a negative value;
//    // If the max ever becomes less than count2, then readjust count2; also, if count2
//    // also becomes less than count1, readjust that also
//    public void IncrementMax(int amt) {
//        _ctr2.IncrementMax(amt);
//        _ctr1.Max = _ctr2.Count;
//    }

//    // Self-explanatory; the max remains unchanged, though
//    public void ZeroOutCount() {
//        _ctr2.ZeroOutCount();
//        _ctr1.ZeroOutCount();
//    }

//    // Self-explanatory; both counters have their count set to the max
//    public void MaxOutCount() {
//        _ctr2.MaxOutCount();
//        _ctr1.Max = _ctr2.Max;
//        _ctr1.MaxOutCount();
//    }

//    public int Max {
//        set { _ctr2.Max = value; }
//        get { return _ctr2.Max; }
//    }

//    public int Count2 {
//        set { _ctr2.Count = value; _ctr1.Max = _ctr2.Count; }
//        get { return _ctr2.Count; }
//    }

//    public int Count1 {
//        set { _ctr1.Count = value; }
//        get { return _ctr1.Count; }
//    }
//}