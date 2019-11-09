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

        // This is to compensate for the base demand in 
        // Make this nonzero for best results
        private static readonly int BaseDemandNoMatterWhat = 0;
        
        // Refined generate function for job demand
        public void GenerateDemand(SimulatorInterfaces.IZoningData commData, SimulatorInterfaces.IZoningData resData) {
            // For identifying the senior household occupant index
            int seniorIndex = (int)ResidentialSimulator.Constants.OccupantType.SENIOR;

            // Calculate how many households have at least one employable person
            // This is tentatively calculated as all households that are not classified as a senior household
            int maxWorkforceFromHousing = resData.OccupantCount - resData.OccupantVector[seniorIndex];
            EmployableMax = BaseDemandNoMatterWhat + maxWorkforceFromHousing;

            // Divvy up the employable max among the different types of employment
            // Since there's only commercial jobs, 100% of the employable max goes to commercial max
            CommercialMax = EmployableMax;

            // Calculate a delta; this delta is how far the current commercial occupant count is from the max
            // From that delta, the increment is calculated
            // This process would be repeated for other types of employment
            int commercialDelta = CommercialMax - commData.OccupantCount;
            CommercialIncrement = General.GenerateIncrement(commercialDelta);

            // Limit demand to 1/16th of available openings, unless negative
            if (CommercialIncrement > 0) {
                int maxIncrement = Mathf.CeilToInt((commData.OccupantMax - commData.OccupantCount) * General.MaxDemandPercentage);
                CommercialIncrement = Mathf.Min(maxIncrement, CommercialIncrement);
            }
        }

        #region OldGenerateFunction
        /*
        public void GenerateDemand(ResidentialScripts.IDemographic demographicBreakdown, SimulatorInterfaces.IZoningData commData, SimulatorInterfaces.IZoningData resData) {
            //// Calculate the total workforce here
            //// TODO: migrate this calculation to the ResSim itself
            //if (popBreakdown.TotalPopulation < 256) {
            //    EmployableMax = Mathf.RoundToInt(0.5f * popBreakdown.TotalPopulation);
            //} else {
            //    // Working population cosists of portions of the YoungAdult, Adult, and MiddleAge populations,
            //    // plus a small portion of the Senior and Teen2 populations
            //    float subpop1 = 0.900f * (popBreakdown.YoungAdultPopulation + popBreakdown.AdultPopulation + popBreakdown.MiddleAgePopulation);
            //    float subpop2 = 0.025f * popBreakdown.SeniorPopulation;
            //    float subpop3 = 0.025f * popBreakdown.Teen2Population;
            //    EmployableMax = Mathf.RoundToInt(subpop1 + subpop2 + subpop3);
            //}
            //EmployableMax = demographicBreakdown.EmployableDemographic + BaseDemandNoMatterWhat;
            EmployableMax = resData.OccupantCount;
            EmployableMax = Mathf.Max(0, EmployableMax);

            // If more than one employment type existed, then the workforce would be divided between the different types,
            // but at this point, the entire workforce goes towards commercial labor; additionally, labor is measured by
            // each workforce type's respective labor unit
            int CommercialMax = EmployableMax;

            // Generate commercial increment
            // The final increment amount is the delta, divided by 6, fed into the GenerateDemand function
            // The "delta" is the amount of employment capacity left over
            int commercialDelta = CommercialMax - commData.OccupantCount;      // Get the employment openings left over based off of previous employment
            CommercialIncrement = General.GenerateIncrement(commercialDelta);               // Generate the increment

            if (CommercialIncrement > 0) {
                int maxIncrement = Mathf.CeilToInt((commData.OccupantMax - commData.OccupantCount) * General.MaxDemandPercentage);
                CommercialIncrement = Mathf.Min(maxIncrement, CommercialIncrement);
            }
        }
        */
        #endregion
    }
}