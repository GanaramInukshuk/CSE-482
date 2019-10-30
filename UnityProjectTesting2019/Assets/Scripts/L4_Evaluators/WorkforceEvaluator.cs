using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Functionality:
// - The WorkforceEvaluator is a companion class for the CommercialSimulator (and proposed IndustrialSimulator
//   and OfficeSimulator) that calculates the appropriate workforce size for the aforementioned simulator(s)
// - This class also calculates the increment amounts for the simulators to simulate job growth (or the reverse)

// Currently, the workforce consists of 90% non-senior adults (basically excludes full-time students or stay-at-home
// caretakers) and 2.5% seniors (EG technically retired senior citizens who choose to work anyway)
// If needed, this can also include 2.5% of the teen2 population to simulate teen workers
// Currently, the workforce is 58% of an entire city's population (assuming default affectors), so for
// really small populations, 3/5 the population (regardless of demographics) is assigned to the workforce

// Note: this previously worked in units of LaborUnits, but I've since reverted the decision of using LaborUnits
// (again) in favor of doing a proper headcount; in other words, the CommSim counts employees individually instead
// of discrete groups of X amount of people and so will this (this decision also results in far easier calculations)

// Labor units don't come into play here, only with the commercial controls

namespace DemandEvaluators {

    public class WorkforceEvaluator {

        public int EmployableMax { private set; get; } = 0;    // Cumulative workforce maximum across all job types

        public int CommercialMax       { private set; get; } = 0;       // Maximum commercial employment
        public int CommercialIncrement { private set; get; } = 0;       // Increment amount
   
        public void GenerateWorkforce(ResidentialScripts.IPopulation popBreakdown, CommercialScripts.IEmployment commEmpBreakdown) {
            // Calculate the total workforce here
            if (popBreakdown.TotalPopulation < 256) {
                EmployableMax = Mathf.RoundToInt(0.5f * popBreakdown.TotalPopulation);
            } else {
                // Working population cosists of portions of the YoungAdult, Adult, and MiddleAge populations,
                // plus a small portion of the Senior and Teen2 populations
                float subpop1 = 0.800f * (popBreakdown.YoungAdultPopulation + popBreakdown.AdultPopulation + popBreakdown.MiddleAgePopulation);
                float subpop2 = 0.025f * popBreakdown.SeniorPopulation;
                float subpop3 = 0.025f * popBreakdown.Teen2Population;
                EmployableMax = Mathf.RoundToInt(subpop1 + subpop2 + subpop3);
            }

            // If more than one employment type existed, then the workforce would be divided between the different types,
            // but at this point, the entire workforce goes towards commercial labor; additionally, labor is measured by
            // each workforce type's respective labor unit
            int CommercialMax = EmployableMax;

            // Generate commercial increment
            // The final increment amount is the delta, divided by 6, fed into the GenerateDemand function
            // The "delta" is the amount of employment capacity left over
            int prevCommercialEmployment = commEmpBreakdown.TotalEmployment;            // Get the previous commercial employment
            int prevCommercialOpenings = CommercialMax - prevCommercialEmployment;      // Get the employment openings left over based off of previous employment
            CommercialIncrement = GenerateIncrement(prevCommercialOpenings);               // Generate the increment

            //if (Mathf.Abs(prevCommercialOpenings) == 1) CommercialIncrement = prevCommercialOpenings > 0 ? 1 : -1;
        }

        //// To calculate an increment, the previous labor force count is needed; this is obtained from CommercialSimulator.UnitCount
        //// There are several situations that can happen:
        //// - The prev and current labor amounts are the same or close to it, resulting in no change
        //// - The new labor amount is larger than the previous amount, resulting in a positive increment
        //// - The new labor amount is smaller than the previous amount, resulting in a negative increment
        //// Increments should be proportional to how big the delta between the two amounts are
        //// This logic should apply for when there is more than one type of workforce (IE Industry and Office)
        //public int GenerateIncrement(int prevCommercialLabor) {
        //}

        // Debug functions
        public void PrintDebugString() {
            string outputString = GetDebugString();
            Debug.Log(outputString);
        }

        public string GetDebugString() {
            return "[WorkforceEvaluator]: Max commercial labor units: " + CommercialMax;
        }

        // Private function that generates demand
        // This system will result in an employment that approaches the commercial max, but does not quite
        // reach it(IE, it's off by a little bit)
        // TODO: fix the issue described by incrementing by 1 until the demand is satisfied
        //private int GenerateIncrement(int demand) {
        //    int mean = Mathf.CeilToInt(demand / 10f);
        //    float stddev = Mathf.Abs(mean) / 2f;
        //    float increment = ExtraRandom.RandomGaussWithClamp(mean, stddev, -3, 3);
        //    return increment > 0 ? Mathf.CeilToInt(increment) : Mathf.FloorToInt(increment);
        //}

        // Until further notice, I'm gonna use this as my increment generator
        private int GenerateIncrement(int demand) {
            int bound = Mathf.CeilToInt(Mathf.Abs(demand) / 16f);
            int increment = Random.Range(0, bound + 1);
            return Mathf.Sign(demand) == 1 ? increment : -increment;
        }

        ////// New function that should prevent overshooting
        //private int GenerateDemand(int demand) {
        //    float openingsFraction = demand > 0 ? Mathf.CeilToInt((float)demand / 8) : Mathf.FloorToInt((float)demand / 8);
        //    return Mathf.RoundToInt(openingsFraction * Random.Range(-0.125f, 0.875f));


        //    //float random
        //    //return demand > 0 ? Mathf.RoundToInt(demand * 0.125f)
        //}
    }
}