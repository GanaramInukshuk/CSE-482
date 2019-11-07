using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Following on the concept of a generalized zoning simulator, I've also generalized the civic simulator
// Traditionally in city simulators, civic services are hospitals, education, fire/police departments, and the like

// Early city simulators didn't have different building sizes either, but some do (either natively or as third-party
// addons); basically larger buildings are high-capacity versions of the originals that in the case of SC4 are unlockable
// at a certain threshold

// Notes for using this with different building sizes (education, for example):
// - Scenario 1: No different school sizes per edu. level - Just use one CivicSimulator wherein the indices of the
//   bicounter corresponds with different levels of schooling
// - Scenario 2: Different school sizes exist per edu. level - One civic simulator per education level, then have those
//   simulators be a part of a greater class
// - Scenario 3: Schools are completely modular and consist of sub-buildings of fixed sizes - Same as scenario 2 where
//   for each education level, within the counter, each index refers to "base buildings" and expansion modules (EG,
//   portables, building wings, etc; portables have a count of one classroom, wings maybe 4-8 classrooms, etc)
// * - Scenarios 2 and 3 may reqire a different setup
public class CivicSimulatorSimple {

    // Bicounter array; each index of the array corresponds to a different tier of civic service, not a building size
    private readonly Bicounter[] _civicCounter;

    // Setters and getters for identifying the civic simulator
    public int    CivicID   { private set; get; }
    public string CivicName { private set; get; }

    // For savedata
    // The fact that 2.147 billion is a part of the savedata should be an indicator that this is working and that this setup
    // isn't exactly ideal for long-term use since I really wanted to use an arraycounter...
    public int[][] DataVector {
        set {
            for (int i = 0; i < value.Length; i++) {
                int[] subarray = { int.MaxValue, value[i][1], value[i][2] };
                _civicCounter[i].DataVector = subarray;
            }
        }
        get {
            int[][] returnVector = new int[_civicCounter.Length][];
            for (int i = 0; i < returnVector.Length; i++) returnVector[i] = _civicCounter[i].DataVector;
            return returnVector;
        }
    }

    // Constructor; the last two parameters are self-explanatory, but I need to explain the first parameter
    // The bicounter consists of two counters where the count of the first counter is the max of the second counter, times
    // some ratio; for example, SC4 had a feature where overall school funding can be tuned down to the student
    // level and this is something I'm more or less recreating; one counter counts off the number of schools available and
    // the other counts off the number of available seats, and the ratio represents how many units in Count2 are represented
    // by one unit in Count1
    // For example, if the bicounter reprots a Count1 of 1 and there are 1500 seats per school, Count2 of the bicounter
    // can count as high as 1500; Count2 can be tuned accordingly denpending on demand, and if more than 1500 students try
    // to attend, that means it's time to add another school and Count2 can then go as high as 3000
    public CivicSimulatorSimple(int[] bicounterRatios, int civicID = -1, string civicName = "unnamed_civic") {
        _civicCounter = new Bicounter[bicounterRatios.Length];
        for (int i = 0; i < bicounterRatios.Length; i++) _civicCounter[i] = new Bicounter(int.MaxValue, bicounterRatios[i]);
        CivicID   = civicID;
        CivicName = civicName;
    }

    public void IncrementBuildings(int level, int incrementAmt) {
        if (level >= 0 || level < _civicCounter.Length) _civicCounter[level].IncrementCount1(incrementAmt);
    }

    public void IncrementUnits(int level, int incrementAmt) {
        if (level >= 0 || level < _civicCounter.Length) _civicCounter[level].IncrementCount2(incrementAmt);
    }
}