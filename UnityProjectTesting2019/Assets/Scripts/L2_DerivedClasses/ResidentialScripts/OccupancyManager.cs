using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Don't attach to a regular old GameObject; instead have it be a member of a larger class 
// that's attached to a UI or empty GameObject

// Functionality:
// - The population manager takes in a quantity of occupied houses and a housing distribution of number of people
//   per household and creates a histogram based on those probabilities; this histogram (occupancy vector) is
//   stored within the class for however long it's needed until a new occupancy vector is needed
// - The occupancy vector represents the number of houses that have 1, 2, ... , 12 persons; this can be thought
//   of as a Poisson distribution, where houses with 1-4 persons are common, but 5 and above are increasingly rare;
//   it should be extremely rare to have a 12-person household; since this can be modeled with a Poisson
//   distribution, a mean between 2 to 4 is recommended
// - Note: population and occupancy are two different values; occupancy is the number of houses that are occupied,
//   whereas population is the total number of people across all houses; a house could have six people living within
//   and be counted as one unit of occupancy and six units of population
// - Also note: the probability distribution can also be anything but a Poisson distribution, as long as it's a
//   sensible vector of probabilities to the BasicManager

namespace ResidentialScripts {
    // Note: this does not require a total occupancy since that's equivalent to HouseholdManager.TotalHouseholds
    public interface IOccupancy {
        int this[int i] { get; }
    }

    public sealed class OccupancyManager : BasicManager, IOccupancy {
        // For getting the population from the total occupancy
        // This is the occupancy translated into a population; the elements in the
        // array/vector correspond to household sizes of 1, 2, 3, and so on
        public int Population {
            get {
                int populationTotal = 0;
                int[] occupancyVector = DataVector;
                for (int i = 0; i < occupancyVector.Length; i++) populationTotal += (i+1) * occupancyVector[i];
                return populationTotal;
            }
        }

        // Constructor
        public OccupancyManager() : base(Constants.OccupancyVectorLength) { }

        // Overridden Generate() function adds randomness; randomness starts at an occupancy of 400
        public override void Generate(int n, float[] weights) {
            if (n > 0 && weights.Length == _vectorSize) {
                int randomAmt = (n > 400) ? 1 : 0;
                int[] v0 = DistributionGen.Histogram.GenerateByWeights(randomAmt, weights);
                int[] v1 = DistributionGen.Histogram.GenerateByRoundDown(n - randomAmt, weights);
                int disc = DistributionGen.Histogram.Discrepancy(n - randomAmt, v1);
                int[] v2 = DistributionGen.Histogram.GenerateByDescending(-disc, weights);
                v0 = DistributionGen.Histogram.Merge(v0, v1);
                v0 = DistributionGen.Histogram.Merge(v0, v2);
                DataVector = v0;
            } else DataVector = new int[_vectorSize];
        }

        // Extra Generate() function uses default weights
        public void Generate(int n) {
            Generate(n, Constants.DefaultOccupancyWeights);
        }

        // Overridden debug function
        public override string GetDebugString() {
            return "[OccupancyManager]: " + DistributionGen.Debug.HistToString(DataVector);
        }
    }
}