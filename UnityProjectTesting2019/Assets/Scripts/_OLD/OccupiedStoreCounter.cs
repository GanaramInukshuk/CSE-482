//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//// Don't attach to a regular old GameObject; instead have it be a member of a larger class 
//// that's attached to a UI or empty GameObject

//// This counter is for counting the number of occupied commercial buildings; bldgs come in 5-6 sizes, each with
//// a different employment capacity { 10, 20, 40, 80, 160, 320 } or "employment units" (one employment unit translates
//// to a workforce of 10; in employment units, this would be { 1, 2, 4, 8, 16, 32 })

//// These buildings are referred to as size 1 to 6; the formula for employment capacity is 10 * 2^(size - 1)

//// Commercial businesses will occupy any one of the six different size buildings and that building's maximum employment
//// capacity translates to the number of jobs available for the city, in employment units

//// The total number of employment units is the number of available jobs in an entire city (disregarding jobs produced
//// by industrial and civic services) and is compared against the number of able workers to determine whether the
//// demand for jobs is satisfied

//namespace CommercialScripts {
//    // Interface for the BldgCtr
//    public interface IOccupiedStore {
//        int TotalOccupiedStores     { get; }    // For getting the total number of occupied stores/bldgs
//        int TotalEmploymentCapacity { get; }    // For getting the total employment capacity
//        int this[int i]             { get; }    // For getting the number of occupied bldgs of a certain size
//    }

//    public class OccupiedStoreCounter : MultiCounter, IOccupiedStore {
//        // Getters
//        public int TotalOccupiedStores     => DistributionGen.Histogram.SumOfElements(Count);
//        public int TotalEmploymentCapacity => Constants.EmploymentUnitSize * ExtraMath.Linear.DotProduct(Count, Constants.EmploymentSizes);

//        // Constructor
//        public OccupiedStoreCounter() : base(Constants.NumEmploymentSizes) { }

//        // Indexer
//        public int this[int i] => Count[i];

//        // Debug functions
//        public void PrintDebugString() {
//            string outputString = GetDebugString();
//            Debug.Log(outputString);
//        }

//        public string GetDebugString() {
//            return "[OccupiedStoreCounter]: " + DistributionGen.Debug.HistToString(Count);
//        }
//    }
//}