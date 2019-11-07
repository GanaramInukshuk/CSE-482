using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is what happens when you generalize a zoning simulator to its bare essentials
// Traditionally in city simulators, zoning is residential, commercial, and industrial (and maybe commercial offices either
// as a distinct zoning type or as a subset as commercial)

// Early city simulators did not have a concept of zoning density, but the later ones do (EG SC4 and C:S);
// basically a zoning type will be of one of three types (res, comm, ind) and of up to three densities (low,
// med, high); zoning densities generally refer to different size ranges of buildings with the smallest referring
// to small single-unit buildings (EG bungalows and duplexes for residential) and the largest referring to high-rises

// Note for using this with zoning density, for example:
// - Scenario 1: Only one zoning density level per zoning type - Just use one ZoningSimulator wherein the
//   indices of the BldgSizes correspond with different size levels
// - Scenario 2: Different zoning densities per zoning type - Use an additional ZoningSimulator for each
//   zoning density, then group those simulators into a greater class OR just account for the different zoning levels
//   by adding additional building sizes into the BldgSizes array that's passed in the constructor

// A zoning simulator consists of the following:
// - A multicounter to hold buildings of different occupant sizes
// - A counter to hold the number of occupants occupying the buildings and to hold the max number of occupants
// - A BasicManager (a class used to generate histograms out of probability distributions and a given amount of datapoints)
//   to generate basic information about those occupants
// - Constants or some reference to constants to, 1, account for building sizes, and B, hold the default distribution for
//   occupant types
// - PROBABLY an ID to help identify what kind of zoning simulator it is, maybe a string

/// <summary>
/// A barebones simulator for a type of zoning typical of a city simulator (usually residential, commercial, industrial, and maybe commercial office).
/// </summary>
//[CreateAssetMenu(fileName = "New Zoning Simulator", menuName = "Zoning Simulator")]
public class ZoningSimulator {

    /// <summary>
    /// A specialized ArrayCounter specifically for use with the ZoningSimulator.
    /// </summary>
    private class BuildingCounter : ArrayCounter {

        // Additional getter for reporting the total zoning capacity across all buildings
        public int OccupantMax => ExtraMath.Linear.DotProduct(Count, BldgSizes);

        // Internal copy of building sizes
        // The purpose of BldgSizes is to represent buildings of different occupancy amounts; for example,
        // { 1, 2, 4, 8 } means that there are four different building sizes with exponentially increasing
        // capacities; this example is more readily apparent if we're talking about houses, for instance,
        // the smallest size is a dwelling fit for a single family and all the other sizes are duplexes,
        // fourplexes, and eightplexes (an eightplex would be teetering on the edge of being a small
        // apartment complex)
        public int[] BldgSizes { private set; get; }

        // Constructor; sets BldgSizes and length for base constructor
        public BuildingCounter(int[] bldgSizes) : base(bldgSizes.Length) {
            BldgSizes = DistributionGen.Histogram.Reconcile(bldgSizes);    
        }

        public override string GetDebugString() {
            return
                "[BuildingCounter]: " + DistributionGen.Debug.HistToString(Count) +
                "Building Sizes: " + DistributionGen.Debug.HistToString(BldgSizes)
            ;
        }
    }
    
    // Specialized BasicManager for use with the ZoningSimulator; contains an extra setter-getter for the
    // default occupancy weights; constructor directly takes occupancy weight vector
    // Also contains a special constructor that auto-sets the base contructor and internal copy of
    // OccWeights; not strictly necessary but this also overrides/customizes the GetDebugString function
    /// <summary>
    /// A specialized BasicManager specifically for use with the ZoningSimulator.
    /// </summary>
    private class OccupantManager : BasicManager {

        // Internal copy of occupancy weights
        // The purpose of OccWeights is to represent a demographic breakdown; piggybacking on the residential
        // example from the BuildingCounter, households can either contain a single person living within, two
        // or more persons cohabitating within the same household, a standard family with children (or what have
        // you), or at least one senior citizen; the weights basically describe a probability at which one
        // houeshold will fall under one of those types
        public float[] OccWeights { private set; get; }

        // Total number of occupants within the OccupantManager
        // For use with reloading the count into the occupant counter when loading savedata
        public int TotalOccupants => DistributionGen.Histogram.SumOfElements(DataVector);

        // Constructor; sets OccWeights and length for base constructor
        // This also reconciles the occupant weights just in case they don't add up to 1, then saves them
        public OccupantManager(float[] occWeights) : base(occWeights.Length) {
            OccWeights = DistributionGen.Probability.Reconcile(occWeights);
        }

        // Generate function for using default weights
        public void Generate(int n) {
            Generate(n, OccWeights);
        }

        // Generate function for using different weights (IE, affected weights)
        public void Generate(int n, int differentWeights) {
            Generate(n, differentWeights);
        }

        public override string GetDebugString() {
            return
                "[OccupantManager]: Data: " + DistributionGen.Debug.HistToString(DataVector) +
                "Occupant weights: " + DistributionGen.Debug.ProbToString(OccWeights);
            ;
        }
    }

    // Counters and managers needed for functionality
    private readonly BuildingCounter _bldgCounter;
    private readonly Counter         _occCounter;
    private readonly OccupantManager _occManager;

    // For identifying the simulator in some meaningful way
    public int    ZoningID   { private set; get; }
    public string ZoningName { private set; get; }

    // For getting specific pieces of data from the zoning simulator
    public int[] BldgVector     => _bldgCounter.Count;
    public int[] OccupantVector => _occManager.DataVector;
    public int TotalBuildings => DistributionGen.Histogram.SumOfElements(_bldgCounter.Count);
    public int OccupantCount  => _occCounter.Count;
    public int OccupantMax    => _occCounter.Max;

    // For savedata
    public int[][] DataVector {
        set {
            _bldgCounter.Count = value[0];
            _occManager.DataVector = value[1];
            _occCounter.Max = _bldgCounter.OccupantMax;
            _occCounter.Count = _occManager.TotalOccupants;
        }
        get => new int[][] {
            _bldgCounter.Count,
            _occManager.DataVector
        };
    }

    // Constructor; use this if the settings are passed in as regular parameters
    public ZoningSimulator(int[] bldgSizes, float[] occWeights, int zoningID = -1, string zoningName = "unnamed_zoning") {
        ZoningID   = zoningID;
        ZoningName = zoningName;
        _bldgCounter = new BuildingCounter(bldgSizes);
        _occCounter  = new Counter(0);
        _occManager  = new OccupantManager(occWeights);
    }

    //// Constructor; this accepts a ZoningSimulatorSettings object that contains all the data needed
    //// for a zoning simulator
    //public ZoningSimulator(ZoningSimulatorSettings z) {
    //    ZoningID   = z.ZoningID;
    //    ZoningName = z.ZoningName;
    //    _bldgCounter = new BuildingCounter(z.BldgSizes);
    //    _occManager  = new OccupantManager(z.OccWeights);
    //}

    //// Constructor; use this if the zoning simulator is itself a scriptable object
    //public ZoningSimulator() {
    //    _bldgCounter = new BuildingCounter(_bldgSizes);
    //    _occManager  = new OccupantManager(_occWeights);
    //}

    // Generate function that uses affectors and accepts an increment; default value for incrementAmt is zero,
    // effectively making that parameter optional
    public void Generate(float[] affectors, int incrementAmt = 0) {
        float[] affectedWeights = DistributionGen.Probability.Reconcile(affectors);
        _occCounter.Max = _bldgCounter.OccupantMax;
        _occCounter.IncrementCount(incrementAmt);
        _occManager.Generate(_occCounter.Count, affectedWeights);
    }

    // Generate function that only accepts an increment; this assumes default affectors (IE, occupancy weights);
    // default value for incrementAmt is zero, effectively making that parameter optional
    public void Generate(int incrementAmt = 0) {
        _occCounter.Max = _bldgCounter.OccupantMax;
        _occCounter.IncrementCount(incrementAmt);
        _occManager.Generate(_occCounter.Count);
    }

    // For incrementing multiple building types at once; not recommended for decrementing
    public void IncrementBldgs(int[] amt) { 
        _bldgCounter.IncrementCount(amt);
        _occCounter.Max = _bldgCounter.OccupantMax;
    }

    // For incrementing a specific building size
    public void IncrementBldgs(int amt, int index) {
        _bldgCounter.IncrementCount(amt, index);
        _occCounter.Max = _bldgCounter.OccupantMax;
    }

    // For incrementing the occupants independently of the Generate function
    public void IncrementOccupants(int amt) {
        _occCounter.IncrementCount(amt);
    }

    public void PrintDebugString() {
        string outputString = GetDebugString();
        Debug.Log(outputString);
    }

    public string GetDebugString() {
        return
            $"[ZoningSimulator]: ZID: {ZoningID} NAME: {ZoningName}; {_occCounter.Count} out of {_occCounter.Max} occupants.\n" +
            _bldgCounter.GetDebugString() + "\n" +
            _occManager .GetDebugString()
        ;
    }
}