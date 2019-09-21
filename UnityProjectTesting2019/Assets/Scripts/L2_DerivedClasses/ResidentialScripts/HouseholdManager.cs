using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Don't attach to a regular old GameObject; instead have it be a member of a larger class 
// that's attached to a UI or empty GameObject

// Functionality:
// - The household manager takes in a number of households and a probability distribution for what type of
//   household it is and creates a histogram based on those probabilities; this histogram (household vector) is
//   stored within the class for however long it's needed until a new population vector is needed
// - This also generates a housing occupancy average (the average number of people per household) that's
//   stored for however long it's needed

// There are six types of households the HouseholdManager keeps track of:
// - Singles: single non-senior persons living in a household
// - Cohabs: two or more people living together but are not family
// - Couples: married couples with no children present or at all
// - Families: self-explanatory
// - Extendeds: extended families; these may have a senior citizen (grandparent) as part of the family
// - Seniors: persons over the age of 65 either living alone or with a partner

namespace ResidentialScripts {

    public interface IHousehold {
        int this[int i]        { get; }
        int SingleHouseholds   { get; }
        int CohabHouseholds    { get; }
        int CoupleHouseholds   { get; }
        int FamilyHouseholds   { get; }
        int ExtendedHouseholds { get; }
        int SeniorHouseholds   { get; }
        int TotalHouseholds    { get; }
    }

    public sealed class HouseholdManager : BasicManager, IHousehold {
        // Getters for each household type; these line up with the interface's getters
        public int SingleHouseholds   => DataVector[0];
        public int CohabHouseholds    => DataVector[1];
        public int CoupleHouseholds   => DataVector[2];
        public int FamilyHouseholds   => DataVector[3];
        public int ExtendedHouseholds => DataVector[4];
        public int SeniorHouseholds   => DataVector[5];
        public int TotalHouseholds    => DistributionGen.Histogram.SumOfElements(DataVector);

        // Constructor
        public HouseholdManager() : base(Constants.HouseholdVectorLength) { }

        // Generate() function and Vector and Total setter/getter are defined in base class
        // Extra Generate() function uses default weights
        public void Generate(int n) {
            Generate(n, Constants.DefaultHouseholdWeights);
        }

        // Overridden debug function
        public override string GetDebugString() {
            return "[HouseholdManager]: " + DistributionGen.Debug.HistToString(DataVector);
        }
    }
}