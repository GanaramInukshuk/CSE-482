//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//// Don't attach to a regular old GameObject (unless testing); instead attach to a UI element, empty
//// GameObject, or have it be a member of an even larger class (that's attached to a UI or empty GameObject)

//// Functionality:
//// - The OccupationManager takes in a quantity of workplaces and a workforce distribution (a distribution
////   of the number of employees per workplace) and generates a histogram based on those probabilities; the
////   histogram (occupationVector) is stored for however long it's needed (until an updated version is needed)
//// - The occupationVector represents the number of workplaces with a certain workforce size; collectively,
////   it represents the total workforce across all workplaces
//// - The OccupationManager has additional functionality in that it determines whether the city's eligible
////   workforce can satisfy the OccupationManager's total workforce; in this sense, the OccupationManager's
////   total workforce is instead a maximum workforce; if the city's workforce is below the workforce maximum,
////   this means the city is in need of workers; otherwise, the city may be suffering from unemployment or there
////   is a demand for jobs
//// - This class is generalized so that more than one OccupationManager may be used at once to represent more
////   than one job type (EG, commercial jobs and industrial jobs); static member variables are included so as to
////   track the total number of satisfied jobs or job demand across all OccupationManagers
//// - Note: I don't expect "total workforce size" for any one workplace to be the same as the total number of
////   workers at a workplace at any one time, especially if the concept of shifts are included (IE, day vs night
////   shift); in other words, even if a workplace employs 50 different people, it's possible that a fraction of
////   them may be available and on-site at any one time as people clock in and out at different times

//// Possible job types (think SC4 or C:S but with a few extra categories):
//// - Commercial service jobs (stores)
//// - Industrial jobs (factories, manufacturing, etc)
//// - Civic (civil?) service jobs (police, health, education, etc)

//// The general order of functions in a class that I've apparently settled on:
//// - Any enums the class needs
//// - Constants needed by the class
//// - Any subclasses and structs needed, in that order
//// - Non-constant private member variables/objects
//// - Member variables declared using setter-getter shorthand
//// - Constructor(s)
//// - Public functions
//// - Setters and getters, especially those that can't be declared using shorthand
//// - Private/helper functions
//// * If the class is a composition, functions based off of the originals classes's
////   functions come before the ones unique to the composition
//// - Debug functions

//public class EmploymentManager {

//    private static readonly int _vectorSize = 10;
//    private readonly BasicManager _mgr = new BasicManager(_vectorSize);

//    public string JobName { set; get; } = "poop";

//    public void GenerateEmploymentVector(int emp, float[] weights) {
//        _mgr.Generate(emp, weights);
//    }

//    public int CurrentEmployment {
//        get { return 123; }
//    }

//    public int EmploymentDifference {
//        get { return 456; }
//    }

//    public int EmploymentTotal {
//        get { return 789; }
//    }

//    public int[] EmploymentVector {
//        set { _mgr.DataVector = value; }
//        get { return _mgr.DataVector;  }
//    }

//    // Debug functions
//    public void PrintDebugString() {
//        string outputString = GetDebugString();
//        Debug.Log(outputString);
//    }

//    public string GetDebugString() {
//        return "[EmploymentManager]: " + DistributionGen.Debug.HistToString(_mgr.DataVector);
//    }
//}
