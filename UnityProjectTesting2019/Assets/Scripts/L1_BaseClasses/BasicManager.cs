using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Functionality:
// - This class simply takes a number of datapoints and an array of probabilities and generates
//   a histogram that's saved for however long it's needed
// - This is a barebones manager; only the minimum amount of functionality is included
// - To use this class, instantiate it directly, have it be a member of a larger class, or
//   if more functionality is needed, have it be inherited and have the inheriting class
//   add additional functionality

// Organization of a class for this particular project:
// - Enums
// - Constants, including private getters
// - Structs
// - Subclasses
// - Private member variables/objects
// - Setters and getters defined using setter-getter shorthand * ** ***
// - Setters and getters that cannot be defined using shorthand * ** ***
// - Constructors and destructors
// - Overloaded operators
// - Functions, including helper functions * ***
// - Debug functions ***

// * If the class is encapsulating instances of another class, then functions needed to access
//   class functions preceed any functions needed for the encapsulating class
// ** Organized as properties with setter & getter, then setter-only, then getter-only
// *** Organized form public to private

public class BasicManager {

    // Insisting on these members be private prevents unnecessary tampering by inheriting classes
    protected readonly int _vectorSize;
    private int[]        _vector;

    // Gets the total number of datapoints in the vector (or histogram)
    //public int Total => DistributionGen.Histogram.SumOfElements(_vector);

    // The setter is for loading previously saved data
    public int[] DataVector {
        set { if (VerifyVector(value)) _vector = value; }
        get => _vector;
    }

    // Indexer; should avoid having to do manager.DataVector[i]
    public int this[int i] => _vector[i];

    // The vectorSize (the number of data "buckets" in the histogram/vector) is set in stone in the constructor
    public BasicManager(int vectorSize) { 
        _vectorSize = Mathf.Max(0, vectorSize);
        _vector = new int[_vectorSize];
    }

    // Generates a histogram using a hybrid method
    // There are two histograms being generated:
    // - One is generated using the round-down method
    // - The second uses the descending method for the discrepancy
    // Cumulative time complexity: O(weights.Length^2)
    // Function can be overridden if needed (EG, if some randomness is desired)
    public virtual void Generate(int n, float[] weights) {
        if (n > 0 && weights.Length == _vectorSize) {
            int[] v1 = DistributionGen.Histogram.GenerateByRoundDown(n, weights);
            int disc = DistributionGen.Histogram.Discrepancy(n, v1);
            int[] v2 = DistributionGen.Histogram.GenerateByDescending(-disc, weights);
            _vector  = DistributionGen.Histogram.Merge(v1, v2);
        } else _vector = new int[_vectorSize];
    }

    // Helper function for the Vector setter
    // Criteria for validity:
    // - The vector is a valid histogram (see DistributionGen class)
    // - The length matches the length stored in the class
    private bool VerifyVector(int[] vector) {
        return DistributionGen.Verify.HistValid(vector) && vector.Length == _vectorSize;
    }

    // Debug functions
    public void PrintDebugString() {
        string outputString = GetDebugString();
        Debug.Log(outputString);
    }

    // Function can be overridden if needed
    public virtual string GetDebugString() {
        return $"[BasicManager]: " + DistributionGen.Debug.HistToString(_vector);
    }
}