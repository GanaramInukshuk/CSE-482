using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Functionality:
// - This is a companion class for the ResSim that calculates residential demand for the residential simulator

namespace DemandEvaluators {

    public class ResidentialEvaluator {

        public int ResidentialMax       { private set; get; } = 0;
        public int ResidentialIncrement { private set; get; } = 0;

        public void GenerateDemand(CommercialScripts.IEmployment empBreakdown) {

        }
    }
}