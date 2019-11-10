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

// Notes on using this with different implementations (education, for example):
// - Scenario 1: No different sized schools per edu. level - use one simulator and have the Generate function
//   accept an int[] of students of different schooling levels
// - Scenario 2: Different sized schools per edu. level OR schools are modular and made from constituent
//   subbuildings of different sizes - use one simulator per education level and have each simulator be a part of
//   a larger simulator and have the Generate function accept an int that represents the students of only one
//   schooling level

public class CivicSimulatorSimple {

    // Counters
    private ArrayCounter _buildingCounter;
    private MultiCounter _seatCounter;

    // An internal copy of the bicounter ratios, or the number of "seats" per building, for each building type
    private int[] _buildingSeats;

    // Setters and getters for identifying the civic simulator
    public int    CivicID   { private set; get; }
    public string CivicName { private set; get; }

    // Getters for individual vectors of data
    public int[] BuildingVector => _buildingCounter.Count;
    public int[] SeatMaxVector => _seatCounter.Max;
    public int[] SeatCountVector => _seatCounter.Count;

    // Setter/getter for the current capacity of each building type, IE, how many people
    // are currently being served for each building type
    public int[] SeatsFilled { private set; get; }

    // Setter/getter for leftover capacity
    // A positive leftover capacity means that there are more seats than needed
    // A negative leftover capacity means that there is more "demand" than what can be satisfied
    public int[] SeatsLeft { private set; get; }

    // For savedata
    public int[][] DataVector {
        set {
            // value[0] is the counts in the ArrayCounter and indirectly sets the max of the MultiCounter (or MaxSeatVector)
            // value[1] is the counts in the MultiCounter and represents AvailableSeatVector
            // value[2] is the number of seats available that are already taken
            // value[3] is the number of seats left over (or how many people still need a seat)
            _buildingCounter.Count = value[0];
            _seatCounter.Max   = ExtraMath.Linear.AlignedVectorProduct(_buildingCounter.Count, _buildingSeats);
            _seatCounter.Count = value[1];
            SeatsFilled = value[2];
            SeatsLeft   = value[3];
        }
        get {
            return new int[][] {
                _buildingCounter.Count,
                _seatCounter.Count,
                SeatsFilled,
                SeatsLeft,
            };
        }
    }

    // Constructor
    public CivicSimulatorSimple(int[] buildingSeats, int civicID = -1, string civicName = "unnamed_civic") {
        _buildingSeats = buildingSeats;
        _buildingCounter = new ArrayCounter(_buildingSeats.Length);
        _seatCounter     = new MultiCounter(_buildingSeats.Length, 0);
        CivicID   = civicID;
        CivicName = civicName;
        SeatsFilled = new int[_buildingSeats.Length];
        SeatsLeft   = new int[_buildingSeats.Length];
    }

    // This setup assumes that each building serves a different demographic each
    public void Generate(int[] persons) {
        for (int i = 0; i < persons.Length; i++) {
            SeatsFilled[i] = Mathf.Clamp(persons[i], 0, _seatCounter.Count[i]);
            SeatsLeft[i] = _seatCounter.Count[i] - persons[i];
        }
    }

    // Typically with SC4, when a new school/hospital is placed, the number of available seats
    // available to that building is at its max; to mimic that, the addition (or removal) of a building
    // will result in the addition of its corresponding amount of seats
    // In other words, if you build a school that has 1200 seats, the number of seats available
    // will also increase by 1200, and the same goes for removal of a school
    // Note that there is a specific order for this to happen:
    // - If the increment is positive, increment the max, THEN the count
    // - If the increment is negative, decrement the count, THEN the max
    public void IncrementBuildings(int incrementAmt, int bldgType) {
        if (bldgType >= 0 || bldgType < _buildingSeats.Length) {
            int totalIncrement = incrementAmt * _buildingSeats[bldgType];
            if (incrementAmt >= 0) {
                _seatCounter.IncrementMax  (totalIncrement, bldgType);
                _seatCounter.IncrementCount(totalIncrement, bldgType);
            } else {
                _seatCounter.IncrementCount(totalIncrement, bldgType);
                _seatCounter.IncrementMax  (totalIncrement, bldgType);
            }

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