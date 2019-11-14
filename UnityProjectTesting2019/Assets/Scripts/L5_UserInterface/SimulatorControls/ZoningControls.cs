using System.Collections;
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
        private IncrementSliderControls _incrementSlider;

        // Awake function is for setting up the listeners and slider parameters
        void Awake() {
            // Set up listeners
            _buttonIncrement.onClick.AddListener(() => IncrementBuildings());
            _buttonDecrement.onClick.AddListener(() => DecrementBuildings());
            _buttonIncrementRandom.onClick.AddListener(() => IncrementRandom());
            _sliderBldgSize.onValueChanged.AddListener(UpdateBldgSizeText);

            // Set up slider parameters
            _sliderBldgSize.value = 0;
            _sliderBldgSize.minValue = 0;
            _sliderBldgSize.maxValue = _textBldgCount.Length - 1;
            _sliderBldgSize.wholeNumbers = true;
        }

        // This is to receive the simulator and increment slider from the game loop, and to set
        // all the labels afterwards
        public void SetSimulator(IZoningControls simulator, IncrementSliderControls incrementSlider) {
            _incrementSlider = incrementSlider;
            _simulator = simulator;
            _textBldgSize.text = _labelBuildingSize + ":\n" + _simulator.ConstBuildingSizes[0];
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
        private void IncrementBuildings() {
            int incrementAmount = _incrementSlider.IncrementAmount;
            _simulator.IncrementBldgs(incrementAmount, (int)_sliderBldgSize.value);
            UpdateTextLabels();
        }

        // Clicking this button decrements housing from the city; much like the increment button
        // the size of buildings being demolished is determined by the slider
        private void DecrementBuildings() {
            int incrementAmount = _incrementSlider.IncrementAmount;
            _simulator.IncrementBldgs(-incrementAmount, (int)_sliderBldgSize.value);
            UpdateTextLabels();
        }

        // This adds a random amount of buildings in a proportion based on the distribution of building
        // sizes built already
        private void IncrementRandom() {
            int     incrementAmount    = _incrementSlider.IncrementAmount;
            int[]   bldgVector         = _simulator.BuildingVector;
            float[] bldgDistribution   = DistributionGen.Probability.GenerateFromHist(bldgVector);
            int[]   bldgAdditionVector = DistributionGen.Histogram.GenerateByWeights(incrementAmount, bldgDistribution);
            _simulator.IncrementBldgs(bldgAdditionVector);
            UpdateTextLabels();
        }

        private void UpdateBldgSizeText(float updatedValue) {
            _textBldgSize.text = _labelBuildingSize + ":\n" + _simulator.ConstBuildingSizes[(int)updatedValue];
        }
    }
}