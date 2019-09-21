using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Don't attach to a regular old GameObject (unless testing); instead attach to a UI element, empty
// GameObject, or have it be a member of an even larger class (that's attached to a UI or empty GameObject)

// Functionality:
// - The HousingManager takes in a number of housing buildings and a probability distribution for the number
//   of sim/du/tri/fourplexes in a city and creates a histogram based on those probabilities; the histogram
//   is added to an array that is effectively a counter
// - The housingVector represents the number of houses that are simplexes, duplexes, triplexes, and fourplexes
//   in a city (meaning there are residential buildings that have 1 to 4 living units within, but the buildings
//   aren't necessarily apartments)

//namespace ResidentialScripts {

//    public sealed class HousingManager : BasicManager {

//        public HousingManager(int vectorSize) : base(vectorSize) { }

//        // Generate() functions and Vector and Total setter/getter are defined in base class

//        // For getting the total number of occupancy units
//        // Note that one occupied house counts as one unit of occupancy, regardless of the number
//        // of people living within; this converts the housing value into something the
//        // OccupancyManager can understand
//        public int Occupancy {
//            get {
//                int occupancyTotal = 0;
//                int[] housingVector = DataVector;
//                for (int i = 0; i < housingVector.Length; i++) occupancyTotal += (i+1) * housingVector[i];
//                return occupancyTotal;
//            }
//        }

//        // Overridden debug function
//        public override string GetDebugString() {
//            return "[HousingManager]: " + DistributionGen.Debug.HistToString(DataVector);
//        }
//    }
//}