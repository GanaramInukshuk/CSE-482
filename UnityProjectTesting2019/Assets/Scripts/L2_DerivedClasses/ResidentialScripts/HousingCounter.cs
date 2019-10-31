using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimulatorInterfaces;

// Don't attach to a regular old GameObject; instead have it be a member of a larger class 
// that's attached to a UI or empty GameObject

// Functionality:
// - The HousingCounter simply counts the number of residential buildings of different sizes (IE, simplexes,
//   duplexes, and up); this also converts the housing count into something the other managers can understand
//   (IE, simplexes are one unit of occupancy, duplexes are two, etc)
// - This needs to receive an array of housing sizes in its constructor; it's not guaranteed that I'll simply
//   go up by 1 (IE, 1, 2, 3, 4, 5, 6) and I may instead skip a few numbers (EG, 1, 2, 3, 4, 6, 8, 10, 12)

// The added functionality of this counter (plus the interface) makes this class more like a manager in all
// but by name

namespace ResidentialScripts {

    public sealed class HousingCounter : ArrayCounter, IZonableBuilding {
        // Getters for housing; lines up with interface's members
        public int   TotalBuildings => DistributionGen.Histogram.SumOfElements(Count);
        public int   OccupantMax    => ExtraMath.Linear.DotProduct(Count, Constants.HousingSizes);
        public int[] BldgVector     => Count;

        // Constructor
        public HousingCounter() : base(Constants.HousingVectorLength) { }

        // All base functionality handled in ArrayCounter

        // Overridden debug function
        public override string GetDebugString() {
            return "[HousingCounter]: " + DistributionGen.Debug.HistToString(Count);
        }
    }
}