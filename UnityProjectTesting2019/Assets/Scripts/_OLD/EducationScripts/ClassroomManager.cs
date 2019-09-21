//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//// Don't attach to a regular old GameObject; instead have it be a member of a larger class 
//// that's attached to a UI or empty GameObject

//// Functionality:
//// - This class takes a number of students, a number of classrooms, and a classroom size
////   (provided by the class and saved as a counter) and determines whether that number of
////   classrooms is suitable for use with the given amount of students; generated info is 
////   saved for however long it's needed

//// Note: This model does not track the number of students in each individual classroom
//// but insteads tracks the expected classroom "occupancy" for a given amount of students;
//// consider the example of 20-seat classrooms and 401 students; in the worst-case scenario,
//// there will be one classroom with only one student with 20 more completely filled; in the
//// real world, some of those classrooms may be below max occupancy to help balance the
//// number of students per classroom, or one classroom may try to squeeze in a 21st student

//// (Also in the real world, classroom sizes may vary a lot, so this model is saying that
//// 20 is the average classroom size rather than saying every classroom is the same size)

//// Because of this, I've included an occupancy index where the number of students is divided
//// by the total number of seats; an index of around 1 is ideal and there is (roughly) a one
//// to one correspondence between students and available seats; an index of less than 0.5 means
//// there are way too many empty seats; an index greater than 1.5 (or maybe 2) means that
//// classrooms are overcrowded due to demand; this index may be use to gauge effectiveness
//// of schools overall, but its main purpose is to gauge overall demand

//// Note: an occupancy index of -1 is a failsafe to prevent division by 0; this means there
//// are no classrooms available

//// The aforementioned example with 401 students would have an index of ~1.003 with 20
//// classrooms and ~0.955 with 21 classrooms

//// The three numbers needed for savedata are the number of students attending (seats required), the
//// number of classrooms available, and the classroom size; everything else can be generated from there;
//// for the EducationSimulator, classrooms available is computed from the number of schools, so the
//// numbers to save then are the number of schools and the number of students

//namespace EducationScripts {
//    // Interface for classroom stats
//    // - EducationId: for distinguishing between different EducationManagers
//    // - ClassroomSize: the size of each classroom in seats; one seat per student
//    // - ClassroomsAvailable: the number of classrooms the EducationManager has received from the
//    //   generate function
//    // - ClassroomsRequired: total number of classrooms needed to meet demand
//    // - ClassroomsLeftover: the number of classrooms away from the required number of classrooms*
//    // - SeatsAvailable: total capacity of all classrooms combined, in seats
//    // - SeatsRequired: total number of seats needed to meet demand; basically the number of students
//    //   passed into the Generate function
//    // - SeatsLeftover: the number of seats away from the required number of seats*
//    // - ClassroomOccupancyIndex: a measure of how full or sparse classrooms are
//    // * - These figures may be positive or negative; positive indicates empty seats or empty
//    //     classrooms after demand is satisfied; negative indicates a deficit in seats or classrooms
//    //     due to demand not being completely satisfied
//    public interface IClassroom {
//        int   EducationId             { get; }
//        int   ClassroomSize           { get; }
//        int   ClassroomsAvailable     { get; }
//        int   ClassroomsRequired      { get; }
//        int   ClassroomsLeftover      { get; }
//        int   SeatsAvailable          { get; }
//        int   SeatsRequired           { get; }
//        int   SeatsLeftover           { get; }
//        float ClassroomOccupancyIndex { get; }
//    }

//    public class ClassroomManager : IClassroom {
//        // For divvying up student count into classrooms
//        private PartitionManager _mgr = new PartitionManager();

//        // Setters and getters
//        public float ClassroomOccupancyIndex { get; private set; } = 0.0f;
//        public int   EducationId             { get; private set; } = -1;    // -1 for error state; use for debugging/testing
//        public int   SeatsRequired           { get; private set; } =  0;    // Needed for savedata; equivalent to the number of students passed in
//        public int   ClassroomsAvailable     { get; private set; } =  0;    // Needed for savedata; classrooms needed to accommodate all students
        
//        // Getters; these values may be used to evaluate school performance
//        public int   ClassroomSize      => _mgr.PartitionSize;
//        public int   SeatsAvailable     => ClassroomSize * ClassroomsAvailable;
//        public int   SeatsLeftover      => SeatsAvailable - SeatsRequired;
//        public int   ClassroomsRequired => _mgr.Partitions;
//        public int   ClassroomsLeftover => ClassroomsAvailable - ClassroomsRequired;

//        // Constructors
//        public ClassroomManager(int eduId) { EducationId = eduId; }     // This constructor is recommended
//        public ClassroomManager()          { }                          // Only use this constructor for debugging/testing

//        // Generate function; requires extra parameters for functionality
//        // This partitions the students into groups of the given classroom size and compares
//        // the number of partitions to the number of classrooms passed into the function;
//        // an occupancy index is also generated based on the number of students and the number
//        // of seats based on the number of classrooms 
//        public void Generate(int students, int classrooms, int classroomSize) {
//            SeatsRequired       = students;
//            ClassroomsAvailable = classrooms;
//            _mgr.Generate(students, classroomSize);
//            ClassroomOccupancyIndex = students != 0 ? (float)students / SeatsAvailable : -1;
//        }

//        // Incrementer for class size; this is for the partition manager
//        //public void AddToClassSize(int n) { _mgr.IncrementSize(n);  }      // Changes class size

//        // Debug functions
//        public void PrintDebugString() {
//            string outputString = GetDebugString();
//            Debug.Log(outputString);
//        }

//        public string GetDebugString() {
//            string s = $"[ClassroomManager]: ID: {EducationId}, Classrooms: {ClassroomsAvailable}, Size: {ClassroomSize}, Capacity: {SeatsAvailable}, Students: {SeatsRequired}, Index: {ClassroomOccupancyIndex}";
//            return s;
//        }
//    }
//}