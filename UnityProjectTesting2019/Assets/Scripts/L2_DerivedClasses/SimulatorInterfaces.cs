using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For use with simulators and, to an extent, its constituent classes
// There are two types of simulators:
// - Zoning-related simulators (EG ResSim, CommSim)
// - Civic-related simulators (EG education, hospitals)

// In general, these simulators will inherit some combination of these interfaces; for example:
// - ZoningSimulators may inherit from IZoningSimulator and ITaxable
// - CivicSimulators may inherit from IUpkeep

namespace SimulatorInterfaces {
    // A general interface for zoning-related simulators; such interfaces require:
    // - A running total of all occupants across all buildings
    // - A setter/getter for the datavector
    // - A generate function that generates occupants using statistical models and input parameters
    // - Increment functions for adding/removing buildings
    // - An additional increment function for adding/removing occupants outside of the generate function
    // - Everything specified in the IZonableBuilding interface
    // Simulators may need to return additional interfaces but this is handled at a case-by-case basis
    // Note: A simulator's constituent classes inherit from different interfaces unique to that zoning type
    public interface IZoningSimulator : IZonableBuilding {
        int     OccupantCount { get; }
        int[]   BldgVector    { get; }
        int[][] DataVector    { set; get; }
        void Generate(float[] affectors, int[] bldgs, int occupants, int incrementAmt);
        void Generate(float[] affectors, int incrementAmt);
        void Generate(int incrementAmt);
        void Generate();
        void IncrementBldgs(int[] amt);
        void IncrementBldgs(int amt, int index);
        void IncrementOccupants(int amt);
    }

    // For use with the multicounters used for a zoning simulator (multicounters record the number
    // of buildings across all sizes) and actual zoning simulators
    public interface IZonableBuilding {
        int this[int i]    { get; }     // Indexer
        int TotalBuildings { get; }     // Total number of buildings
        int OccupantMax    { get; }     // Total/max number of "zoning" units
    }

    // For use with the job-generating simulators
    public interface ILaborUnits {
        int LaborUnitCount { get; }
        int LaborUnitMax   { get; }
    }

    public interface IEmployable {
        int EmploymentCount { get; }
        int EmploymentMax   { get; }
    }

    // For providing how much utilities a simulator uses per week
    // Utilities would be provided by utility companies (power plant, water pump, etc)
    public interface IUtilities {
        int ElectricityUsage { get; }       // In kilowatt-hours
        int WaterUsage       { get; }       // In cubic meters
        int GasUsage         { get; }       // Also in cubic meters 
    }

    // For providing how much waste a simulator uses per week
    // Such wastes are handled by waste management services
    public interface IWaste {
        int SewageProduction  { get; }      // In cubic meters
        int GarbageProduction { get; }      // In metric tons
    }

    // For providing how much taxable revenue is produced by a simulator per week
    // This would be property tax, sales tax, export tax, etc; this is a generalized tax
    public interface ITaxable {
        int TaxRevenue { get; }
    }

    // For providing monetary upkeep per week
    // This would be for any simulator that doesn't provide any revenue and only has a cost
    public interface IUpkeep {
        int UpkeepCost { get; }
    }

    //// For any facilities that provide additional employment outside of commerce and industry
    //// Think school jobs, hospital jobs, police jobs, etc
    //public interface IEmployable {
    //    int EmploymentNeeded { get; }
    //}
}