using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimulatorInterfaces;

public class HealthSimulator : CivicSimulatorSimple, ICivicControls {

    /// <summary>
    /// A static class containing the simulator's accompanying constants. This contains:
    /// <para>An enum for each of the building types.</para>
    /// <para>An array for the number of seats per building type.</para>
    /// </summary>
    public static class Constants {
        public enum BuildingType { CLINIC, HOSPITAL };
        public static int[] BuildingSeats = { 200, 600 };
    }

    // For getting the simulator's constants
    public int   [] ConstBuildingSeats => Constants.BuildingSeats;
    public string[] ConstBuildingTypes => System.Enum.GetNames(typeof(Constants.BuildingType));

    // Constructor
    public HealthSimulator() : base(Constants.BuildingSeats, 1, "HEALTH") { }

    // Member functions need not be overridden; their base functionality is plenty already
}
