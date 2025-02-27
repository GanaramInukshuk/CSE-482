﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimulatorInterfaces;

// This script should inherit from MonoBehaviour and therefore should be attached to a UI object
// Attach this to the relevant UI panel and have this script be composed of its relevant simulator and evaluator

namespace PlayerControls {

    public class ZoningControls : MonoBehaviour {

        // Main UI objects
        [Header("Main UI Objects and Parameters")]
        public Button      _buttonIncrement;
        public Button      _buttonDecrement;
        public Button      _buttonIncrementRandom;
        public Button[]    _buttonQuickSwitch;

        // For displaying information back into the UI
        public Text[]      _textBldgCount;
        public Text        _textTotal;
        public Text        _textBldgSize;
        public Text        _textBldgCost;
        public ProgressBar _progressBar;
        public Slider      _sliderBldgSize;

        // Strings for use with labels; for clarity:
        // - With residential, these are "Households per building" and "households" (or occupants)
        // - For commercial/industrial, these are "Employed households" and "employed households"
        [Header("Labels")]
        public string _labelBuildingSize        = "Occupancy per building";
        public string _labelGeneralOccupantName = "occupants";

        // Other private members/variables
        private IZoningControls _simulator;
        //private IncrementSliderControls _incrementSlider;
        private FundingManager _fundingMgr;

        private Slider _incrementSlider;

        // Awake function is for setting up the listeners and slider parameters
        void Awake() {
            // Set up listeners
            _buttonIncrement.onClick.AddListener(() => IncrementBuildings());
            _buttonDecrement.onClick.AddListener(() => DecrementBuildings());
            //_buttonIncrementRandom.onClick.AddListener(() => IncrementRandom());
            _sliderBldgSize.onValueChanged.AddListener(UpdateBldgSizeText);

            // Set up slider parameters
            _sliderBldgSize.value = 0;
            _sliderBldgSize.minValue = 0;
            _sliderBldgSize.maxValue = _textBldgCount.Length - 1;
            _sliderBldgSize.wholeNumbers = true;

            // Set up listeners to the "quick switch" buttons
            // These buttons allow for quickly switching between building sizes by clicking what
            // appears to be an image graphic without having to use the slider
            for (int i = 0; i < _buttonQuickSwitch.Length; i++) {
                int j = i;
                _buttonQuickSwitch[i].onClick.AddListener(() => QuickSwitch(j));
            }
        }

        // This is to receive the simulator and increment slider from the game loop, and to set
        // all the labels afterwards
        // This also receives the funding manager that handles construction costs and demolition costs (or recouped funds from demolition)
        public void SetSimulator(IZoningControls simulator, Slider incrementSlider, FundingManager fundingManager) {
            _incrementSlider = incrementSlider;
            _simulator = simulator;
            _fundingMgr = fundingManager;

            _incrementSlider.onValueChanged.AddListener(UpdateConstructionCostText);
            UpdateBldgSizeText(0);
            UpdateTextLabels();
        }

        // This is used to update the rest of the text labels
        // This also updates the progress bar
        public void UpdateTextLabels() {
            // Total text
            string breakdownText = $"{_simulator.ZoningName}: {_simulator.OccupantCount} out of {_simulator.OccupantMax} max {_labelGeneralOccupantName} spread over {_simulator.TotalBuildings} buildings.";
            _textTotal.text = breakdownText;

            // Breakdown text
            // These labels show the number of buildings by size and are displayed
            for (int i = 0; i < _textBldgCount.Length; i++) {
                _textBldgCount[i].text = _simulator.BuildingVector[i].ToString();
            }

            // Progress bar
            _progressBar.UpdateFill(0, _simulator.OccupantMax, _simulator.OccupantCount);
        }

        // Clicking this button adds residential buildings of a particular size determined by
        // the slider; that slider value is used as the index that determines which building
        // size to increment by
        // This action can only be successfully performed if there is sufficient funds to do so
        private void IncrementBuildings() {
            int incrementAmount = (int)_incrementSlider.value;
            int buildingIndex   = (int)_sliderBldgSize.value;                       // Pass this to _simulator.IncrementBldgs
            int buildingSize    = _simulator.ConstBuildingSizes[buildingIndex];     // Pass this to _fundMgr.ConstructZoning

            if (_fundingMgr.ConstructZoning(incrementAmount, buildingSize)) {
                _simulator.IncrementBldgs(incrementAmount, buildingIndex);
                UpdateTextLabels();
            }
        }

        // Clicking this button decrements housing from the city; much like the increment button
        // the size of buildings being demolished is determined by the slider
        // This action can only be successfully performed if there is sufficient funds to do so
        private void DecrementBuildings() {
            int buildingIndex = (int)_sliderBldgSize.value;                       // Pass this to _simulator.IncrementBldgs
            int buildingSize  = _simulator.ConstBuildingSizes[buildingIndex];     // Pass this to _fundMgr.ConstructZoning

            // If the increment amount exceeds the number of buildings available for that size,
            // replace the increment amount with the number of buildings
            // Or just use mathf.min
            int incrementAmount = Mathf.Min(_simulator.BuildingVector[buildingIndex], (int)_incrementSlider.value);

            if (_fundingMgr.DemolishZoning(incrementAmount, buildingSize)) {
                _simulator.IncrementBldgs(-incrementAmount, buildingIndex);
                UpdateTextLabels();
            }
        }

        // This adds a random amount of buildings in a proportion based on the distribution of building
        // sizes built already
        // I'm, uhh, actually gonna deactivate this feature...
        private void IncrementRandom() {
            //int     incrementAmount    = _incrementSlider.IncrementAmount;
            //int[]   bldgVector         = _simulator.BuildingVector;
            //float[] bldgDistribution   = DistributionGen.Probability.GenerateFromHist(bldgVector);
            //int[]   bldgAdditionVector = DistributionGen.Histogram.GenerateByWeights(incrementAmount, bldgDistribution);
            //_simulator.IncrementBldgs(bldgAdditionVector);
            //UpdateTextLabels();
        }

        // For quickly switching between building sizes
        private void QuickSwitch(int i) {
            _sliderBldgSize.value = i;
        }

        // For updating the text for the building size; this also displays the construction cost per building
        private void UpdateBldgSizeText(float updatedValue) {
            int buildingSize = _simulator.ConstBuildingSizes[(int)updatedValue];
            int incrementAmt = (int)_incrementSlider.value;
            _textBldgSize.text = _labelBuildingSize + ":\n" + buildingSize;
            _textBldgCost.text = "Cost: " + _fundingMgr.CalculateZoningConstructionCost(buildingSize) * incrementAmt + "\nDemolition: " + _fundingMgr.CalculateZoningDemolitionCost(buildingSize) * incrementAmt;
        }
        
        // For updating the construction/demolition cost whenever the increment slider is changed
        private void UpdateConstructionCostText(float updatedValue) {
            int incrementAmt = (int)updatedValue;
            int buildingSize = _simulator.ConstBuildingSizes[(int)_sliderBldgSize.value];
            _textBldgCost.text = "Cost: " + _fundingMgr.CalculateZoningConstructionCost(buildingSize) * incrementAmt + "\nDemolition: " + _fundingMgr.CalculateZoningDemolitionCost(buildingSize) * incrementAmt;
        }
    }
}