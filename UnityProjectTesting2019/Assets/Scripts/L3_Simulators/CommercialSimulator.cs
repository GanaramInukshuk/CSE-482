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
public class CommercialSimulator : ZoningSimulator, IZoningData {

    public static class Constants {

        // Enum for occupant types
        // Make sure the enums line up with the occupant weights listed below
        /// <summary>
        /// An enum that describes the occupant types available to the simulator.
        /// <para>Current types are: GROCERY, RETAIL, FOOD, SERVICE, AUTO</para>
        /// </summary>
        public enum OccupantType { GROCERY, RETAIL, FOOD, SERVICE, AUTO };

        // Occupant weights
        // In the context of commercial zoning, these represent the percentages at which a unit of employment
        // is of one of the following types:
        // - Grocery - Self-explanatory
        // - Retail - Retail stores (clothing, furniture, etc)
        // - Food - restaurants, including fast food
        // - Service - other commercial services, such as door-to-door services
        // - Automotive - gas stations and auto shops
        /// <summary>
        /// An array of probabilities that describes the probability that an occupant is one of the types
        /// described by the OccupantType enum.
        /// </summary>
        public static float[] OccupantWeights = { 0.20f, 0.10f, 0.20f, 0.30f, 0.10f, 0.10f };

        // Building sizes
        // Basically, units of employment are one-to-one with households, and each household contains at least
        // one employable person, so depending on household averages, one household should have more than one
        // employable person; if we take the optimistic average of 2 employable persons per household (which is
        // almost certainly the case with certain demographics), the smallest building size will employ 8 people
        // (that is, unless I change the first parameter in the ScalarVectorMult function)
        /// <summary>
        /// An array of building sizes available to the simulator.
        /// </summary>
        public static int[] BuildingSizes = ExtraMath.Linear.ScalarVectorMult(8, new int[] { 1, 2, 3, 4, 6, 8 });
    }

    // Constructor
    public CommercialSimulator() : base(Constants.BuildingSizes, Constants.OccupantWeights, 1 , "Commercial") { }

    // Member functions need not be overridden; their base functionality is plenty already
}
