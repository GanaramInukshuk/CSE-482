using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimulatorInterfaces;
using static ResidentialSimulator.Constants;

// This is a companion class for the civic simulators that calculates the appropriate number
// of students and patients for the education and health simulators

namespace DemandEvaluators {

    public class CivicEvaluator {

        public int ElementarySchoolMax { private set; get; }
        public int MiddleShcoolMax     { private set; get; }
        public int HighSchoolMax       { private set; get; }

        public void GenerateDemand(IZoningData resData) { 
            int familiesIndex = (int)OccupantType.FAMILY;
            int extendedsIndex = (int)OccupantType.EXTENDED;
            float householdsWithSchoolchildren = resData.OccupantVector[familiesIndex] + resData.OccupantVector[extendedsIndex];
            //Debug.Log(resData.OccupantVector[familiesIndex]);

            // These are tentative calculations until I get the population simulator up and running again
            // Recall that tentative calculations are as follows:
            // One household = 2.5 people and 1.75 employees
            // For our purposes, let's assume an avg family household size of 3.8 with 1.8 being parental figures
            // then assume that 1/3rd that difference each goes to each level of schooling
            float childrenPerHousehold = 3.5f - 1.8f;
            ElementarySchoolMax = MiddleShcoolMax = HighSchoolMax = Mathf.RoundToInt(householdsWithSchoolchildren * childrenPerHousehold / 3);
            //Debug.Log(HighSchoolMax);
            }
    }
}