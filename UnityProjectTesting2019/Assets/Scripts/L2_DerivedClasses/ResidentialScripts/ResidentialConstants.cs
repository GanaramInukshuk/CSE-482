using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is a place to offload constants needed for the residential simulator and its classes
// Most of these are probabilities based on real-life statistics; links provided

// This also includes counts necssary for functionality of very important counters

// Note: if any of the managers needed for calculation ever spit out a zero vector, that may be due
// to a weight vector size mismatch, IE, using the wrong default weights for a manager (EG, using
// the default pop weights for the occupancy manager)

namespace ResidentialScripts {

    public static class Constants {
        // Enum for housing sizes
        // May include 6plex, 8plex, 10plex, 12plex later on
        public enum HOUSINGSIZE { SIMPLEX, DUPLEX, TRIPLEX, FOURPLEX, SIXPLEX, /*EIGHTPLEX, TENPLEX, TWELVEPLEX*/ };

        // Array of housing sizes; these correspond to simplexes, duplex, triplexes, and fourplexes
        // For use with the housing counter
        public static int[] HousingSizes => new int[] { 1, 2, 3, 4, 6, 8 };

        // The percentage of households by type 
        // Source: https://www.census.gov/prod/cen2010/briefs/c2010br-14.pdf (table 4, first row)
        // Source: https://www.census.gov/content/dam/Census/library/visualizations/time-series/demo/families-and-households/hh-1.pdf (for families vs non-families vs presumably extended families)
        // For use with the HouseholdManager
        //public static float[] DefaultHouseholdWeights => new float[] { 0.17f, 0.09f, 0.23f, 0.25f, 0.17f, 0.09f };       // For 6-part affectors, based on actual census data
        public static float[] DefaultHouseholdWeights => new float[] { 0.20f, 0.10f, 0.20f, 0.30f, 0.10f, 0.10f };       // Rounded accordingly such that each is a multiple of 1/10; also better mimics census data on hh size

        // The percentage of households that have 1..10 persons living within
        // Source: https://www.statista.com/statistics/242189/disitribution-of-households-in-the-us-by-household-size/
        //private static float[] DefaultOccupancyWeights => new float[] { .2700f, .3400f, .1600f, .1400f, .0600f, .0200f, .0075f, .0019f, .0005f, .0001f };       // From actual census data
        // A good indicator of how accurate the simulated weights are is seeing whether 2-person and 3-person hh's make up a combined 50% of all hh's
        // For use with the OccupancyManager
        public static float[] DefaultOccupancyWeights => new float[] { .2624f, .3655f, .1364f, .1091f, .0697f, .0373f, .0135f, .0054f, .0005f, .0002f };       // From using default hh affectors   
    
        // The percentage of the population being within a specific age group (0-4..95-99) 
        // Translates to a household affector of { 1, 1, 1, 1, 1, 1 }
        // Source: https://www.populationpyramid.net/united-states-of-america/2010/
        // For use with the PopulationManager
        public static float[] DefaultPopulationWeights => new float[] { .0655f, .0651f, .0671f, .0701f, .0701f, .0691f, .0651f, .0661f, .0691f, .0741f, .0721f, .0641f, .0541f, .0401f, .0301f, .0230f, .0190f, .0110f, .0040f, .0011f };

        // Effective lengths of datavectors; for use with loops and savedata helper
        public static readonly int HousingVectorLength    = HousingSizes            .Length;
        public static readonly int HouseholdVectorLength  = DefaultHouseholdWeights .Length;
        public static readonly int OccupancyVectorLength  = DefaultOccupancyWeights .Length;
        public static readonly int PopulationVectorLength = DefaultPopulationWeights.Length;

        // For use with the savedata helper that handles jagged arrays
        public static int[] ExpectedVectorLengths => new int[] {
            HousingVectorLength     ,
            HouseholdVectorLength   ,
            OccupancyVectorLength   ,
            PopulationVectorLength,
        };
    }
}