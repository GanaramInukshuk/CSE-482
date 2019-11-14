using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimulatorInterfaces;

// This is a refined version of the CommercialSimulator; basically the constituent classes are grouped into a
// base simulator that this class inherits from and a static class of constants is included within; this setup
// eliminates the overly complicated feature of having each constituent class be standalone. Additionally, this
// simulator coes bundled with all the constants and interfaces it requires, apart from the general interfaces.

/// <summary>
/// A specialized version of the ZoningSimulator designed to simulate commercial zoning.
/// </summary>
public class CommercialSimulator : ZoningSimulator, IZoningControls {

    // Constants
    // - OccupantType is an enum describing each of the occupant types
    // - Building sizes represents different density levels of a given zoning type
    // - Occupant weights represent the probabilities that an occupant is one of the types described below; note that
    //   these weights must, 1, add up to 1.0f, and 2, match with the enum described above
    /// <summary>
    /// A static class containing the simulator's accompanying constants. This contains:
    /// <para>An enum for each of the occupant types.</para>
    /// <para>An array of building sizes.</para>
    /// <para>An array of probabilities (weights) describing the probability that
    /// an occupant is one of the types described by the enum.</para>
    /// </summary>
    public static class Constants {
        public enum OccupantType { GROCERY, RETAIL, FOOD, SERVICE, AUTO };
        public static int  [] BuildingSizes   => ExtraMath.Linear.ScalarVectorMult(8, new int[] { 1, 2, 4, 8 });
        public static float[] OccupantWeights => new float[] { 0.20f, 0.10f, 0.20f, 0.30f, 0.10f, 0.10f };
    }

    // Getters for the constants listed above
    // This is in case a class needs to be fed these things directly, IE, cannot be accessed
    // via Simulator.Constants.BuildingSizes, for example
    public int   [] ConstBuildingSizes   => Constants.BuildingSizes  ;
    public float [] ConstOccupantWeights => Constants.OccupantWeights;
    public string[] ConstOccupantTypes   => System.Enum.GetNames(typeof(Constants.OccupantType));

    // Constructor
    public CommercialSimulator() : base(Constants.BuildingSizes, Constants.OccupantWeights, 1 , "COMMERCIAL") { }

    // Member functions need not be overridden; their base functionality is plenty already
}
