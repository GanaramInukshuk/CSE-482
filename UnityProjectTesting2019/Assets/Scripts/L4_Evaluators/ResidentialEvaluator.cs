﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Functionality:
// - This is a companion class for the ResSim that calculates residential demand for the residential simulator

// Rules for demand:
// - There is a base demand that caps out at 256 units of residential occupancy; this base demand is fulfilled
//   no matter what and is typically the starting demand at the start of a game
// - Additional demand generated after that is generated by commercial buildings wishing to hire additional
//   people; EmployableDemographic divided by TotalPopulation gives the percentage of the population that contributes
//   to the overall workforce, and the reciprocal of that figure gives the residential demand generated per unused
//   unit of employment (basically residential demand per one employment opening)

// - I am unsure whether that is suitable for enticing people to move into a city, so I have in mind additional
//   rules: performance of education buildings and health buildings which, depending on the score, drives demand
//   up or down

namespace DemandEvaluators {

    public class ResidentialEvaluator {

        public int ResidentialMax       { private set; get; } = 0;
        public int ResidentialIncrement { private set; get; } = 0;

        // This is a base demand that gets fulfilled no matter what
        // This has to be nonzero or the city won't grow at all
        private static readonly int BaseDemandNoMatterWhat = 64;

        // Refined generate function for residential demand
        // One metric for residential demand is the maximum number of job openings
        // Due to a change to how this is calculated, one unit of commercial occupancy translates to one household's worth
        // of employment (IE, at least one person within the household is employed); along with a predetermined amount of base
        // demand, this function will generate an increment that approaches that maximum demand
        public void GenerateDemand(SimulatorInterfaces.IZoningData resData, SimulatorInterfaces.IZoningData commData) {
            // Calculate the maximum employable maximum across all job-generating simulators; this figure represents
            // the maximum residential demand (since one unit of employment is one household)
            // This calculation may be replaced due to some weird, undesirable behaviours; also, I'm kinda doubtful
            // that commercial jobs alone can make people want to move in; basically, this calculation ain't necessarily
            // ideal and I wanna one day replace this with something else; the one for the WorkEval is probably OK
            int maxResidentialFromJobs = commData.OccupantMax;
            ResidentialMax = BaseDemandNoMatterWhat + maxResidentialFromJobs;

            // Calculate a delta; this delta is how far the current residential occupant count is from the max
            // From that delta, the increment is calculated
            int residentialDelta = ResidentialMax - resData.OccupantCount;
            ResidentialIncrement = General.GenerateIncrement(residentialDelta);

            // Limit demand to 1/16th of available openings, unless negative
            if (ResidentialIncrement > 0) {
                int maxIncrement = Mathf.CeilToInt((resData.OccupantMax - resData.OccupantCount) * General.MaxDemandPercentage);
                ResidentialIncrement = Mathf.Min(maxIncrement, ResidentialIncrement);
            }
        }

        #region OldGenerateFunction
        //// One way to generate demand is with employment openings
        //// Demand due to employment openings is determined by calculating population per 1 household and employees per
        //// one population (PPH and EPP); PPH is calculated by dividing the total population by the ResSim's occupant count
        //// and EPP is calculated by dividing the employable demographic (from the ResSim) by the total population; these two
        //// figures are multiplied together to get employees per household (EPH; some unit cancellations reveals that this
        //// can be determined by simply dividing employable demographic by occupancy count).
        //// Turns out the reciprocal of the EPH is needed; this figure is multiplied by the maximum number of jobs that each
        //// of the job-generating simulators offer, and that resulting number is the residential max
        //public void GenerateDemand(SimulatorInterfaces.IZoningData commData, SimulatorInterfaces.IZoningData resData, ResidentialScripts.IDemographic demData) {
        //    // Calculate max based on job openings
        //    //float employeesPerHousehold = (float)demData.EmployableDemographic / (resData.OccupantCount == 0 ? 1 : resData.OccupantCount);
        //    float householdsPerEmployee = (float)resData.OccupantCount / (demData.EmployableDemographic == 0 ? 1 : demData.EmployableDemographic);
        //    int jobMax   = commData.OccupantMax;
        //    int jobCount = commData.OccupantCount;

        //    int maxResidentialFromJobs = Mathf.RoundToInt(jobMax * householdsPerEmployee);

        //    // Calculate overall max
        //    ResidentialMax = BaseDemandNoMatterWhat + maxResidentialFromJobs;
        //    int resDelta = ResidentialMax - resData.OccupantCount;

        //    // Calculate demand
        //    ResidentialIncrement = General.GenerateIncrement(resDelta);

        //    // Limit demand to 1/16th of the available openings, unless negative
        //    if (ResidentialIncrement > 0) {
        //        int maxIncrement = Mathf.CeilToInt((resData.OccupantMax - resData.OccupantCount) * General.MaxDemandPercentage);
        //        ResidentialIncrement = Mathf.Min(maxIncrement, ResidentialIncrement);
        //    }
        //}
        #endregion
    }
}