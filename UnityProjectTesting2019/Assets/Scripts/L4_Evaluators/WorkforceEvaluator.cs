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

public class WorkforceEvaluator {

    public int CommercialMax       { private set; get; } = 0;       // Commercial labor
    public int CommercialIncrement { private set; get; } = 0;       // Increment amount
   
    public void GenerateWorkforce(ResidentialScripts.IPopulation popBreakdown, CommercialScripts.IEmployment commEmpBreakdown) {
        // Calculate the total workforce here
        int cumulativeWorkforce = 0;
        if (popBreakdown.TotalPopulation < 256) {
            cumulativeWorkforce = Mathf.FloorToInt(0.6f * popBreakdown.TotalPopulation);
        } else {
            int subpop1 = Mathf.FloorToInt(0.900f * (popBreakdown.YoungAdultPopulation + popBreakdown.AdultPopulation + popBreakdown.MiddleAgePopulation));
            int subpop2 = Mathf.FloorToInt(0.025f * popBreakdown.SeniorPopulation);
            int subpop3 = Mathf.FloorToInt(0.025f * popBreakdown.Teen2Population);
            cumulativeWorkforce = subpop1 + subpop2 + subpop3;
        }

        // If more than one employment type existed, then the workforce would be divided between the different types,
        // but at this point, the entire workforce goes towards commercial labor; additionally, labor is measured by
        // each workforce type's respective labor unit
        int commercialWorkforce = cumulativeWorkforce;

        // Generate commercial increment
        // The final increment amount is the delta, divided by 6, fed into the GenerateDemand function
        int prevCommercialLabor = commEmpBreakdown.TotalEmployment;
        int commercialLaborDelta = commercialWorkforce - prevCommercialLabor;
        CommercialIncrement = Mathf.RoundToInt(GenerateDemand(commercialLaborDelta / 6));
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

    // Private function that generates demand using a RandomGauss RNG
    // This is similar to the function used to generate residential demand
    // What this function means:
    // - Generally for a large enough mean, (say, 100), this function will
    //   return a value within the range [50, 150] about 68% of the time with other values
    //   (such as 250 and -50) occuring around <1% of the time
    private float GenerateDemand(int mean) {
        float stddev = 1 + (float)mean / 2;
        float demand = ExtraRandom.RandomGauss(mean, stddev);
        return demand;
    }
}