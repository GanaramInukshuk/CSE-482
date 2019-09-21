//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//// Don't attach to a regular old GameObject (unless testing); instead attach to a UI element, empty
//// GameObject, or have it be a member of an even larger class (that's attached to a UI or empty GameObject)

//// Functionality
//// - This combines the functionality of the Housing, Household, Occupancy, and Population Managgers into one class
//// - There are also two counters for the number of residential buildings and the number of occupied living units;
////   these values are saved within the ResidentialSimulator; see the notes on dataflow for more information
//// - This class's generate function takes in up to four parameters: residential buildings, occupied living units,
////   a housing affector, and an array of affectors known as housing affectors; the affectors are used to generate
////   probabilities that the managers use; see dataflow notes for affectors; the affectors are technically optional;
////   not including them (there's an overloaded function that doesn't take affectors) will default to using the default
////   probabilities instead

//// NOTE: There are two implementations of this simulator: one where the HousingManager is skipped entirely and the
//// other where the HousingManager is implemented as a basic manager; this uses the HousingManager

//// The key difference is that using a basic manager, the only way to get larger housing complexes is by encouraging
//// construction of such (IE, changing the housing affector to a lower value); the counting manager would allow
//// for finer control but would disincentivize having residential buildings of differing sizes

//// Key:
//// _hsgCtr and _occCtr -> Housing and occupancy counters
//// _hsgMgr, _hhdMgr, _occMgr, and _popMgr -> Housing, household, occupancy, and population managers

//// General dataflow between each manager and counter:
//// _hsgCtr -> _hsgMgr -> _occCtr -> _hhdMgr -> _occMgr -> _popMgr

//// General dataflow for affectors:
//// - The WeightAffector helper class consists of four helper functions
//// - The first function takes in a housing affector and generates a housing distribution for the housing manager
//// - The last three functions take the same set of household affectors and each one generates a probability 
////   distribution for the household, occupancy, and population managers

//// Dataflow between each manager and counter: _hsgCtr -> _hsgMgr -> _occCtr -> _occMgr -> _popMgr
////  1. _hsgCtr.Count (the number of residential bldgs) goes into _hsgMgr to calculate the number of total living units
////  2. _hsgMgr.Occupancy (the maximum number of living units) goes into _occCtr as _occCtr.Max
////  3. _occCtr.Count (the number of occupied living units) goes into _hhdMgr
////  4. _hhdMgr.Count (equivalent to the number of occupied living units) goes into _occMgr;
////     _hddMgr.OccupancyAverage is used to generate weights for _occMgr
////  4. _occMgr.Population goes into _popMgr to calculate a population
////  5. _popMgr generates a population breakdown for use with other simulators/managers
//// In other words, _hsgMgr.Occupancy != _occMgr.OccupancyTotal; instead, _hsgMgr.Occupancy == _occCtr.Max 
//// and _occCtr.Count == _occMgr.OccupancyTotal
//// Also, try not to confuse the HousingManager and HouseholdManager, at least until I can find a better name

//public class ResidentialSimulator {

//    // The percentage of households that are simplexes, duplexes, triplexes, and fourplexes
//    // Translates to a housing affector of 8
//    // No source exists on this information so I made it up
//    private static float[] DefaultHousingWeights => new float[] { 0.875f, 0.109f, 0.014f, 0.002f };

//    // The percentage of households by type 
//    // Source: https://www.census.gov/prod/cen2010/briefs/c2010br-14.pdf (table 4, first row)
//    private static float[] DefaultHouseholdWeights => new float[] { 0.17f, 0.07f, 0.30f, 0.35f, 0.11f };

//    // The percentage of households that have 1..10 persons living within
//    // Source: https://www.statista.com/statistics/242189/disitribution-of-households-in-the-us-by-household-size/
//    //private static float[] DefaultOccupancyWeights => new float[] { .2700f, .3400f, .1600f, .1400f, .0600f, .0200f, .0075f, .0019f, .0005f, .0001f };       // From actual census data
//    private static float[] DefaultOccupancyWeights => new float[] { .2274f, .3533f, .1592f, .1208f, .0746f, .0398f, .0157f, .0063f, .0022f, .0007f };       // From using hh affectors of { 1, 1, 1, 1, 1 }

//    // The percentage of the population being within a specific age group (0-4..95-99)
//    // Translates to a household affector of { 1, 1, 1, 1, 1 }
//    // Source: https://www.populationpyramid.net/united-states-of-america/2010/
//    private static float[] DefaultPopulationWeights => new float[] { .0655f, .0651f, .0671f, .0701f, .0701f, .0691f, .0651f, .0661f, .0691f, .0741f, .0721f, .0641f, .0541f, .0401f, .0301f, .0230f, .0190f, .0110f, .0040f, .0011f };

//    // This is a helper class that takes the affectors and generates probability arrays for each of the managers
//    // This class is just as self-contained as the other classes
//    // The following affectors affect the following:
//    // - Housing affector: changing this changes the distribution of simplexes, duplexes, triplexes, and fourplexes
//    // - Household affectors: this consists of five values that represent a different household type: singles, cohabs,
//    //   pairs, families, and seniors; changing any one of them changes the proportion of that household type; see
//    //   comments for more info
//    private static class WeightAffector {

//        // The average household size for each hh type
//        // Used to calculate household, occupancy, and population distributions
//        // Source: https://www.census.gov/prod/cen2010/briefs/c2010br-14.pdf (table 4, first row)
//        // Household type and averages:
//        // - Singles: one person, self-explanatory
//        // - Cohabs: two or more persons per household living together for any reason other than marriage or family;
//        //   avg should around the same as families or less
//        // - Pairs: families with a household size of 2 (not necessarily a married couple without children; may include
//        //   single-parents with one child)
//        // - Families: families with a minimum household size of 3, average of 3.2 because the Poisson function is weird
//        // - Seniors: one or two senior citizens living together, average between 1 and 2
//        // Notes: The household types were chosen based on the data inferred from census data; however, family households
//        // were split into groups of hh size 2 and 3+ to better simulate hh sizes
//        private static float[] HouseholdSizeAverages => new float[] { 1.00f, 3.20f, 2.00f, 3.20f, 1.20f };

//        // The percentage of how much each household type contributes to each age group in the population
//        // Used to calculate population distribution
//        // This is a 2D array; each row represents the five hh types; the columns are for each of the 20 age buckets
//        // and each row contributes a certain percentage to each age group:
//        // - Singles: This contributes to all adult age groups, but moreso the young-adult age group; also contributes
//        //   a small amount to the teen age group
//        // - Cohabs: Contributes to all adult age groups, but tapers off with older age groups
//        // - Pairs: Contributes to all age groups but doesn't taper off as much compared to cohabs
//        // - Families: Sole contributor to the infant, child, and teen age groups; also contributes a small amount
//        //   to the senior population; contributes more to the adult and middle-age age groups
//        // - Seniors: Main contributor to the senior age group
//        private static float[][] HouseholdContributorWeights => new float[][] {
//            //            INFANT     CHILD      TEEN              YOUNG-ADULT       ADULT                    MIDDLE-AGE                      SENIOR
//            new float[] { 0.00f,     0.00f,     0.00f, 0.03f,     0.35f, 0.31f,     0.27f, 0.23f, 0.19f,     0.15f, 0.11f, 0.07f, 0.03f,     0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f },
//            new float[] { 0.00f,     0.00f,     0.00f, 0.03f,     0.16f, 0.14f,     0.12f, 0.10f, 0.08f,     0.06f, 0.04f, 0.02f, 0.00f,     0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f },
//            new float[] { 0.00f,     0.00f,     0.00f, 0.02f,     0.18f, 0.20f,     0.22f, 0.24f, 0.26f,     0.36f, 0.46f, 0.56f, 0.66f,     0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f },
//            new float[] { 1.00f,     1.00f,     1.00f, 0.92f,     0.31f, 0.35f,     0.39f, 0.43f, 0.47f,     0.43f, 0.39f, 0.35f, 0.31f,     0.19f, 0.16f, 0.13f, 0.10f, 0.07f, 0.03f, 0.01f },
//            new float[] { 0.00f,     0.00f,     0.00f, 0.00f,     0.00f, 0.00f,     0.00f, 0.00f, 0.00f,     0.00f, 0.00f, 0.00f, 0.00f,     0.81f, 0.84f, 0.87f, 0.90f, 0.93f, 0.96f, 0.99f },
//        };

//        // Selectively zeroes out the certain values while leaving the values to use untouched
//        private static int[][] OccupancyCancellationValues => new int[][] {
//            new int[] { 1, 0, 0, 0, 0,     0, 0, 0, 0, 0 },     // Singles; the only hh size should be 1
//            new int[] { 0, 1, 1, 1, 1,     1, 0, 0, 0, 0 },     // Cohabs; hh sizes range from 2 to 6
//            new int[] { 0, 1, 0, 0, 0,     0, 0, 0, 0, 0 },     // Pairs; the only hh size should be 2
//            new int[] { 0, 0, 1, 1, 1,     1, 1, 1, 1, 1 },     // Families; hh sizes range from 3 to 10
//            new int[] { 1, 1, 1, 1, 0,     0, 0, 0, 0, 0 },     // Seniors; hh sizes range from 1 to 4
//        };

//        // Affector function for housing; this is just a geometric progression
//        // (1/n^0, 1/n^1, 1/n^2, 1/n^3) reconciled into probabilities; the only
//        // parameter to change is n; recommended range 1-100, default is 8
//        public static float[] GenerateHousingWeights(float n) {
//            int length = Probabilities.HousingWeights.Length;
//            float[] weights = ExtraMath.General.Geometric(n, length);
//            return DistributionGen.Probability.Reconcile(weights);
//        }

//        // Affector function for household types
//        public static float[] GenerateHouseholdWeights(float[] householdAffectors) {
//            float[] returnValues = ExtraMath.Linear.AlignedVectorProduct(DefaultHouseholdWeights, householdAffectors);
//            return DistributionGen.Probability.Reconcile(returnValues);
//        }

//        // Affector function for occupancy; this is four Poisson distributions added together like a weighted average
//        // Household weights should be pre-generated by the previous generate function, or try to regenerate them?
//        // STEPS:
//        // 1. Generate a matrix where each row vector is a Poisson distribution for a household type and the column
//        //    vector count matches that of the default Poisson distribution
//        // 2. Use the cancellation matrix to selectively zero out some of those values, then reconcile
//        // 3. Multiply each row vector with a corresponding household weight
//        // 4. Add each row vector together into the final distribution
//        public static float[] GenerateOccupancyWeights(float[] householdAffectors) {
//            // STEP 0: Generate the household weights and get the size of the matrix
//            // The cancellation matrix's dimensions should match that of the Poisson matrix
//            float[] householdWeights = GenerateHouseholdWeights(householdAffectors);
//            int matrixRows = OccupancyCancellationValues.Length;
//            int matrixCols = OccupancyCancellationValues[0].Length;
//            float[][] poissonMatrix    = new float[matrixRows][];

//            float[]   hhSizeAvgs       = HouseholdSizeAverages;             // FOR STEP 1
//            int  [][] cancelMatrix     = OccupancyCancellationValues;       // FOR STEP 2
//            float[]   occupancyWeights = new float[matrixCols];             // FOR STEP 4
//            for (int i = 0; i < matrixRows; i++) {
//                poissonMatrix[i] = DistributionGen.Function.PoissonZero(hhSizeAvgs[i], 1, matrixCols);              // STEP 1
//                poissonMatrix[i] = ExtraMath.Linear.AlignedVectorProduct(poissonMatrix[i], cancelMatrix[i]);        // STEP 2a
//                poissonMatrix[i] = DistributionGen.Probability.Reconcile(poissonMatrix[i]);                         // STEP 2b
//                poissonMatrix[i] = ExtraMath.Linear.ScalarVectorMult(householdWeights[i], poissonMatrix[i]);        // STEP 3
//                occupancyWeights = ExtraMath.Linear.VectorSum(occupancyWeights, poissonMatrix[i]);                  // STEP 4
//            }
//            return occupancyWeights;
//        }

//        // Affector function for population; this is also a bunch of linear algebra
//        // STEPS:
//        // 1. Create a copy of the contribution matrix; have each row of that matrix be the scalar-vector product of the contributor
//        //    weights and its corresponding row vector
//        // 2. Add all the row vectors of the multiplied contribution matrix together
//        // 3. Find the aligned vector product (see ExtraMath) of the default population weights and the vector sum
//        // 4. Reconcile the AVP so that its probabilities add up to 1
//        public static float[] GeneratePopulationWeights(float[] householdAffectors) {
//            int numAffectors = DefaultHouseholdWeights.Length;     // The number of household affectors; should be the same as HouseholdWeights.Length
//            float[][] c1 = HouseholdContributorWeights;
//            float[] pop1 = new float[DefaultPopulationWeights.Length];

//            int numPopBuckets = DefaultPopulationWeights.Length;     // The number of population "buckets"; should be 20
//            for (int i = 0; i < numAffectors; i++) {
//                c1[i] = ExtraMath.Linear.ScalarVectorMult(householdAffectors[i], c1[i]);       // STEP 1
//                pop1  = ExtraMath.Linear.VectorSum(pop1, c1[i]);                                // STEP 2
//            }
//            pop1 = ExtraMath.Linear.AlignedVectorProduct(DefaultPopulationWeights, pop1);       // STEP 3
//            return DistributionGen.Probability.Reconcile(pop1);                                 // STEP 4
//        }
//    }

//    // For use with savefiles and debugging
//    public struct ResidentialVectors {
//        public int[] housingVector;     // Represents bldgs that are simplexes, duplexes, triplexes, fourplexes
//        public int[] householdVector;   // Represents houses that are one of the five household types; basically another way arrange households by anything but household size
//        public int[] occupancyVector;   // Represents living units with 1..10 persons per living unit
//        public int[] populationVector;  // Represents population age groups of 0-4, 5-9, ... , 95-99 year ranges
//    }

//    // For use with getters
//    public struct ResidentialTotals {
//        public int housingTotal;        // Total number of residential buildings
//        public int occupancyTotal;      // Total number of (occupied) living units; this is equivalent to _hhdMgr.Total, so householdTotal is not included
//        public int populationTotal;     // Total number of people living within all living units
//    }

//    // A simplified version of the populationVector; corresponds with the PopulationManager's 
//    // getter for each population group
//    public struct PopulationBreakdown {
//        public int infantPopulation;
//        public int childPopulation;
//        public int teenPopulation;
//        public int youngAdultPopulation;
//        public int adultPopulation;
//        public int middleAgePopulation;
//        public int seniorPopulation;
//    }

//    // Managers
//    private readonly HousingManager    _hsgMgr = new HousingManager   (DefaultHousingWeights   .Length);
//    private readonly HouseholdManager  _hhdMgr = new HouseholdManager (DefaultHouseholdWeights .Length);
//    private readonly OccupancyManager  _occMgr = new OccupancyManager (DefaultOccupancyWeights .Length);
//    private readonly PopulationManager _popMgr = new PopulationManager(DefaultPopulationWeights.Length);

//    // Counters
//    private readonly Counter _hsgCtr = new Counter( );      // This doesn't need a (specified) max unless you wanna simulate construction
//    private readonly Counter _occCtr = new Counter(0);      // This max is initially zero until _hsgMgr generates an occupancy

//    // A simplified 0-param Generate function that uses stored counts and default weights
//    public void GenerateResidents() {
//        // Housing counter step
//        //_hsgCtr.Count = residentialBldgs;

//        // Housing manager step
//        float[] housingWeights = DefaultHousingWeights;
//        _hsgMgr.Generate(_hsgCtr.Count, housingWeights);
//        _occCtr.Max = _hsgMgr.Occupancy;

//        // Occupancy counter step
//        //if (occupiedLivingUnits == -1)     _occCtr.MaxOutCount();
//        //else if (occupiedLivingUnits >= 0) _occCtr.Count = Mathf.Min(occupiedLivingUnits, _occCtr.Max);
//        //else _occCtr.Count = 0;

//        // Household manager step
//        float[] householdWeights = DefaultHouseholdWeights;
//        _hhdMgr.Generate(_occCtr.Count, householdWeights);

//        // Occupancy manager step
//        float[] occupancyWeights = DefaultOccupancyWeights;
//        _occMgr.Generate(_hhdMgr.Total, occupancyWeights);

//        // Population manager step
//        float[] populationWeights = DefaultPopulationWeights;
//        _popMgr.Generate(_occMgr.Population, populationWeights);
//    }

//    // A simplified 2-param Generate function sets counters but uses default weights
//    public void GenerateResidents(int residentialBldgs, int occupiedLivingUnits) {
//        // Housing counter step
//        _hsgCtr.Count = residentialBldgs;

//        // Housing manager step
//        float[] housingWeights = DefaultHousingWeights;
//        _hsgMgr.Generate(_hsgCtr.Count, housingWeights);
//        _occCtr.Max = _hsgMgr.Occupancy;

//        // Occupancy counter step
//        if (occupiedLivingUnits == -1) _occCtr.MaxOutCount();
//        else if (occupiedLivingUnits >= 0) _occCtr.Count = Mathf.Min(occupiedLivingUnits, _occCtr.Max);
//        else _occCtr.Count = 0;

//        // Household manager step
//        float[] householdWeights = DefaultHouseholdWeights;
//        _hhdMgr.Generate(_occCtr.Count, householdWeights);

//        // Occupancy manager step
//        float[] occupancyWeights = DefaultOccupancyWeights;
//        _occMgr.Generate(_hhdMgr.Total, occupancyWeights);

//        // Population manager step
//        float[] populationWeights = DefaultPopulationWeights;
//        _popMgr.Generate(_occMgr.Population, populationWeights);
//    }

//    // A two-paramater Generate function that uses stored counts and affectors
//    // This should be the default for simulation
//    public void GenerateResidents(float housingAffector, float[] householdAffectors) {
//        // Housing counter step
//        //_hsgCtr.Count = residentialBldgs;

//        // Housing manager step
//        float[] housingWeights = WeightAffector.GenerateHousingWeights(housingAffector);
//        _hsgMgr.Generate(_hsgCtr.Count, housingWeights);
//        _occCtr.Max = _hsgMgr.Occupancy;

//        // Occupancy counter step
//        //if (occupiedLivingUnits == -1)     _occCtr.MaxOutCount();
//        //else if (occupiedLivingUnits >= 0) _occCtr.Count = Mathf.Min(occupiedLivingUnits, _occCtr.Max);
//        //else _occCtr.Count = 0;

//        // Household manager step
//        float[] householdWeights = WeightAffector.GenerateHouseholdWeights(householdAffectors);
//        _hhdMgr.Generate(_occCtr.Count, householdWeights);

//        // Occupancy manager step
//        float[] occupancyWeights = WeightAffector.GenerateOccupancyWeights(householdAffectors);
//        _occMgr.Generate(_hhdMgr.Total, occupancyWeights);

//        // Population manager step
//        float[] populationWeights = WeightAffector.GeneratePopulationWeights(householdAffectors);
//        _popMgr.Generate(_occMgr.Population, populationWeights);
//    }

//    // A four-parameter Generate function that sets the counters accordingly and uses affectors
//    public void GenerateResidents(int residentialBldgs, int occupiedLivingUnits, float housingAffector, float[] householdAffectors) {
//        // Housing counter step
//        _hsgCtr.Count = residentialBldgs;

//        // Housing manager step
//        float[] housingWeights = WeightAffector.GenerateHousingWeights(housingAffector);
//        _hsgMgr.Generate(_hsgCtr.Count, housingWeights);
//        _occCtr.Max = _hsgMgr.Occupancy;

//        // Occupancy counter step
//        if (occupiedLivingUnits == -1) _occCtr.MaxOutCount();
//        else if (occupiedLivingUnits >= 0) _occCtr.Count = Mathf.Min(occupiedLivingUnits, _occCtr.Max);
//        else _occCtr.Count = 0;

//        // Household manager step
//        float[] householdWeights = WeightAffector.GenerateHouseholdWeights(householdAffectors);
//        _hhdMgr.Generate(_occCtr.Count, householdWeights);

//        // Occupancy manager step
//        float[] occupancyWeights = WeightAffector.GenerateOccupancyWeights(householdAffectors);
//        _occMgr.Generate(_hhdMgr.Total, occupancyWeights);

//        // Population manager step
//        float[] populationWeights = WeightAffector.GeneratePopulationWeights(householdAffectors);
//        _popMgr.Generate(_occMgr.Population, populationWeights);
//    }

//    // Incrementers; incrementing by a negative value counts as decrementing
//    public void IncrementHousing(int n) { _hsgCtr.IncrementCount(n); }
//    public void IncrementOccupancy(int n) { _occCtr.IncrementCount(n); }

//    // Note that this does not populate the counters with any values directly; instead, the
//    // managers populate the counters that preceed them
//    public ResidentialVectors Vectors {
//        set {
//            _hsgMgr.DataVector = value.housingVector;
//            _hhdMgr.DataVector = value.householdVector;
//            _occMgr.DataVector = value.occupancyVector;
//            _popMgr.DataVector = value.populationVector;
//            _hsgCtr.Count = _hsgMgr.Total;
//            _occCtr.Max   = _hhdMgr.Total;
//            _occCtr.Count = _occMgr.Total;
//        }
//        get {
//            return new ResidentialVectors {
//                housingVector    = _hsgMgr.DataVector,
//                householdVector  = _hhdMgr.DataVector,
//                occupancyVector  = _occMgr.DataVector,
//                populationVector = _popMgr.DataVector
//            };
//        }
//    }

//    // The only reason I'm allowing this to be its own getter is to bypass the need to
//    // call DistributionGen.Histogram.SumOfElements(); remember that _hhdMgr.Total
//    // and occupancyTotal are equivalent
//    public ResidentialTotals Totals {
//        get {
//            return new ResidentialTotals {
//                housingTotal    = _hsgMgr.Total,
//                occupancyTotal  = _hsgMgr.Occupancy,
//                populationTotal = _occMgr.Population
//            };
//        }
//    }

//    public PopulationBreakdown PopulationTotals {
//        get {
//            return new PopulationBreakdown {
//                infantPopulation     = _popMgr.InfantPopulation,
//                childPopulation      = _popMgr.ChildPopulation,
//                teenPopulation       = _popMgr.TeenPopulation,
//                youngAdultPopulation = _popMgr.YoungAdultPopulation,
//                adultPopulation      = _popMgr.AdultPopulation,
//                middleAgePopulation  = _popMgr.MiddleAgePopulation,
//                seniorPopulation     = _popMgr.SeniorPopulation
//            };
//        }
//    }

//    public void PrintDebugString() {
//        string outputString = GetDebugString();
//        Debug.Log(outputString);
//    }

//    public string GetDebugString() {
//        return "[ResidentialSimulator]: Hsg: "
//            + _hsgMgr.Total            + ", Occ: "
//            + _occCtr.Count            + " out of "
//            + _occCtr.Max              + ", Pop: "
//            + _popMgr.Total            + "\n"
//            + _hsgMgr.GetDebugString() + "\n"
//            + _hhdMgr.GetDebugString() + "\n"
//            + _occMgr.GetDebugString() + "\n"
//            + _popMgr.GetDebugString();
//    }

//    // One way to gauge the reliability of this model is to see whether the sum of
//    // seniors households and singles households is greater than the number
//    // of single-person households
//    public string SanityCheck() {
//        string sc = "[ResidentialSimulator]: Singles (HhdMgr vs OccMgr): "
//            + (_hhdMgr.DataVector[0] + _hhdMgr.DataVector[4]) + " " + _occMgr.DataVector[0]
//            + "; Multi-person households: " + DistributionGen.Histogram.SumOfElements(_hhdMgr.DataVector, 1, 4) + " " + DistributionGen.Histogram.SumOfElements(_occMgr.DataVector, 1, 9);
//        //+ " Pairs (HhdMgr vs OccMgr): " + (_hhdMgr.Vector[2] + _hhdMgr.Vector[4]) + " " + _occMgr.Vector[1];
//        return sc;
//    }
//}