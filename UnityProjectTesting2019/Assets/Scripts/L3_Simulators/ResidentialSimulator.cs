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

    /// <summary>
    /// Constants specific to the ResidentialSimulator.
    /// </summary>
    public static class Constants {

        // Enum for occupant types
        // Make sure the enums line up with the occupant weights listed below
        /// <summary>
        /// An enum that describes the occupant types available to the simulator.
        /// <para>Current types are: SINGLE, COHAB, COUPLE, FAMILY, EXTENDED, SENIOR</para>
        /// </summary>
        public enum OccupantType { SINGLE, COHAB, COUPLE, FAMILY, EXTENDED, SENIOR };

        // Occupant weights
        // In the context of residential zoning, these represent the percentages at which a household
        // is of one of the following types:
        // - Single   - a household with a single occupant (strictly one occupant who is not a senior citizen)
        // - Cohab    - a household with 2 or more unrelated persons living within (EG, college roommates)
        // - Couple   - ...a married couple (strictly two occupants and may be classified as a childless family)
        // - Family   - ...a family, usually nuclear but may include other types (EG stepfamily, single-parent, etc)
        // - Extended - ...a family with at least one outside family member, usually a grandparent
        // - Senior   - ...one or two senior citizens living within
        // Note that these probabilities must sum up to 1.00f
        /// <summary>
        /// An array of probabilities that describes the probability that an occupant is one of the types
        /// described by the OccupantType enum.
        /// </summary>
        public static float[] OccupantWeights = { 0.20f, 0.10f, 0.20f, 0.30f, 0.10f, 0.10f };

        // Building sizes
        // Basically, these go up from bungalows to duplexes to fourplexes, all the way to what could be considered
        // as a small apartment complex; I'm pretty sure my hometown has at least one duplex and I know for certain
        // there are small complexes that have 4 or more households; honestly, it's much more readily apparent if
        // your hometown features a military base with military housing; most of those houses are fourplexes (at least
        // the ones I've seen)
        /// <summary>
        /// An array of building sizes available to the simulator.
        /// </summary>
        public static int[] BuildingSizes = { 1, 2, 4, 8 };
    }

    // Constructor
    public ResidentialSimulator() : base(Constants.BuildingSizes, Constants.OccupantWeights, 0 , "Residential") { }

    // Member functions need not be overridden; their base functionality is plenty already
}