using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommercialScripts;
using SimulatorInterfaces;

// Don't attach to a regular old GameObject; instead have it be a member of a larger class 
// that's attached to a UI or empty GameObject

// Functionality:
// - This combines the functionality of the following managers and counters: StoreCounter and EmploymentManager (an
//   additional counter is utilized and comes between the two in the dataflow)
// - This class holds a count for the number of commercial buildings in a city (which collectively produce a maximum
//   employment capacity) and a count of the number of able workers in the city

// Dataflow between counters and managers (new one this time)
// 0. The Generate function is called periodically and the counters may be incremented/decremented in between callings
// 1. _storeCounter.MaxEmployment is used the max for _laborCtr (_laborCtr.Max) and represents the maximum labor units available
// 2. _laborCtr.Count is passed into the generate function for _empMgr
// 3. _empMgr generates a breakdown of employment specializations; this also converts labor units into an employment count

// As for an evaluator, I've considered having that built-in, but it can be a standalone class with the proposed functionaltiy:
// - A single workforce evaluator can handle more than one job category (IE, commercial, industrial, office); it can tabulate
//   the total employment capacity of all three fields, generate a probability distribution, and use the dist with the
//   total number of able workers to divvy up the working population into three subpopulations (AND WITH A BasicManager, TOO!!);
//   other factors apart from employment capacity may come into play, such as job desirability by field (EG, high education
//   yields office jobs and trade schools yield industrial jobs; commercial jobs may be worked by anyone)
// - If I'm still in the business of assigning fun names to things, then I'd propose "COIL" - Commercial, Office, and Industrial Labor
// - In the example of commercial jobs, the commercial subpopulation goes through the usual dataflow as outlined above; the
//   other two subpops would go through similar dataflows with their respective simulators
// - If demand allows for such, the evaulator can generate a new number for employment, more or less than the previous
//   amount depending on whether there is enough demand and unused commercial space OR there is too much demand; recall the
//   fact that the generate function takes in employment as a parameter

// Dataflow between counters and managers (also old)
// 0. The Generate function is called periodically and the counters may be incremented/decremented in between callings
// 1. _storeCtr.MaxEmploymentUnits is used as the max of a bicounter (_empBtr); the first count of the bicounter is the
//    number of available employment units and the second count is the number of able workers (passed in by the Generate
//    function); the ratio of the bicounter is the number of employees per employment unit, so one unit in the first count
//    translates to 8 units of the second count's max
// 2. _empBtr.Count2 is passed into the EmploymentManager to generate a breakdown of specializations

// Dataflow between counters and managers (old)
// 0. The Generate function is called periodically and the counters may be incremented/decremented in between callings
// 1. _storeCtr.MaxEmploymentUnits is used as the maximum for _occCtr (_occCtr.Max) and represents the maximum number of employment units
// 2. _occCtr.Count is passed into the generate function for _empMgr
// 3. _empMgr generates a breakdown of employment specializations for use with other simulators/managers

public class CommercialSimulator : IZoningSimulator {
    // This is a helper class that takes affectors (really, proportions) and generates a probability array for the EmploymentManager
    // This is not as complex compared to the ResidentialSimulator's affector class
    private static class WeightAffector {
        public static float[] GenerateEmploymentWeights(float[] commercialAffectors) {
            return DistributionGen.Probability.Reconcile(commercialAffectors);
        }
    }

    // Managers and counters
    private readonly StoreCounter      _storeCtr = new StoreCounter     ( );
    private readonly Counter           _laborCtr = new Counter          (0);
    private readonly EmploymentManager _empMgr   = new EmploymentManager( );

    // Interface-related getters
    public IEmployment EmploymentBreakdown => _empMgr;

    // Getters related to IZonableBuilding
    public int this[int i]    => _storeCtr[i];       // Indexer for bldg count by bldg size
    public int TotalBuildings => _storeCtr.TotalBuildings;

    // One unit of occupancy translates to 8 units of employment
    public int OccupantCount => _laborCtr.Count;
    public int OccupantMax   => _laborCtr.Max;        // Equivalent to _laborCtr.OccupantMax and required by IZonableBuilding

    // Bldg data getter
    public int[] BldgVector { get => _storeCtr.Count; }

    // Savedata setter-getter
    public int[][] DataVector {
        set {
            int[][] tempVector   = SavedataHelper.LoadMismatchedVector(value, Constants.ExpectedVectorLengths);
            _storeCtr.Count      = tempVector[0];
            _empMgr  .DataVector = tempVector[1];
            _laborCtr.Max   = _storeCtr.OccupantMax;     // Maximum employment capacity
            _laborCtr.Count = _empMgr  .TotalEmployment;    // Employment count
        }
        get => new int[][] {
            _storeCtr.Count     ,
            _empMgr  .DataVector,
        };
    }

    // Constructors
    public CommercialSimulator() { }
    public CommercialSimulator(int[][] savedata) { DataVector = savedata; }

    // A 4-param Generate function; sets bldg count, uses affectors, sets unit count, and increments unit count
    // For debugging
    // Order of operations:
    // - If applicable, set the building count; this is the number of buildings available, by type, organized in an array
    // - If applicable, set the unit count; if this is -1, set this to the max; any other negative zeros it out
    // - If applicable, increment the unit count
    // - Call the ManagerGenerate function
    public void Generate(float[] affectors, int[] bldgs, int units, int incrementAmt) {
        _storeCtr.Count = bldgs;
        _laborCtr.Max   = _storeCtr.OccupantMax;
        _laborCtr.Count = (units == -1) ? _storeCtr.OccupantMax : units;
        _laborCtr.IncrementCount(incrementAmt);
        ManagerGenerate(affectors);
    }

    // A 2-param Generate function; uses affectors and increments unit count
    // This should be typical of in-depth gameplay
    public void Generate(float[] affectors, int incrementAmt) {
        _laborCtr.IncrementCount(incrementAmt);
        ManagerGenerate(affectors);
    }

    // A 1-param Generate function; only increments unit count
    // This can be used instead of the 2-param version if in-depth simulation (IE, specialization) isn't needed
    public void Generate(int incrementAmt) {
        _laborCtr.IncrementCount(incrementAmt);
        ManagerGenerate();
    }

    // A 0-param Generate function; only calls the Generate function
    // Also for testing purposes
    public void Generate() {
        ManagerGenerate();
    }

    // Incrementers; incrementing by a negative value counts as decrementing
    // IncrementBldgs() should be called (indirectly) by the player and represents construction (or demolition) of residential bldgs
    // IncrementUnits() may be called by a random event system that adds or removes occupancy units
    // Incrementing buildings updates the counts
    public void IncrementBldgs(int[] amt) { 
        _storeCtr.IncrementCount(amt);
        _laborCtr.Max = _storeCtr.OccupantMax;
    }

    public void IncrementBldgs(int amt, int index) {
        _storeCtr.IncrementCount(amt, index);
        _laborCtr.Max = _storeCtr.OccupantMax;
    }

    public void IncrementOccupants(int amt) {
        _laborCtr.IncrementCount(amt);
    }

    // Private helper function; generates weights using affectors and calls the individual Generate functions 
    private void ManagerGenerate(float[] affectors) {
        float[] commercialWeights = WeightAffector.GenerateEmploymentWeights(affectors);
        _empMgr.Generate(_laborCtr.Count, commercialWeights);
    }

    // Priave helper function; calls the individual Generate functions 
    private void ManagerGenerate() {
        _empMgr.Generate(_laborCtr.Count);
    }

    // Debug functions
    public void PrintDebugString() {
        string outputString = GetDebugString();
        Debug.Log(outputString);
    }

    public string GetDebugString() {
        return "[CommercialSimulator]: Bldgs: "
            + _storeCtr.TotalBuildings   + ", Labor: "
            + _laborCtr.Count            + " out of "
            + _laborCtr.Max              + ", Emp: "
            + _empMgr.TotalEmployment    + "\n"
            + _storeCtr.GetDebugString() + "\n"
            + _empMgr.GetDebugString();
    }
}