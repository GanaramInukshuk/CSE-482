using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DemandEvaluators;
using PlayerControls;

// This class is in charge of all of the game's calculations and requires references
// to every UI object in the game's UI

public class GameLoop : MonoBehaviour {

    // Many of the UI objects here either have a simulator or something else that's important
    // so those UI objects require accessors to the simulator's data (setters/getters basically)
    // - Most if not all zoning and civic controls: their simulators's interfaces and datavectors (for savedata)
    // - 
    [Header("References to Main UI Objects")]
    [SerializeField] private ResidentialControls     _resCtrl;
    [SerializeField] private CommercialControls      _commCtrl;
    [SerializeField] private IncrementSliderControls _incrementCtrl;
    [SerializeField] private TimeControls            _timeCtrl;
    [SerializeField] private FundingControls         _fundingCtrl;

    [Header("References to other UI objects")]
    [SerializeField] private Text _textPopulation;
    [SerializeField] private Text _textEmployment;

    // Private objects
    private WorkforceEvaluator   _workEval = new WorkforceEvaluator();
    private ResidentialEvaluator _resEval  = new ResidentialEvaluator();

    private void Start() {
        _textPopulation.text = "Population: 0";
        _textEmployment.text = "Employment: 0";
    }

    // In general:
    // - The user may add buildings at any time and such calculations (EG construction costs
    //   and updating max occupancy) are done outside of FixedUpdate
    // - The GameLoop handles events at three different frequencies: every in-game day, every
    //   in-game week, and every four in-game weeks
    // - Daily events include increases in occupancy (EG households becoming occupied) and changes
    //   in demand
    // - Weekly events include updates to population and employment; think of this as a census
    //   being conducted every week in that, in between censuses being conducted, anything can
    //   happen
    // - Quad-weekly events include updates to city finances (self-explanatory); this is basically
    //   a figure of how much income the city is generating based on its tax revenue (property
    //   tax, sales tax, etc) and how much the city spends on other amenities (public schools,
    //   hospitals, etc)
    private void FixedUpdate() {

        // Actions to perform every in-game day (not to be confused with an episodic day)
        if (_timeCtrl.TickCount % Timekeeper._ticksPerDay == 0) {
            //Debug.Log("[GameLoop]: Performing daily actions.");

            // Plans for demand system
            // - Demand is generated as a RandomGauss number
            // - The mean may increase by things such as high employment demand or good education and
            //   healthcare; likewise, lack of employment or poor education/health can drive this
            //   value into the negatives
            // - For simplicity's sake, stddev may increase proportionally to the mean; I'm currently
            //   undecided whether I wanna make this be determined differently; for example, the stddev
            //   may be abs(mean) / 2 + 1
            // - For extra randomness, outside factors may screw with demand

            // Some notes about a bellcurve
            // - With a mean and stddev of 0 and 1, the area under the curve within +/- 1, 2, and 3 stddevs
            //   from the mean represents 68, 95, and 99.7% of total area under the curve; alternatively,
            //   given abs(RandomGauss(0, 1)), values between 0 and 1, 1 and 2, and 2 and 3 should occur
            //   68, 27, and 4.7% of the time
            // Base demand is based off of a RandomGauss with a mean of 1 and effective stddev of 1.5


            //int baseDemand = 256;
            //int prevResidentialOpenings = baseDemand - _resCtrl.ZoningBreakdown.OccupantCount;
            //_resCtrl.IncrementOccupants(General.GenerateIncrement(prevResidentialOpenings));

            //// Generate commercial demand; this is based off of the residential population
            ////_workforceEval.GenerateDemand(_resCtrl.PopulationBreakdown, _commCtrl.EmploymentBreakdown);
            //_workforceEval.GenerateDemand(_resCtrl.DemographicBreakdown, _commCtrl.EmploymentBreakdown);
            //int commercialIncrement = _workforceEval.CommercialIncrement;
            //_commCtrl.IncrementOccupants(commercialIncrement);

            //_resEval.GenerateDemand(_commCtrl.ZoningBreakdown, _resCtrl.ZoningBreakdown, _resCtrl.DemographicBreakdown);
            //_workEval.GenerateDemand(_resCtrl.DemographicBreakdown, _commCtrl.ZoningBreakdown, _resCtrl.ZoningBreakdown);

            _resEval.GenerateDemand(_resCtrl.Simulator, _commCtrl.Simulator);
            _workEval.GenerateDemand(_commCtrl.Simulator, _resCtrl.Simulator);

            _resCtrl.IncrementOccupants(_resEval.ResidentialIncrement);
            _commCtrl.IncrementOccupants(_workEval.CommercialIncrement);
        }

        // Actions to perform every in-game week
        if (_timeCtrl.TickCount % Timekeeper._ticksPerWeek == 0) {
            //Debug.Log("[GameLoop]: Performing weekly actions.");
            _resCtrl.Generate();
            _commCtrl.Generate();

            // These calculations are tentative until I get the population simulator up and running (and separated from the ResSim)
            _textPopulation.text = "Population: " + Mathf.RoundToInt(_resCtrl.Simulator.TotalHouseholds * 2.5f).ToString();
            _textEmployment.text = "Employment: " + Mathf.RoundToInt(_commCtrl.Simulator.TotalEmployment * 1.95f).ToString();
        }


        // Actions to perform every 4 in-game weeks
        if (_timeCtrl.TickCount % Timekeeper._ticksPerEpisodicDay == 0) {
            //Debug.Log("[GameLoop]: Performing quad-weekly actions.");

        }

    
    }
}