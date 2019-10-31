using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimulatorInterfaces;

// Don't attach to a regular old GameObject; instead have it be a member of a larger class 
// that's attached to a UI or empty GameObject

// Functionality:
// - This is simply a counter of commercial buildings of various sizes; the counts are then used
//   for the StoreCounter's max
// - Note: employment is measured in employment units (default of 10 employees)

namespace CommercialScripts {

    public class StoreCounter : ArrayCounter, IZonableBuilding {
        // Getters
        public int TotalBuildings => DistributionGen.Histogram.SumOfElements(Count);
        public int OccupantMax    => ExtraMath.Linear.DotProduct(Count, Constants.EmploymentSizes);
        public int[] BldgVector   => Count;

        // Constructor
        public StoreCounter() : base(Constants.StoreVectorLength) { }

        // Overridden debug function
        public override string GetDebugString() {
            return "[StoreCounter]: " + DistributionGen.Debug.HistToString(Count);
        }
    }
}