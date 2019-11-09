using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Following on the concept of a generalized zoning simulator, I've also generalized the civic simulator
// Traditionally in city simulators, civic services are hospitals, education, fire/police departments, and the like

// Early city simulators didn't have different building sizes either, but some do (either natively or as third-party
// addons); basically larger buildings are high-capacity versions of the originals that in the case of SC4 are unlockable
// at a certain threshold

// Dataflow between classes:
// ArrayCounter -> MultiCounter -> IntArray
// - ArrayCounter - This is an arraycounter that represents the number of buildings of each type
// - MultiCounter - This is a second arraycounter whose max is the counts of the ArrayCounter multiplied by the bicounter ratios;
//   as defined by the original Bicounter class, think of the ArrayCounter as the number of schools and the MultiCounter as the number
//   of seats across all schools; if the ArrayCounter reprots 3 schools and each school has a 1200 seat capacity, then the MultiCounter
//   will allow the user to specify a set number of seats available from 0 to 3600; this mimics how schools work in SC4 except it
//   affects every school at once, not individual buildings.
// - IntArray - This is simply an int[] that counts how many persons are being served by the civic simulator

public class CivicSimulatorSimple {

    // Counters
    private ArrayCounter _buildingCounter;
    private MultiCounter _seatCounter;

    // An internal copy of the bicounter ratios, or the number of "seats" per building, for each building type
    private int[] _buildingSeats;

    // Setter/getter for the current capacity of each building type, IE, how many people
    // are currently being served for each building type
    public int[] CurrentCapacity { set; get; }

    // Setters and getters for identifying the civic simulator
    public int    CivicID   { private set; get; }
    public string CivicName { private set; get; }

    // Getters for individual bits of data
    public int[] BuildingVector => _buildingCounter.Count;
    public int[] SeatVector => _seatCounter.Count;

    // For savedata
    public int[][] DataVector {
        set {
            // value[0] is the counts in the ArrayCounter
            // value[1] is the counts in the MultiCounter
            // value[2] is the CurrentCapacity
            _buildingCounter.Count = value[0];
            _seatCounter.Max   = ExtraMath.Linear.AlignedVectorProduct(_buildingCounter.Count, _buildingSeats);
            _seatCounter.Count = value[1];
            CurrentCapacity = value[2];
        }
        get {
            return new int[][] {
                _buildingCounter.Count,
                _seatCounter.Count,
                CurrentCapacity
            };
        }
    }

    // Constructor
    public CivicSimulatorSimple(int[] buildingSeats, int civicID = -1, string civicName = "unnamed_civic") {
        _buildingSeats = buildingSeats;
        _buildingCounter = new ArrayCounter(_buildingSeats.Length);
        _seatCounter = new MultiCounter(_buildingSeats.Length);
        CivicID   = civicID;
        CivicName = civicName;
        CurrentCapacity = new int[_buildingSeats.Length];
    }

    // This setup assumes that each building serves a different demographic each
    public void Generate(int[] persons) {

    }

    public void IncrementBuildings(int incrementAmt, int bldgType) {
        if (bldgType >= 0 || bldgType < _buildingSeats.Length) {
            _buildingCounter.IncrementCount(incrementAmt, bldgType);

            // Set the new max for the multicounter
            _seatCounter.IncrementMax(incrementAmt * _buildingSeats[bldgType], bldgType);

            // A more... overkill method
            //int[] newMax = ExtraMath.Linear.AlignedVectorProduct(_buildingCounter.Count, _bicounterRatios);
            //_seatCounter.Max = newMax;
        }
    }

    public void IncrementSeats(int incrementAmt, int bldgType) {
        if (bldgType >= 0 || bldgType < _buildingSeats.Length) {
            _seatCounter.IncrementCount(incrementAmt, bldgType);
        }
    }
}