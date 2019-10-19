using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResidentialScripts;       // Namespace for organizing classes
using GeneralScripts;

// Don't attach to a regular old GameObject; instead have it be a member of a larger class 
// that's attached to a UI or empty GameObject

// Functionality
// - This combines the Housing, Household, Occupancy, and Population Managers/Counters into one class
// - There is also an additional for the number of occupied living units; these values are saved within the 
//   ResidentialSimulator; see the notes on dataflow for more information
// - This class's generate function invokes the other managers's generate function; depending on whether the
//   main generate function has taken in any parameters, the individual generate function will either use
//   default weights or have different weights passed in instead

// Key:
// _hsgCtr and _occCtr -> Housing and occupancy counters
// _hhdMgr, _occMgr, and _popMgr -> Household, occupancy, and population managers

// General dataflow between each manager and counter:
// _hsgCtr -> _occCtr -> _hhdMgr -> _occMgr -> _popMgr

// General dataflow for affectors:
// - The WeightAffector helper class consists of three helper functions
// - Each function takes the household affectors and each one generates a probability distribution for the 
//   household, occupancy, and population managers

// Dataflow between each manager and counter: _hsgCtr -> _hhdMgr -> _occCtr -> _occMgr -> _popMgr
//  0. The Generate() function is called periodically (every in-game week). During this time, _hsgCtr and
//     _occCtr may be incremented by the user and by housing demand respectively
//  1. _hsgCtr.Occupancy (the number of living units) goes into _occCtr as _occCtr.Max
//  2. _occCtr.Count (the number of occupied living units) goes into _hhdMgr
//  3. _hhdMgr.Count (equivalent to the number of occupied living units) goes into _occMgr;
//     _hddMgr.OccupancyAverage is used to generate weights for _occMgr
//  4. _occMgr.Population goes into _popMgr to calculate a population
//  5. _popMgr generates a population breakdown for use with other simulators/managers

public class ResidentialSimulator {
    // This is a helper class that takes the affectors and generates probability arrays for each of the managers
    private static class WeightAffector {
        // Notes on household types and averages:
        // - Singles: one person, self-explanatory
        // - Cohabs: two or more persons per household living together for any reason other than marriage or family;
        //   avg should be around the same as families or less
        // - Couples: families with a household size of 2; typically a married couple with no children present or at all
        // - Families: families with a minimum household size of 3, average of ~3.2 because the Poisson function is weird
        // - Extended families: Same average as regular families but contains grandparent(s) or other family members
        // - Seniors: one or two senior citizens living together, average between 1 and 2 (maybe 3)

        // Notes on household contributions:
        // - Singles: This contributes to all adult age groups, but moreso the young-adult age group; also contributes
        //   a small amount to the teen age group
        // - Cohabs: Contributes to all adult age groups, but tapers off with older age groups
        // - Couples: Contributes to all age groups but doesn't taper off as much compared to cohabs; in fact, it increases
        //   with age level due to empty-nest families
        // - Families: Sole contributor to the infant, child, and teen age groups; also contributes a small amount
        //   to the senior population (extended families only); rises with age, peaks, and tapers down to simulate couples
        //   planning on having children, having children, then having the children move away and leaving an "empty nest"
        // - Seniors: Main contributor to the senior age group

        // The average household size for each hh type
        // Used to calculate household, occupancy, and population distributions
        // Source: https://www.census.gov/prod/cen2010/briefs/c2010br-14.pdf (table 4, first row)
        // Household type and averages:
        private static float[] HouseholdSizeAverages => new float[] { 1.00f, 3.20f, 2.00f, 3.20f, 3.20f, 1.20f };

        // The percentage of how much each household type contributes to each age group in the population
        // Used to calculate population distribution
        // This is a 2D array; each row represents the six hh types; the columns are for each of the 20 age buckets
        // and each row contributes a certain percentage to each age group
        // NOTE: These contributor weights are based on a 20:10:20:30:10:10 ratio of percentages, but each vector (if you can
        // call it that) is divided by that percentage (or multiplied by 1/percentage) so that every value is independent of
        // the original percentages; if affectors are to be used, the prescribed ratio should be 2:1:2:3:1:1, which translates
        // to a 20:10:20:30:10:10 percent ratio between each hh type; in comparison, using a ratio of 1:1:1:1:1:1 should 
        // produce a hh composition of equal parts of each type (16.67% of each hh type)
        private static float[][] HouseholdContributorWeights => new float[][] {
            new float[] {  0.000f,  0.000f,  0.000f,  0.000f,  2.000f,  1.778f,  1.556f,  1.333f,  1.111f,  0.889f,  0.667f,  0.444f,  0.222f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  },
            new float[] {  0.000f,  0.000f,  0.000f,  0.000f,  2.000f,  1.778f,  1.556f,  1.333f,  1.111f,  0.889f,  0.667f,  0.444f,  0.222f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  },
            new float[] {  0.000f,  0.000f,  0.000f,  0.000f,  0.120f,  0.282f,  0.444f,  0.607f,  0.769f,  1.274f,  1.778f,  2.282f,  2.786f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  },
            new float[] {  2.500f,  2.500f,  2.500f,  2.500f,  0.940f,  1.026f,  1.111f,  1.197f,  1.282f,  1.197f,  1.111f,  1.026f,  0.940f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  },
            new float[] {  2.500f,  2.500f,  2.500f,  2.500f,  0.940f,  1.026f,  1.111f,  1.197f,  1.282f,  1.197f,  1.111f,  1.026f,  0.940f,  1.900f,  1.600f,  1.300f,  1.000f,  0.700f,  0.400f,  0.100f,  },
            new float[] {  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  8.100f,  8.400f,  8.700f,  9.000f,  9.300f,  9.600f,  9.900f,  },
        };

        // Selectively zeroes out certain values while leaving the values to use untouched
        // This effectively limits the range of a Poisson distribution from [1, infinity] to
        // [1, some_max], including [1, 1]
        private static int[][] OccupancyCancellationValues => new int[][] {
            new int[] { 1, 0,    0, 0,   0, 0,   0, 0,   0, 0 },     // Singles; the only hh size should be 1
            new int[] { 0, 1,    1, 1,   1, 1,   0, 0,   0, 0 },     // Cohabs; hh sizes range from 2 to 6
            new int[] { 0, 1,    0, 0,   0, 0,   0, 0,   0, 0 },     // Couples; the only hh size should be 2
            new int[] { 0, 1,    1, 1,   1, 1,   1, 1,   0, 0 },     // Families; hh sizes range from 2 to 8
            new int[] { 0, 1,    1, 1,   1, 1,   1, 1,   1, 1 },     // Extendeds; hh sizes range from 2 to 10
            new int[] { 1, 1,    0, 0,   0, 0,   0, 0,   0, 0 },     // Seniors; hh sizes range from 1 to 2
        };

        // Affector function for household types
        // This takes a ratio of parts (think "a parts x, b parts y, c parts z" kind of parts) and generates a distribution
        public static float[] GenerateHouseholdWeights(float[] householdAffectors) {
            //float[] returnValues = ExtraMath.Linear.AlignedVectorProduct(Constants.DefaultHouseholdWeights, householdAffectors);
            return DistributionGen.Probability.Reconcile(householdAffectors);
        }

        // Affector function for occupancy; this is six Poisson distributions added together like a weighted average
        // Household weights should be pre-generated by the previous generate function, or try to regenerate them?
        // STEPS:
        // 1. Generate a matrix (implemented as a jagged array) where each row vector is a Poisson distribution
        //    for a household type and the column vector count matches that of the default Poisson distribution
        // 2. Use the cancellation matrix to selectively zero out some of those values, then reconcile
        // 3. Multiply each row vector with a corresponding household weight
        // 4. Add each row vector together into the final distribution
        public static float[] GenerateOccupancyWeights(float[] householdAffectors) {
            // STEP 0: Generate the household weights and get the size of the matrix
            // The cancellation matrix's dimensions should match that of the Poisson matrix
            float[] householdWeights = GenerateHouseholdWeights(householdAffectors);
            int hhTypes     = OccupancyCancellationValues.Length;       // Number of household types
            int poissonSize = OccupancyCancellationValues[0].Length;    // Length of Poisson vector
            float[][] poissonVectors = new float[hhTypes][];

            float[]   hhSizeAvgs       = HouseholdSizeAverages;             // FOR STEP 1
            int  [][] cancelVectors    = OccupancyCancellationValues;       // FOR STEP 2
            float[]   occupancyWeights = new float[poissonSize];            // FOR STEP 4
            for (int i = 0; i < hhTypes; i++) {
                poissonVectors[i] = DistributionGen.Function.PoissonZero(hhSizeAvgs[i], 1, poissonSize);            // STEP 1
                poissonVectors[i] = ExtraMath.Linear.AlignedVectorProduct(poissonVectors[i], cancelVectors[i]);     // STEP 2a
                poissonVectors[i] = DistributionGen.Probability.Reconcile(poissonVectors[i]);                       // STEP 2b
                poissonVectors[i] = ExtraMath.Linear.ScalarVectorMult(householdWeights[i], poissonVectors[i]);      // STEP 3
                occupancyWeights  = ExtraMath.Linear.VectorSum(occupancyWeights, poissonVectors[i]);                // STEP 4
            }
            return occupancyWeights;
        }

        // Affector function for population; this is also a bunch of linear algebra
        // STEPS:
        // 1a. Create a copy of the contribution matrix; if the contribution matrix is based on the values in the spreadsheet
        //     "affectors_6part_corrected", have each row of that matrix be the scalar-vector product of the contributor
        //     weights and its corresponding row vector and skip step 1b
        // 1b. If the contribution matrix is based on the valuues in the spreadsheet "affectors_6part_proportion", replace each column
        //     vector in the matrix with the scalar-vector product of the column vector and its corresponding household weight,
        //     disregarding step 1a
        // 2. Add all the row vectors of the multiplied contribution matrix together
        // 3. Find the aligned vector product (see ExtraMath) of the default population weights and the vector sum
        // 4. Reconcile the AVP so that its probabilities add up to 1
        public static float[] GeneratePopulationWeights(float[] householdAffectors) {
            int numAffectors = Constants.DefaultHouseholdWeights.Length;        // The number of household affectors; should be the same as HouseholdWeights.Length
            float[][] c1 = HouseholdContributorWeights;
            float[] pop1 = new float[Constants.DefaultPopulationWeights.Length];
            float[] householdWeights = GenerateHouseholdWeights(householdAffectors);    // For step 1b

            int numPopBuckets = Constants.DefaultPopulationWeights.Length;      // The number of population "buckets"; should be 20
            for (int i = 0; i < numAffectors; i++) {
                //c1[i] = ExtraMath.Linear.ScalarVectorMult(householdAffectors[i], c1[i]);        // STEP 1a
                c1[i] = ExtraMath.Linear.ScalarVectorMult(householdWeights[i], c1[i]);          // STEP 1b
                pop1  = ExtraMath.Linear.VectorSum(pop1, c1[i]);                                // STEP 2
            }
            pop1 = ExtraMath.Linear.AlignedVectorProduct(Constants.DefaultPopulationWeights, pop1);       // STEP 3
            return DistributionGen.Probability.Reconcile(pop1);                                 // STEP 4
        }
    }

    // Managers and counters
    private readonly HousingCounter    _hsgCtr = new HousingCounter   ( );
    private readonly Counter           _occCtr = new Counter          (0);      // This ensures the Counter.Max starts at zero and not int.MaxValue
    private readonly HouseholdManager  _hhdMgr = new HouseholdManager ( );
    private readonly OccupancyManager  _occMgr = new OccupancyManager ( );
    private readonly PopulationManager _popMgr = new PopulationManager( );

    // Interface-related getters
    // These return an interface-implementing object
    public IZonableBuilding ZoningBreakdown     => _hsgCtr;
    public IHousehold       HouseholdBreakdown  => _hhdMgr;
    public IOccupancy       OccupancyBreakdown  => _occMgr;
    public IPopulation      PopulationBreakdown => _popMgr;

    // One unit of occupancy translates to 2.5 units of population on average (assuming default affectors)
    public int UnitCount => _occCtr.Count;
    public int UnitMax   => _occCtr.Max;

    // For use with savedata
    // Note that this populates the occupancy counter using the values from the housing counter (for
    // the max) and the household manager (for the count)
    public int[][] DataVector {
        set {
            int[][] tempVector = SavedataHelper.LoadMismatchedVector(value, Constants.ExpectedVectorLengths);
            _hsgCtr.Count      = tempVector[0];
            _hhdMgr.DataVector = tempVector[1];
            _occMgr.DataVector = tempVector[2];
            _popMgr.DataVector = tempVector[3];
            _occCtr.Max   = _hsgCtr.MaxZoningUnits;       // Total number of available living units
            _occCtr.Count = _hhdMgr.TotalHouseholds;    // Total number of occupied living units
        }
        get => new int[][] {
            _hsgCtr.Count     ,
            _hhdMgr.DataVector,
            _occMgr.DataVector,
            _popMgr.DataVector
        };
    }

    //// Constructors
    public ResidentialSimulator() { }
    public ResidentialSimulator(int[][] savedata) { DataVector = savedata; }

    //// If using this as a component, the start function is needed
    //// This ensures all the counts start at zero
    //public void Start() {
    //    //_occCtr.Max   = _hsgCtr.MaxOccupancy;       // Total number of available living units
    //    //_occCtr.Count = _hhdMgr.TotalHouseholds;    // Total number of occupied living units
    //}

    //// Indexer for the housing counter?
    //public int this[HOUSINGSIZE h] => _hsgCtr[(int)h];

    // A 4-param Generate function; sets bldg count, uses affectors, sets unit count, and increments unit count
    // For debugging
    // Order of operations:
    // - If applicable, set the building count
    // - If applicable, set the unit count; if this is -1, set this to the max; any other negative zeros it out
    // - If applicable, increment the unit count
    // - Call the ManagerGenerate function
    public void Generate(float[] affectors, int[] bldgs, int units, int incrementAmt) {
        _hsgCtr.Count = bldgs;
        _occCtr.Max   = _hsgCtr.MaxZoningUnits;
        _occCtr.Count = (units == -1) ? _hsgCtr.MaxZoningUnits : units;
        _occCtr.IncrementCount(incrementAmt);
        ManagerGenerate(affectors);
    }

    // A 2-param Generate function; uses affectors and increments unit count
    // This should be typical of in-depth gameplay
    public void Generate(float[] affectors, int incrementAmt) {
        _occCtr.Max   = _hsgCtr.MaxZoningUnits;
        _occCtr.IncrementCount(incrementAmt);
        ManagerGenerate(affectors);
    }

    // A 1-param Generate function; only increments unit count
    // This can be used instead of the 2-param version if in-depth simulation (IE, demographics) isn't needed
    public void Generate(int incrementAmt) {
        _occCtr.Max   = _hsgCtr.MaxZoningUnits;
        _occCtr.IncrementCount(incrementAmt);
        ManagerGenerate();
    }

    // A 0-param Generate function; only calls the Generate function
    // Also for testing purposes
    public void Generate() {
        _occCtr.Max   = _hsgCtr.MaxZoningUnits;
        ManagerGenerate();
    }

    // Incrementers; incrementing by a negative value counts as decrementing
    // IncrementBldgs() should be called (indirectly) by the player and represents construction (or demolition) of residential bldgs
    // IncrementUnits() may be called by a random event system that adds or removes occupancy units
    // Incrementing buildings updates the counts
    public void IncrementBldgs(int[] amt) { 
        _hsgCtr.IncrementCount(amt);
        _occCtr.Max = _hsgCtr.MaxZoningUnits;
    }

    public void IncrementBldgs(int amt, int index) {
        _hsgCtr.IncrementCount(amt, index);
        _occCtr.Max = _hsgCtr.MaxZoningUnits;
    }

    public void IncrementUnits(int amt) {
        _occCtr.IncrementCount(amt);
    }

    // Private helper function; generates weights using affectors and calls the individual Generate functions 
    private void ManagerGenerate(float[] affectors) {
        float[] householdWeights  = WeightAffector.GenerateHouseholdWeights (affectors);
        float[] occupancyWeights  = WeightAffector.GenerateOccupancyWeights (affectors);
        float[] populationWeights = WeightAffector.GeneratePopulationWeights(affectors);
        _hhdMgr.Generate(_occCtr.Count          , householdWeights );
        _occMgr.Generate(_hhdMgr.TotalHouseholds, occupancyWeights );
        _popMgr.Generate(_occMgr.Population     , populationWeights);
    }

    // Private helper function; calls the individual Generate functions 
    private void ManagerGenerate() {
        _hhdMgr.Generate(_occCtr.Count          );
        Debug.Log("_hhdMgr.TotalHouseholds = " + _hhdMgr.TotalHouseholds);
        _occMgr.Generate(_hhdMgr.TotalHouseholds);
        _popMgr.Generate(_occMgr.Population     );
    } 

    // Debug functions
    public void PrintDebugString() {
        string outputString = GetDebugString();
        Debug.Log(outputString);
    }

    public string GetDebugString() {
        return "[ResidentialSimulator]: Hsg: "
            + _hsgCtr.TotalBuildings   + ", Occ: "
            + _occCtr.Count            + " out of "
            + _occCtr.Max              + ", Pop: "
            + _popMgr.TotalPopulation  + "\n"
            + _hsgCtr.GetDebugString() + "\n"
            + _hhdMgr.GetDebugString() + "\n"
            + _occMgr.GetDebugString() + "\n"
            + _popMgr.GetDebugString();
    }
}