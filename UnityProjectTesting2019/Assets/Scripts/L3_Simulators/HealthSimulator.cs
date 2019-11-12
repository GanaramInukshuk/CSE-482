using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimulatorInterfaces;

public class HealthSimulator : CivicSimulatorSimple, ICivicControls {

    /// <summary>
    /// Constants specific to the EducationSimulator.
    /// </summary>
    public static class Constants {

        // Enum for building types
        public enum BuildingType { CLINIC, HOSPITAL };

        // Array for bicounter ratios
        // How many patients can each building type accommodate?
        public static int[] BuildingSeats = { 200, 1000 };
    }

    // Constructor
    public HealthSimulator() : base(Constants.BuildingSeats, 1, "Health") { }

    // Member functions need not be overridden; their base functionality is plenty already
}
