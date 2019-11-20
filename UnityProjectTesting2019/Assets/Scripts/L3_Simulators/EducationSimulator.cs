using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimulatorInterfaces;

public class EducationSimulator : CivicSimulatorSimple, ICivicControls {

    /// <summary>
    /// A static class containing the simulator's accompanying constants. This contains:
    /// <para>An enum for each of the building types.</para>
    /// <para>An array for the number of seats per building type.</para>
    /// </summary>
    public static class Constants {
        public enum BuildingType { ELEMENTARY, MIDDLE, HIGH };
        public static int[] BuildingSeats = { 400, 600, 800 };
    }

    // For getting the simulator's constants
    public int   [] ConstBuildingSeats => Constants.BuildingSeats;
    public string[] ConstBuildingTypes => System.Enum.GetNames(typeof(Constants.BuildingType));

    // Constructor
    public EducationSimulator() : base(Constants.BuildingSeats, 0, "EDUCATION") { }

    // Member functions need not be overridden; their base functionality is plenty already
}