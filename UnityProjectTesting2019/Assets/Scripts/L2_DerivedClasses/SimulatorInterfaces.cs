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
    // This interface, provides basic zoning information (the number of buildings,
    // the number of occupants, and the max occupancy)
    public interface IZoningData {
        int[] BuildingVector { get; }
        int[] OccupantVector { get; }
        int TotalBuildings   { get; }
        int OccupantCount    { get; }
        int OccupantMax      { get; }
    }

    //// For use with the job-generating simulators
    //public interface ILaborUnits {
    //    int LaborUnitCount { get; }
    //    int LaborUnitMax   { get; }
    //}

    //// I'm not sure whether I need to use this...
    //public interface IEmployable {
    //    int EmploymentCount { get; }
    //    int EmploymentMax   { get; }
    //}

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