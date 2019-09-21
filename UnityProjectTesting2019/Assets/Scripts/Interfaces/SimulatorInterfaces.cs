using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For use with simulators and not meant to be inherited by a counter or other base class

namespace SimulatorInterfaces {
    // For providing how much utilities a simulator uses per week
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

    // For any facilities that provide additional employment outside of commerce and industry
    // Think school jobs, hospital jobs, police jobs, etc
    public interface IEmployment {
        int EmploymentNeeded { get; }
    }
}