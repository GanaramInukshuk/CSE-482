using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EducationSimulator : CivicSimulatorSimple {

    /// <summary>
    /// Constants specific to the EducationSimulator.
    /// </summary>
    public static class Constants {

        // Enum for building types
        public enum BuildingType { ELEMENTARY, MIDDLE, HIGH };

        // Array for bicounter ratios
        // How many persons does each building type accommodate?
        public static int[] BuildingSeats = { 1200, 1500, 1800 };
    }

    // Constructor
    public EducationSimulator() : base(Constants.BuildingSeats, 0, "Education") { }

    // Member functions need not be overridden; their base functionality is plenty already
}