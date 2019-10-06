using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a place for interfaces that can be reused between simulators
// EG consider the case of the HousingCounter and StoreCounter for the ResSim and CommSim; their
// interfaces can be generalized into one called IZonableBuilding from which the aforementioned
// counters can inherit from

namespace GeneralScripts {

    // For multicounters whose purpose is to count the number of buildings by size
    public interface IZonableBuilding {
        int this[int i]    { get; }     // Indexer
        int TotalBuildings { get; }     // Total number of buildings
        int MaxZoningUnits { get; }     // Total/max number of "zoning" units
    }


}