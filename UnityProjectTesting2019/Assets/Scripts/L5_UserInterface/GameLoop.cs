﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DemandEvaluators;
using PlayerControls;
using Newtonsoft.Json;
using System.IO;
using TMPro;

// This class is in charge of all of the game's calculations and requires references
// to every UI object in the game's UI

// The gameloop also manages savefiles

public class GameLoop : MonoBehaviour {

    // A data class for savefiles
    private class Savedata {
        public ZoningSimulator.DataClass residentialData = new ZoningSimulator.DataClass();
        public ZoningSimulator.DataClass commercialData  = new ZoningSimulator.DataClass();
        public CivicSimulatorSimple.DataClass educationData  = new CivicSimulatorSimple.DataClass();
        public CivicSimulatorSimple.DataClass healthcareData = new CivicSimulatorSimple.DataClass();
        public int financialData = 200000;
        public int gameTime      = 0;
        public bool gameSaved = false;
    }

    // Many of the UI objects here either have a simulator or something else that's important
    // so those UI objects require accessors to the simulator's data (setters/getters basically)
    // - Most if not all zoning and civic controls: their simulators's interfaces and datavectors (for savedata)
    // - 
    [Header("Zoning Simulators")]
    public ZoningControls _resCtrl;
    public ZoningControls _commCtrl;

    [Header("Civic Simulators")]
    public CivicControls _eduCtrl;
    public CivicControls _hlthCtrl;

    [Header("Other controls")]
    public IncrementSliderControls _incrementCtrl;
    public TimeControls            _timeCtrl;
    //public FundingControls         _fundingCtrl;

    [Header("References to other UI objects")]
    public Text _textPopulation;
    public Text _textEmployment;
    public Text _textResidentialDemand;
    public Text _textCommercialDemand;
    public Text _textFunds;
    public Text _textIncome;
    public TextMeshProUGUI _tmpStats;

    public Text _textSavefilePreview;

    [Header("Parameters")]
    public int _initialFunds = 200000;

    [Header("References to Game Objects")]
    public SunController _sc;

    // Private objects
    private WorkforceEvaluator   _workEval = new WorkforceEvaluator();
    private ResidentialEvaluator _resEval  = new ResidentialEvaluator();
    private CivicEvaluator       _civicEval = new CivicEvaluator();

    private FundingManager _fundingMgr;

    // Simulators
    private ResidentialSimulator _resSim  = new ResidentialSimulator();
    private CommercialSimulator  _commSim = new CommercialSimulator ();
    private EducationSimulator   _eduSim  = new EducationSimulator  ();
    private HealthSimulator      _hlthSim = new HealthSimulator     ();

    private void Awake() {
        _textPopulation.text = "Population: 0";
        _textEmployment.text = "Employment: 0";

        _fundingMgr = new FundingManager(_initialFunds, _textFunds, _textIncome);

        _textFunds.text = $"Funds: {_initialFunds}";
        _textIncome.text = "";

        _resCtrl .SetSimulator(_resSim , _incrementCtrl._incrementSlider, _fundingMgr);
        _commCtrl.SetSimulator(_commSim, _incrementCtrl._incrementSlider, _fundingMgr);
        _eduCtrl .SetSimulator(_eduSim , _incrementCtrl._incrementSlider, _fundingMgr);
        _hlthCtrl.SetSimulator(_hlthSim, _incrementCtrl._incrementSlider, _fundingMgr);
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
        // Actions to perform EVERY TICK must preceed the IF statement blocks
        _sc.UpdateSunAndMoon(_timeCtrl.TickCount, Timekeeper._ticksPerEpisodicDay);

        // Actions to perform every in-game day (not to be confused with an episodic day)
        if (_timeCtrl.TickCount % Timekeeper._ticksPerDay == 0) {
            // If generating every in-game is not desired, comment out the six lines before the evaulators' generate functinos
            _resSim.Generate();
            _commSim.Generate();

            // Generate K12 education data
            int[] schoolchildren = new int[] { _civicEval.ElementarySchoolMax, _civicEval.MiddleShcoolMax, _civicEval.HighSchoolMax };
            _eduSim.Generate(schoolchildren);

            // Generate data for patients
            int[] patients = new int[] { _civicEval.ClinicPatientsMax, _civicEval.HospitalPatientsMax };
            _hlthSim.Generate(patients);

            //Debug.Log("[GameLoop]: Performing daily actions.");

            _resEval.GenerateDemand(_resSim, _eduSim, _hlthSim);
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
            _textResidentialDemand.text = "Residential demand: " + (_resEval.ResidentialMax - _resSim .OccupantCount).ToString();
            _textCommercialDemand .text = "Commercial demand: "  + (_workEval.EmployableMax - _commSim.OccupantCount).ToString();

            // Stats
            PrintStats();
        }

        // Actions to perform every in-game week
        if (_timeCtrl.TickCount % Timekeeper._ticksPerWeek == 0) {
            ////Debug.Log("[GameLoop]: Performing weekly actions.");
            //_resSim.Generate();
            //_commSim.Generate();

            //// Generate K12 education data
            //int[] schoolchildren = new int[] { _civicEval.ElementarySchoolMax, _civicEval.MiddleShcoolMax, _civicEval.HighSchoolMax };
            //_eduSim.Generate(schoolchildren);

            //// Generate data for patients
            //int[] patients = new int[] { _civicEval.ClinicPatientsMax, _civicEval.HospitalPatientsMax };
            //_hlthSim.Generate(patients);

            // Update text labes on controls
            _resCtrl .UpdateTextLabels();
            _commCtrl.UpdateTextLabels();
            _eduCtrl .UpdateTextLabels();
            _hlthCtrl.UpdateTextLabels();

            // These calculations are tentative until I get the population simulator up and running (and separated from the ResSim)
            _textPopulation.text = "Population: " + Mathf.RoundToInt(_resSim .OccupantCount * 2.50f).ToString();
            _textEmployment.text = "Employment: " + Mathf.RoundToInt(_commSim.OccupantCount * 1.75f).ToString();

            // Calculate the total number of civic seats available
            int totalEduSeats    = DistributionGen.Histogram.SumOfElements(_eduSim .SeatCountVector);
            int totalHealthSeats = DistributionGen.Histogram.SumOfElements(_hlthSim.SeatCountVector);
            int totalZoningOccupants = _resSim.OccupantCount + _commSim.OccupantCount;
            _fundingMgr.GenerateIncome(totalZoningOccupants, totalEduSeats, totalHealthSeats);
        }


        // Actions to perform every 4 in-game weeks
        // Saves are recorded to autosave at these intervals
        if (_timeCtrl.TickCount % Timekeeper._ticksPerEpisodicDay == 0) {
            SaveGame();
        }
    }

    // This genreates the stats from each of the simulators and outputs it to a TextMeshPro text object
    // Yes, there's a systematic way of doing this and that requires having each simulator generate their
    // own stats and also having each simulator also manage their own expenses/revenues as a string, then
    // combining all those strings together into one
    public void PrintStats() {
        int expectedResidentialTaxRevenue = _resSim.OccupantCount * _fundingMgr.ConstBaseTaxRevenue;
        int expectedCommercialTaxRevenue = _commSim.OccupantCount * _fundingMgr.ConstBaseTaxRevenue;

        int elemExpenses = _eduSim.SeatCountVector[0] * _fundingMgr.ConstBaseCivicSeatCost[0];
        int middExpenses = _eduSim.SeatCountVector[1] * _fundingMgr.ConstBaseCivicSeatCost[0];
        int highExpenses = _eduSim.SeatCountVector[2] * _fundingMgr.ConstBaseCivicSeatCost[0];

        int clinicExpenses = _hlthSim.SeatCountVector[0] * _fundingMgr.ConstBaseCivicSeatCost[1];
        int hospitalExpenses = _hlthSim.SeatCountVector[1] * _fundingMgr.ConstBaseCivicSeatCost[1];

        int income = expectedCommercialTaxRevenue + expectedResidentialTaxRevenue - elemExpenses - middExpenses - highExpenses - clinicExpenses - hospitalExpenses;

        string outputString = 
            "<b>Overall City Stats</b>\n" +
            $"Population: {Mathf.RoundToInt(_resSim .OccupantCount * 2.50f).ToString()}\n" +
            $"Employment: {Mathf.RoundToInt(_commSim.OccupantCount * 1.75f).ToString()}\n" +
            $"Expected weekly income: {income}\n\n" +

            "<b>Residential Stats</b>\n" +
            $"Residences: {_resSim.OccupantCount}\n" + 
            $"Expected tax revenue per week: {expectedResidentialTaxRevenue}\n\n" +

            "<b>Commercial Stats</b>\n" +
            $"Employed residences: {_commSim.OccupantCount}\n" + 
            $"Expected tax revenue per week: {expectedCommercialTaxRevenue}\n\n" +

            // Note: any would-be division by 0 results in a 0; for our purposes, n / 0 where n is [0, infinity) gets mapped back to 0
            "<b>Education Stats</b>\n" +
            $"Elementary School Percent Capacity: {(_eduSim.SeatCountVector[0] == 0 ? 0 : Mathf.RoundToInt(_eduSim.SeatsFilled[0] * 100f / _eduSim.SeatCountVector[0]))}%\n" + 
            $"Middle School Percent Capacity: {(_eduSim.SeatCountVector[1] == 0 ? 0 : Mathf.RoundToInt(_eduSim.SeatsFilled[1] * 100f / _eduSim.SeatCountVector[1]))}%\n" + 
            $"High School Percent Capacity: {(_eduSim.SeatCountVector[2] == 0 ? 0 : Mathf.RoundToInt(_eduSim.SeatsFilled[2] * 100f / _eduSim.SeatCountVector[2]))}%\n" + 
            $"Elementary School Expenses per week: {elemExpenses}\n" +
            $"Middle School Expenses per week: {middExpenses}\n" +
            $"High School Expenses per week: {highExpenses}\n" +
            "\n" +

            "<b>Healthcare Stats</b>\n" +
            $"Clinic Percent Capacity: {(_hlthSim.SeatCountVector[0] == 0 ? 0 : Mathf.RoundToInt(_hlthSim.SeatsFilled[0] * 100f / _hlthSim.SeatCountVector[0]))}%\n" + 
            $"Hospital Percent Capacity: {(_hlthSim.SeatCountVector[1] == 0 ? 0 : Mathf.RoundToInt(_hlthSim.SeatsFilled[1] * 100f / _hlthSim.SeatCountVector[1]))}%\n" + 
            $"Clinic Expenses per week: {clinicExpenses}\n" +
            $"Hospital Facility Expenses per week: {hospitalExpenses}\n" +
            "\n"
        ;

        _tmpStats.text = outputString;
    }

    // Savefile handler
    // This is accomplished with the help of the JSON.NET for Unity asset
    public void SaveGame(string filename = "_autosave") {
        // Strings for the save directory and savefile
        string savePath = Application.dataPath + "/Saves";
        string saveName = savePath + "/" + filename + ".txt";

        // Sanity check: if the filename is "", assume it's "_autosave"
        if (filename == "") saveName = savePath + "/_autosave.txt";

        // The game will save data in JSON format into a text file
        // These datavectors, plus the funding amount, are saved into a single JSON string
        Savedata s = new Savedata {
            residentialData = _resSim.DataVector ,
            commercialData  = _commSim.DataVector,
            educationData   = _eduSim.DataVector ,
            healthcareData  = _hlthSim.DataVector,
            financialData   = _fundingMgr.Funds,
            gameTime = _timeCtrl.TickCount,
            gameSaved = true
        };

        // Convert the savedata class into a JSON string
        string saveString = JsonConvert.SerializeObject(s, Formatting.Indented);

        // Create the savefile folder if it doesn't exist
        DirectoryInfo d = Directory.CreateDirectory(savePath);

        // Create the savefile if it doesn't exist either; if it exists, overwrite it
        File.WriteAllText(saveName, saveString);

        // For debugging; load the string
        Debug.Log(saveString);
        _textSavefilePreview.text = "Saved to savefile: " + filename.ToString();
    }

    // This is also accomplished with the JSON.NET for Unity asset
    // Parameter is the file name (without the file extension)
    public void LoadGame(string filename = "_autosave") {
        // Strings for the save directory and savefile
        string savePath = Application.dataPath + "/Saves";
        string saveName = savePath + "/" + filename + ".txt";

        // Sanity check: if the filename is "", assume it's "_autosave"
        if (filename == "") saveName = savePath + "/_autosave.txt";

        try {
            string saveString = File.ReadAllText(saveName);
            Savedata s = JsonConvert.DeserializeObject<Savedata>(saveString);

            _resSim.DataVector = s.residentialData;
            _commSim.DataVector = s.commercialData;
            _eduSim.DataVector = s.educationData;
            _hlthSim.DataVector = s.healthcareData;
            _fundingMgr.Funds      = s.financialData;
            _timeCtrl.TickCount  = s.gameTime;

            Debug.Log("[GameLoop]: Loaded save.");

            // Loading a save requires updating the game's UI to reflect new data
            _resEval.GenerateDemand(_resSim, _eduSim, _hlthSim);
            _workEval.GenerateDemand(_commSim, _resSim);
            _civicEval.GenerateDemand(_resSim);

            _resSim.IncrementOccupants(_resEval.ResidentialIncrement);
            _commSim.IncrementOccupants(_workEval.CommercialIncrement);

            // Update text labes on controls
            _resCtrl.UpdateTextLabels();
            _commCtrl.UpdateTextLabels();
            _eduCtrl.UpdateTextLabels();
            _hlthCtrl.UpdateTextLabels();

            // Update funds
            _fundingMgr.UpdateText();

            // Demand
            _textResidentialDemand.text = "Residential demand: " + (_resEval.ResidentialMax - _resSim.OccupantCount).ToString();
            _textCommercialDemand.text = "Commercial demand: "  + (_workEval.EmployableMax - _commSim.OccupantCount).ToString();

            _textSavefilePreview.text = "Loaded savefile: " + filename.ToString();
        } catch (FileNotFoundException) {
            _textSavefilePreview.text = "No data saved in slot to load.";
            Debug.Log("[GameLoop]: Failed to find save.");
        }
    }

    // This deletes a savefile
    // Note: if testing this in Unity, this doesn't automatically delete a file's metadata
    public void DeleteSave(string filename = "_autosave") {
        // Strings for the save directory and savefile
        string savePath = Application.dataPath + "/Saves";
        string saveName = savePath + "/" + filename + ".txt";

        // Sanity check: if the filename is "", assume it's "_autosave"
        if (filename == "") saveName = savePath + "/_autosave.txt";

        // Try to open the file and preview its contents
        try {
            File.Delete(saveName);
            _textSavefilePreview.text = "Save deleted: " + filename.ToString();
        } catch (FileNotFoundException) {
            _textSavefilePreview.text = "No data saved in slot to preview.";
            Debug.Log("[GameLoop]: Failed to find save for previewing.");
        }
    }



    // This previews the contents of the savefile
    public void PreviewSave(string filename = "_autosave") {
        // Strings for the save directory and savefile
        string savePath = Application.dataPath + "/Saves";
        string saveName = savePath + "/" + filename + ".txt";

        // Sanity check: if the filename is "", assume it's "_autosave"
        if (filename == "") saveName = savePath + "/_autosave.txt";

        // Try to open the file and preview its contents
        try {
            string saveString = File.ReadAllText(saveName);
            Savedata s = JsonConvert.DeserializeObject<Savedata>(saveString);

            string previewText = "Savefile: " + filename + "\n" +
                "City Funds: " + s.financialData + "\n" +
                "Game time: " + Timekeeper.SimpleDate(s.gameTime, 2000);
            ;

            _textSavefilePreview.text = previewText;
        } catch (FileNotFoundException) {
            _textSavefilePreview.text = "No data saved in slot to preview.";
            Debug.Log("[GameLoop]: Failed to find save for previewing.");
        }
    }


}