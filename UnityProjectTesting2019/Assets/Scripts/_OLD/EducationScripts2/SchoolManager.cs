//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//// Functionality of SchoolManager:
//// - This uses a bicounter to count the number of available schools and, in turn, the max number
////   of classrooms available
//// - Encapsulation is necessary to prevent manipulating the max

//namespace EducationScripts {

//    interface ISchool {
//        int EducationID     { get; }
//        int TotalSchools    { get; }
//        int MaxClassrooms   { get; }
//        int TotalClassrooms { get; }
//        int TotalSeats      { get; }
//    }

//    public class SchoolManager : ISchool {

//        private readonly Bicounter _btr;

//        // Setters and getters
//        public int EducationID { get; }

//        // Just like the other managers, this manager has a datavector; the getter is handled by the bicounter
//        // while the setter is custom-written and based off of the bicounter's setter
//        // Note: this doesn't really need the max from the datavector, but the fact that the zeroth element of
//        // this manager's datavector is 2.147 billion should be an indicator that it's working as intended, and
//        // it needn't be 2.147 billion, just a number high enough that's believable (like 65535 or 255)
//        public int[] DataVector {
//            set {
//                if (value.Length == 3) {
//                    _btr.Max1   = int.MaxValue;
//                    _btr.Count1 = value[1];     // Number of schools => max number of classrooms
//                    _btr.Count2 = value[2];     // Number of classrooms
//                }
//            }
//            get => _btr.DataVector;
//        }

//        // Setter-getter for the total number of schools available
//        public int TotalSchools {
//            set { if (value >= 0) _btr.Count1 = value; }
//            get => _btr.Count1;
//        }

//        // Setter-getter for the total (not max) number of classrooms available
//        public int TotalClassrooms {
//            set { if (value >= 0) _btr.Count2 = value; }
//            get => _btr.Count2;
//        }

//        // Other getters
//        public int MaxClassrooms => _btr.Max2;
//        public int TotalSeats    => _btr.Count2 * Constants.SeatsPerClassroom;

//        // Constructor; this takes in an education ID and assigns the bicounter's ratio (classrooms per school)
//        // based on the eduID passed in; if the eduID is invalid, then it's set to -1 and the ratio is
//        // set to a default value (20 classrooms per school by default)
//        public SchoolManager(int eduID) {
//            EducationID = (eduID < 0) ? -1 : eduID;
//            int ratio = (0 <= eduID && eduID < Constants.ClassroomsPerSchool.Length) ? eduID : 1;
//            _btr = new Bicounter(-1, Constants.ClassroomsPerSchool[ratio]);
//        }

//        // Constructor for debug purposes; this will force the ID to be -1
//        public SchoolManager() {
//            EducationID = -1;
//            _btr = new Bicounter(-1, Constants.ClassroomsPerSchool[0]);
//        }

//        // Incrementers; note that the placement of one new school automatically adds classrooms
//        public void IncrementSchools   (int amt) { _btr.IncrementCount (amt); }
//        public void IncrementClassrooms(int amt) { _btr.IncrementCount2(amt); }
//        public void ZeroOutClassrooms() { _btr.ZeroOutCount2(); }
//        public void MaxOutClassrooms () { _btr.MaxOutCount2 (); }

//        // Debug functions
//        public void PrintDebugString() {
//            Debug.Log(GetDebugString());
//        }

//        public string GetDebugString() {
//            return $"[SchoolManager]: EduID: {EducationID} Schools: {TotalSchools} Classrooms: {TotalClassrooms} out of {MaxClassrooms} Seats: {TotalSeats}";
//        }
//    }
//}