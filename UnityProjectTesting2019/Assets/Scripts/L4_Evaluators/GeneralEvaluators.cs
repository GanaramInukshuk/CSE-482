using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is just a helper static class used to offload common functions between the other evaluators

namespace DemandEvaluators {

    public static class General {
        // Demand entered into the GenerateIncrement function(s) will generate an increment whose maximum
        // is some percentage of the incoming demand, rounded up
        public static readonly float MaxDemandPercentage = 1f / 16;

        // This is the GenerateIncrement function that I've decided to use
        // Steps:
        // 1 - Multiply the (abs. value of) demand by the demand percentage, then round up to the next integer; this is your upperbound
        // 2 - Generate a random number within the range [0, upperbound + 1); the number generated is your increment
        // 3 - Return the increment as a positive or negative number, depending on whether the demand is positive or negative
        public static int GenerateIncrement(int demand) {
            int bound = Mathf.CeilToInt(Mathf.Abs(demand) * MaxDemandPercentage);
            int increment = Random.Range(0, bound + 1);
            return Mathf.Sign(demand) == 1 ? increment : -increment;
        }

        // Overload for accepting float demand
        // The RNG here has a range of [lower, upper] instead of [lower, upper)
        public static int GenerateIncrement(float demand) {
            float bound = Mathf.Abs(demand) * MaxDemandPercentage;
            int increment = Mathf.RoundToInt(Random.Range(0, bound));
            return Mathf.Sign(demand) == 1 ? increment : -increment;
        }

        // TODO: Function to clamp demand to 1/16th of available vacancies
    }
}