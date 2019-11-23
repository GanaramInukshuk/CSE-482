using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimulatorInterfaces;

// This is a refined version of the ResidentialSimulator; basically the constituent classes are grouped into a
// base simulator that this class inherits from and a static class of constants is included within; this setup
// eliminates the overly complicated feature of having each constituent class be standalone. Additionally, this
// simulator coes bundled with all the constants and interfaces it requires, apart from the general interfaces.

// This setup also eliminates the need to use the OccupancyManager and PopulationManager from the previous
// implementation as those parts are, 1, not really needed for a residential simulator, and 2, can be separated
// into a separate class.

// Note that the only interface these simulators should inherit from is the IZoningData interface; I've basically
// removed the need to have additional interfaces for these things; to access data for occupant types, do this:

// simulator.OccupantVector[(int)Simulator.Constants.OccupantType.TYPE]
// where Simulator is the name of the simulator and simulator is an instance of said simulator

/// <summary>
/// A specialized version of the ZoningSimulator designed to simulate residential zoning.
/// </summary>
public class ResidentialSimulator : ZoningSimulator, IZoningControls {

    // Constants
    // - OccupantType is an enum describing each of the occupant types
    // - Building sizes represents different density levels of a given zoning type
    // - Occupant weights represent the probabilities that an occupant is one of the types described below; note that
    //   these weights must, 1, add up to 1.0f, and 2, match with the enum described above
    public static class Constants {
        public enum OccupantType { SINGLE, COHAB, COUPLE, FAMILY, EXTENDED, SENIOR };
        public static int  [] BuildingSizes      => new int  [] { 1, 2, 4, 8 };
        public static float[] OccupantWeights    => new float[] { 0.20f, 0.10f, 0.20f, 0.30f, 0.10f, 0.10f };
        //public static int  [] ConstructionCost   => ExtraMath.Linear.ScalarVectorMult(200, BuildingSizes);
        //public static int  [] DemolitionCost     => ExtraMath.Linear.ScalarVectorMultToInt(0.5f, ConstructionCost);
        //public static int     RevenuePerOccupant => 8;
    }

    public string[] ConstOccupantTypes      => System.Enum.GetNames(typeof(Constants.OccupantType));
    public int   [] ConstBuildingSizes      => Constants.BuildingSizes;
    public float [] ConstOccupantWeights    => Constants.OccupantWeights;
    //public int   [] ConstConstructionCost   => Constants.ConstructionCost;
    //public int   [] ConstDemolitionCost     => Constants.DemolitionCost;
    //public int      ConstRevenuePerOccupant => Constants.RevenuePerOccupant;

    // Constructor; note that this also sets the zoning ID and zoning name
    public ResidentialSimulator() : base(Constants.BuildingSizes, Constants.OccupantWeights, 0, "RESIDENTIAL") { }

    // Member functions need not be overridden; their base functionality is plenty already
}