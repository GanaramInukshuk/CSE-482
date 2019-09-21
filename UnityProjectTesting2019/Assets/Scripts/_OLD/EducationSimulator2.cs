//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using EducationScripts;

//// Don't attach to a regular old GameObject; instead have it be a member of a larger class 
//// that's attached to a UI or empty GameObject

//// TODO: Write a descripton of what this class does

//public class EducationSimulator {
//    // For use with savedata
//    // The only thing needed to be saved are the number of schools, the number of students,
//    // and the classroom size; everything else can be regenerated from those numbers
//    public struct EducationData {
//        public int[] students;
//        public int[] schoolCount;
//        public int[] classroomSize;
//    }

//    // Counters and managers
//    private readonly SchoolCounter        _schoolCtr    = new SchoolCounter();
//    private readonly ClassroomSizeCounter _sizeCtr      = new ClassroomSizeCounter();
//    private readonly ClassroomManager[]   _classroomMgr = new ClassroomManager[Constants.NumEducationLevels];

//    // An int array to hold the previously entered student count
//    private int[] _studentCount = new int[Constants.NumEducationLevels];

//    // Setter-getter for savedata
//    // Setting is easily done by calling the Generate function
//    // Getting is done by getting the numbers saved in the counters (and the schoolcount array)
//    public EducationData Savedata {
//        set { GenerateEducation(value.students, value.schoolCount, value.classroomSize); }
//        get => new EducationData {
//            students      = _studentCount,
//            schoolCount   = _schoolCtr.Count,
//            classroomSize = _sizeCtr.Count
//        };
//    }

//    // Other getters for interface-implementing objects
//    // An interface for classroom size is probably unnecessary since that counter is only useful
//    // to the education manager and it can be inferred from the classroom interface, anyway
//    // (In other words, classroom count is more important than school count)
//    public ISchool SchoolBreakdown => _schoolCtr;

//    // Constructor; this has to initialize an array of classroom managers, THEN initialize each one;
//    // the main thing needed to do is assign an education ID to each classroom manager
//    public EducationSimulator() {
//        for (int i = 0; i < Constants.SchoolSizes.Length; i++) _classroomMgr[i] = new ClassroomManager(i);
//    }

//    // Constructor that accepts savedata; still needs to initialize the classroom manager array
//    public EducationSimulator(EducationData savedata) { 
//        for (int i = 0; i < Constants.SchoolSizes.Length; i++) _classroomMgr[i] = new ClassroomManager(i);
//        Savedata = savedata;
//    }

//    // Indexer-Getter for classroom information; may be used to score school performance;
//    // This is returned as an interface-implementing object and the manager's stats can be
//    // accessed in this way (Classrooms, EducationId, etc)
//    // Notice that the index is the education level
//    public IClassroom this[Constants.EDUCATIONLEVEL ed] => _classroomMgr[(int)ed];

//    // A one-param function that uses stored counts for classroom size and school count; the only
//    // param is the number of students and should be the default for simulation
//    public void GenerateEducation(int[] students) {
//        _studentCount = students;                           // Record the number of students
//        //_schoolCtr.Count = schoolCount;                     // Set the amount of schools    
//        //_classroomSizeCounter.Count = classroomSize;        // Set the classroom sizes
//        for (int i = 0; i < Constants.NumEducationLevels; i++) {
//            _classroomMgr[i].Generate(students[i], _schoolCtr[(Constants.EDUCATIONLEVEL)i], _sizeCtr[i]);
//        }
//    }

//    // A three-param function that sets counters; other than the students, the params being passed
//    // are the number of schools and the class size
//    public void GenerateEducation(int[] students, int[] schoolCount, int[] classroomSize) {
//        _studentCount = students;                           // Record the number of students
//        _schoolCtr.Count = schoolCount;                     // Set the amount of schools    
//        _sizeCtr.Count = classroomSize;        // Set the classroom sizes
//        for (int i = 0; i < Constants.NumEducationLevels; i++) {
//            // Note: To get the number of classrooms, use the indexer that uses EDUCATIONLEVEL as the index
//            _classroomMgr[i].Generate(students[i], _schoolCtr[(Constants.EDUCATIONLEVEL)i], _sizeCtr[i]);
//        }
//    }

//    // For incrementing classroom sizes or total number of schools
//    public void AddToClassSize(int n, Constants.EDUCATIONLEVEL lvl) { _sizeCtr  .IncrementCount(n, (int)lvl); }
//    public void AddSchools    (int n, Constants.EDUCATIONLEVEL lvl) { _schoolCtr.IncrementCount(n, (int)lvl); }

//    // Debug functions
//    public void PrintDebugString() {
//        string outputString = GetDebugString();
//        Debug.Log(outputString);
//    }

//    public string GetDebugString() {
//        string s = "[EducationSimulator]: " + DistributionGen.Debug.HistToString(_schoolCtr.Count);
//        foreach (ClassroomManager c in _classroomMgr) s += "\n" + c.GetDebugString();
//        return s;
//    }
//}