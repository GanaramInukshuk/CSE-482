using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Functionality:
// - The WorkforceEvaluator is a companion class for the CommercialSimulator (and proposed IndustrialSimulator
//   and OfficeSimulator) that calculates the appropriate workforce size for the aforementioned simulator(s)
// - This class also calculates the increment amounts for the simulators to simulate job growth (or the reverse)

// Currently, the workforce consists of 90% non-senior adults (EG full-time students or stay-at-home
// caretakers) and 2.5% seniors (EG technically retired senior citizens who choose to work anyway)
// If needed, this can also include 2.5% of the teen2 population
// Currently, the workforce is 58% of an entire city's population (assuming default affectors), so for
// really small populations, 1/2 the population (regardless of demographics) is assigned to the workforce

public class WorkforceEvaluator {

    public int CommercialLabor     { private set; get; } = 0;       // Commercial labor in commercial labor units
    public int CommercialIncrement { private set; get; } = 0;       // Increment amount in commercial labor units
   
    public void GenerateWorkforce(ResidentialScripts.IPopulation popBreakdown) {
        // Calculate the total workforce here
        int workforceTotal = 0;
        if (popBreakdown.TotalPopulation < 256) {
            workforceTotal = popBreakdown.TotalPopulation / 2;
        } else {
            int subpop1 = Mathf.FloorToInt(0.900f * (popBreakdown.YoungAdultPopulation + popBreakdown.AdultPopulation + popBreakdown.MiddleAgePopulation));
            int subpop2 = Mathf.FloorToInt(0.025f * popBreakdown.SeniorPopulation);
            int subpop3 = Mathf.FloorToInt(0.025f * popBreakdown.Teen2Population);
            workforceTotal = subpop1 + subpop2 + subpop3;
        }

        // If more than one employment type existed, then the workforce would be divided between the different types,
        // but at this point, the entire workforce goes towards commercial labor; additionally, labor is measured by
        // each workforce type's respective labor unit
        CommercialLabor = Mathf.RoundToInt(workforceTotal / CommercialScripts.Constants.LaborUnit);
    }

    // To calculate an increment, the previous labor force count is needed; this is obtained from CommercialSimulator.UnitCount
    // There are several situations that can happen:
    // - The prev and current labor amounts are the same or close to it, resulting in no change
    // - The new labor amount is larger than the previous amount, resulting in a positive increment
    // - The new labor amount is smaller than the previous amount, resulting in a negative increment
    // Increments should be proportional to how big the delta between the two amounts are
    // This logic should apply for when there is more than one type of workforce (IE Industry and Office)
    public void GenerateIncrement(int prevCommercialLabor) {
        // TODO: Populate this
        CommercialIncrement = 4;
    }

    // Debug functions
    public void PrintDebugString() {
        string outputString = GetDebugString();
        Debug.Log(outputString);
    }

    public string GetDebugString() {
        return "[WorkforceEvaluator]: Max commercial labor units: " + CommercialLabor;
    }
}