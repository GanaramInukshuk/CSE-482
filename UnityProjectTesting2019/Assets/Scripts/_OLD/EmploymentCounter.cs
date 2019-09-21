using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Don't attach to a regular old GameObject; instead have it be a member of a larger class 
// that's attached to a UI or empty GameObject

// Functionality:
// - This is a counter whose max is the maximum number of employment units and the count
//   is the current employment capacity of a city; this is similar to how the ResSim counts
//   off residential buildings; the occupancy of an individual building is not accounted for
//   but instead the overall occupancy across all buildings
// - I need a custom counter because I need to convert from employment units

//public class EmploymentCounter : MonoBehaviour
//{
//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//}
