using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Functionality:
// - This takes a capacity and a demand and calculates the occupancy index; for example, if a building
//   has a maximum occupancy of 100 persons and 87 people decide to enter the building, the building
//   calculates an occupancy index of 0.87, or 87% capacity; if 101 people try to enter the building,
//   then the building is at 101% capacity
// - Determining whether an occupancy index greather than 1 is permissible depends on the situation
// - Another value to consider is the satiation index; using the previous example of 87 people entering
//   a 100-person capacity building, the satiation index is 100%, meaning that the building can satisfy
//   all of the demand (technically, it would be 115% since that represents how much more capacity there
//   is compared to demand; in extreme cases, this would mean there is not enough demand to satisfy the
//   given capacity); however, if 101 people try to enter the bldg but only 100 are allowed in, then only
//   ~99.01% of the demand is satisfied
// - Again, determining whether going over 100% satiation is OK depends on the situation

// Basic arithmetic rules on the OccupancyIndex (demand / capacity)
// - Zero demand results in an occupancy index of 0 (as to be expected)
// - Zero capacity results in division by zero, so as a failsafe, this results in an occupancy index of -1

// Basic arithmetic rules on the SatiationIndex (capacity / demand)
// - Zero capacity results in a satiation index of 0 (as to be expected)
// - Zero demand results in a division by zero, so this has to become -1 as well

// Basically, the OccupancyIndex and SatiationIndex are inverses of one another; one measures the fullness
// of a building and the other measures how many more people need to enter the building

public class CapacityEvaluator {

    // Occupancy and satiation indices
    public float OccupancyIndex { private set; get; } = 0f;
    public float SatiationIndex { private set; get; } = 0f;

    // These are the capacity and demand entered into the Evaluate function
    public int CapacityAvailable { private set; get; } = 0;
    public int DemandAvailable { private set; get; } = 0;

    // Note that this function can be overridden just in case an inheriting class needs extra functionality
    public virtual void Evaluate(int capacity, int demand) {
        CapacityAvailable = capacity;
        DemandAvailable   = demand;
        OccupancyIndex = (demand   > 0) ? (float)demand   / capacity : -1f;
        SatiationIndex = (capacity > 0) ? (float)capacity / demand   : -1f;
    }
}
