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
    public ResidentialControls _resCtrl;
    public CommercialControls  _commCtrl;
    public EducationControls   _eduCtrl;
    public HealthControls      _hlthCtrl;
    public IncrementSliderControls _incrementCtrl;
    public TimeControls            _timeCtrl;
    public FundingControls         _fundingCtrl;

    [Header("References to other UI objects")]
    public Text _textPopulation;
    public Text _textEmployment;
    public Text _textResidentialDemand;
    public Text _textCommercialDemand;

    // Private objects
    private WorkforceEvaluator   _workEval = new WorkforceEvaluator();
    private ResidentialEvaluator _resEval  = new ResidentialEvaluator();
    private CivicEvaluator _civicEval = new CivicEvaluator();

    // Simulators
    private ResidentialSimulator _resSim  = new ResidentialSimulator();
    private CommercialSimulator  _commSim = new CommercialSimulator ();
    private EducationSimulator   _eduSim  = new EducationSimulator  ();
    private HealthSimulator      _hlthSim = new HealthSimulator     ();

    private void Awake() {
        _textPopulation.text = "Population: 0";
        _textEmployment.text = "Employment: 0";

        _resCtrl .SetSimulator(_resSim , _incrementCtrl);
        _commCtrl.SetSimulator(_commSim, _incrementCtrl);
        _eduCtrl .SetSimulator(_eduSim , _incrementCtrl);
        _hlthCtrl.SetSimulator(_hlthSim, _incrementCtrl);
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

            _resEval.GenerateDemand(_resSim, _commSim);
            _workEval.GenerateDemand(_commSim, _resSim);
            _civicEval.GenerateDemand(_resSim);

            _resSim.IncrementOccupants(_resEval.ResidentialIncrement);
            _commSim.IncrementOccupants(_workEval.CommercialIncrement);

            // Update text labes on controls
            _resCtrl .UpdateTextLabels();
            _commCtrl.UpdateTextLabels();
            _eduCtrl .UpdateTextLabels();
            _hlthCtrl.UpdateTextLabels();

            // Demand
            _textResidentialDemand.text = "Residential: " + (_resEval.ResidentialMax - _resSim .OccupantCount).ToString();
            _textCommercialDemand .text = "Commercial: "  + (_workEval.EmployableMax - _commSim.OccupantCount).ToString();
        }

        // Actions to perform every in-game week
        if (_timeCtrl.TickCount % Timekeeper._ticksPerWeek == 0) {
            //Debug.Log("[GameLoop]: Performing weekly actions.");
            _resSim.Generate();
            _commSim.Generate();

            // Generate K12 education data
            int[] schoolchildren = new int[] { _civicEval.ElementarySchoolMax, _civicEval.MiddleShcoolMax, _civicEval.HighSchoolMax };
            _eduSim.Generate(schoolchildren);

            // Generate data for patients
            int[] patients = new int[] { _civicEval.ClinicPatientsMax, _civicEval.HospitalPatientsMax };
            _hlthSim.Generate(patients);

            // Update text labes on controls
            _resCtrl .UpdateTextLabels();
            _commCtrl.UpdateTextLabels();
            _eduCtrl .UpdateTextLabels();
            _hlthCtrl.UpdateTextLabels();

            // These calculations are tentative until I get the population simulator up and running (and separated from the ResSim)
            _textPopulation.text = "Population: " + Mathf.RoundToInt(_resSim .OccupantCount * 2.50f).ToString();
            _textEmployment.text = "Employment: " + Mathf.RoundToInt(_commSim.OccupantCount * 1.75f).ToString();
        }


        // Actions to perform every 4 in-game weeks
        if (_timeCtrl.TickCount % Timekeeper._ticksPerEpisodicDay == 0) {
            //Debug.Log("[GameLoop]: Performing quad-weekly actions.");

        }
    }
}