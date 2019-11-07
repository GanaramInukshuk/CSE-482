//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//// Don't attach to a regular old GameObject; instead have it be a member of a larger class 
//// that's attached to a UI or empty GameObject

//// Functionality:
//// - The population manager takes in a population and a probability distribution for a population's age
////   groups and creates a histogram based on those probabilities; this histogram (population vector) is
////   stored within the class for however long it's needed until a new population vector is needed
//// - The GeneratePopulationVector function handles the main calculations; there are also additional getter
////   functions for each age group (infants, children, teens, young adults, adults, seniors)
//// - The population vector represents the population broken up into age groups, with each age group being
////   a 5-year range from 0-4, 5-9, all the way to 95-99; typically, each age group up to around 65-69
////   should have roughly the same amount of people, after which the amount of people per age group should
////   fall sharply; this closely mimics a populaton pyramid for a stable population (such as the US for
////   around the 2010s)

//// Note that this thing has TWO interfaces
//// - The first one is for the population pyramid and its other age-based subpopulations
//// - The second one is for demographics; for example, the workforce

//namespace ResidentialScripts {

//    public interface IPopulation {
//        //int this[int i]          { get; }
//        int TotalPopulation      { get; }
//        int InfantPopulation     { get; }
//        int ChildPopulation      { get; }
//        int Teen1Population      { get; }
//        int Teen2Population      { get; }
//        int YoungAdultPopulation { get; }
//        int AdultPopulation      { get; }
//        int MiddleAgePopulation  { get; }
//        int SeniorPopulation     { get; }
//        int[] PopulationVector   { get; }
//    }

//    public interface IDemographic {
//        int ElemSchoolDemographic { get; }
//        int MiddSchoolDemographic { get; }
//        int HighSchoolDemographic { get; }
//        int K12SchoolDemographic  { get; }
//        int EmployableDemographic { get; }
//        int RetiredDemographic    { get; }
//    }

//    public sealed class PopulationManager : BasicManager, IPopulation, IDemographic {
//        // Constructor
//        // Note that this manager (and its sister managers) use constants defined in the constants
//        // helper class; as long as all the contents of the namespace are intact, the managers may
//        // be used standalone; the only reason to combine them into simulators is because it's easier
//        // to handle this way
//        public PopulationManager() : base(Constants.PopulationVectorLength) { }

//        // Getters for each age group; these line up with the interface's members
//        // Infant:       0- 4, index 0
//        // Child:        5- 9, index 1
//        // Tween:       10-14, index 2
//        // Teen:        15-19, index 3
//        // Young adult: 20-29, index 4-5
//        // Adult:       30-44, index 6-8
//        // Middle age:  45-64, index 9-12
//        // Senior:      65-99, index 13-19 
//        public int InfantPopulation     => DataVector[0];
//        public int ChildPopulation      => DataVector[1];
//        public int Teen1Population      => DataVector[2];
//        public int Teen2Population      => DataVector[3];
//        public int YoungAdultPopulation => DistributionGen.Histogram.SumOfElements(DataVector,  4,  5);
//        public int AdultPopulation      => DistributionGen.Histogram.SumOfElements(DataVector,  6,  8);
//        public int MiddleAgePopulation  => DistributionGen.Histogram.SumOfElements(DataVector,  9, 12);
//        public int SeniorPopulation     => DistributionGen.Histogram.SumOfElements(DataVector, 13, 19);
//        public int TotalPopulation      => DistributionGen.Histogram.SumOfElements(DataVector);
//        public int[] PopulationVector   => DataVector;

//        // Getters for K12 demographics
//        public int ElemSchoolDemographic => Mathf.RoundToInt(ChildPopulation * 0.95f);
//        public int MiddSchoolDemographic => Mathf.RoundToInt(Teen1Population * 0.95f);
//        public int HighSchoolDemographic => Mathf.RoundToInt(Teen2Population * 0.95f);
//        public int K12SchoolDemographic  => ElemSchoolDemographic + MiddSchoolDemographic + HighSchoolDemographic;
        
//        // Getter for workforce
//        // Note that if secondary education is ever added, it's certainly possible for a college student to have a job
//        public int EmployableDemographic => Mathf.RoundToInt(
//            0.025f * Teen2Population      +
//            0.850f * YoungAdultPopulation +
//            0.850f * AdultPopulation      +
//            0.850f * MiddleAgePopulation  +
//            0.025f * SeniorPopulation
//        );

//        // Retired population consists mainly of the non-working seniors
//        public int RetiredDemographic => Mathf.RoundToInt(SeniorPopulation * 0.975f);


//        //Overridden Generate() function adds randomness; randomness starts at a population of 1000
//        public override void Generate(int n, float[] weights) {
//            if (n > 0 && weights.Length == _vectorSize) {
//                int randomAmt = (n > 1000) ? Mathf.CeilToInt(ExtraMath.General.Lg(n - 1000)) : 0;
//                int[] v0 = DistributionGen.Histogram.GenerateByWeights(randomAmt, weights);
//                int[] v1 = DistributionGen.Histogram.GenerateByRoundDown(n - randomAmt, weights);
//                int disc = DistributionGen.Histogram.Discrepancy(n - randomAmt, v1);
//                int[] v2 = DistributionGen.Histogram.GenerateByDescending(-disc, weights);
//                v0 = DistributionGen.Histogram.Merge(v0, v1);
//                v0 = DistributionGen.Histogram.Merge(v0, v2);
//                DataVector = v0;
//            } else DataVector = new int[_vectorSize];
//        }

//        // Extra Generate() function uses default weights
//        public void Generate(int n) {
//            Generate(n, Constants.DefaultPopulationWeights);
//        }

//        // Overridden debug function
//        public override string GetDebugString() {
//            return "[PopulationManager]: " + DistributionGen.Debug.HistToString(DataVector);
//        }
//    }
//}