using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Don't attach to a regular old GameObject; instead have it be a member of a larger class 
// that's attached to a UI or empty GameObject

// Functionality:
// - This is simply a counter of commercial buildings of various sizes; the counts are then used
//   for the StoreCounter's max
// - Note: employment is measured in employment units (default of 10 employees)

namespace CommercialScripts {

    public interface IStore {
        int this[int i]   { get; }     // For getting the total number of buildings of a given size, regardless of occupancy
        int TotalStores   { get; }     // For getting the total number of buildings
        int MaxLaborUnits { get; }     // ...max labor units
    }

    public class StoreCounter : ArrayCounter, IStore {
        // Getters
        public int TotalStores   => DistributionGen.Histogram.SumOfElements(Count);
        public int MaxLaborUnits => ExtraMath.Linear.DotProduct(Count, Constants.EmploymentSizes);

        // Constructor
        public StoreCounter() : base(Constants.StoreVectorLength) { }

        // Overridden debug function
        public override string GetDebugString() {
            return "[StoreCounter]: " + DistributionGen.Debug.HistToString(Count);
        }
    }
}