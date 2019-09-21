//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//// Don't attach to a regular old GameObject; instead have it be a member of a larger class 
//// that's attached to a UI or empty GameObject

//// Functionality:
//// - The SchoolCounter simply counts the number of schools for each level of education; this also converts
////   the number of schools into classrooms that the ClassroomManagers can use
//// - This is basically a customized ArrayCounter

//// The added functionality of this counter (plus the interface) makes this class more like a manager in all
//// but by name

//namespace EducationScripts {

//    public interface ISchool {
//        int SchoolTotal { get; }
//        int this[Constants.EDUCATIONLEVEL ed] { get; }
//        int SchoolCount(Constants.EDUCATIONLEVEL ed);
//    }

//    public sealed class SchoolCounter : ArrayCounter, ISchool {
//        // For getting the total number of schools
//        public int SchoolTotal => DistributionGen.Histogram.SumOfElements(Count);

//        // Constructor
//        public SchoolCounter() : base(Constants.NumEducationLevels) { }

//        // Indexer; this returns the number of classrooms for a given education level
//        public int this[Constants.EDUCATIONLEVEL ed] => Count[(int)ed] * Constants.SchoolSizes[(int)ed];

//        // All base functionality handled in ArrayCounter

//        // This returns the number of schools for a given education level
//        public int SchoolCount(Constants.EDUCATIONLEVEL ed) {
//            return Count[(int)ed];
//        }

//        // Overridden debug function
//        public override string GetDebugString() {
//            return "[SchoolCounter]: " + DistributionGen.Debug.HistToString(Count);
//        }
//    }
//}