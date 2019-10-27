using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommercialScripts;
using SimulatorInterfaces;

// This script should inherit from MonoBehaviour and therefore should be attached to a UI object
// Attach this to the relevant UI panel and have this script be composed of its relevant simulator and evaluator

namespace PlayerControls {

    [System.Serializable]
    public class CommercialControls : MonoBehaviour {

        // Main UI objects
        [Header("Main UI Objects and Parameters")]
        [SerializeField] private Button      _buttonIncrement;
        [SerializeField] private Button      _buttonDecrement;
        [SerializeField] private Button      _buttonIncrementRandom;
        [SerializeField] private Text[]      _textBldgCount;
        [SerializeField] private Text        _textTotal;
        [SerializeField] private Text        _textBldgSize;
        [SerializeField] private ProgressBar _progressBar;
        [SerializeField] private Slider      _sliderBldgSize;

        [Header("Secondary UI Objects and Parameters")]
        [SerializeField] private IncrementSliderControls _incrementSlider;
        //[SerializeField] private TimeControls            _timeControls;

        // Other private members/variables
        private CommercialSimulator _simulator;
        private int _bldgSize;

        // References to the simulator's interfaces; these are needed by other simulators
        // As of right now, building counts aren't needed by other simulators
        public IZoningSimulator ZoningBreakdown     { get => _simulator                    ; } 
        public IEmployment      EmploymentBreakdown { get => _simulator.EmploymentBreakdown; }

        // Start is called before the first frame update
        void Start() {
            // Set up main stuff
            _simulator   = new CommercialSimulator();
            UpdateTextLabels();

            //// Set up increment button functionality
            //_buttonIncrement.GetComponentInChildren<Text>().text = "Increment residential";
            //_buttonIncrement.onClick.AddListener(IncrementBuildings);

            // Set up listeners
            _buttonIncrement.onClick.AddListener(() => IncrementBuildings());
            _buttonDecrement.onClick.AddListener(() => DecrementBuildings());
            _buttonIncrementRandom.onClick.AddListener(() => IncrementRandom());
            _sliderBldgSize.onValueChanged.AddListener(UpdateBldgSizeText);

            //// Set up decrement button functionality
            //// https://forum.unity.com/threads/how-to-addlistener-featuring-an-argument.266934/
            //for (int i = 0; i < _buttonDecrement.Length; i++) {
            //    int j = i;
            //    _buttonDecrement[i].GetComponent<Button>().onClick.AddListener(() => DecrementBuildings(j));
            //}

            // Set up slider parameters
            _sliderBldgSize.minValue = 0;
            _sliderBldgSize.maxValue = _textBldgCount.Length - 1;
            _sliderBldgSize.wholeNumbers = true;

            _textBldgSize.text = "Capacity: 1";
        }

        // Some other notes:
        // - I had originally intended the zoning simulators to have updates to occupancy and updates
        //   to population to happen at the same time and this is reflected in the fact that the zoning
        //   simulators have four Generate functions in which three of them have an incrementAmount
        //   parameter; since the two are being separated, anytime the Generate() function is called
        //   and the incrementAmount parameter is needed, that param should be zero
        public void Generate(int incrementAmount) {
            _simulator.Generate(incrementAmount);
            UpdateTextLabels();
        }

        public void Generate() {
            _simulator.Generate();
            UpdateTextLabels();
        }

        public void IncrementOccupants(int incrementAmt) {
            _simulator.IncrementOccupants(incrementAmt);
            UpdateTextLabels();
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
            int   incrementAmount = _incrementSlider.IncrementAmount;
            int[] bldgVector      = _simulator.BldgVector;
            float[] bldgDistribution   = DistributionGen.Probability.GenerateFromHist(bldgVector);
            int[]   bldgAdditionVector = DistributionGen.Histogram.GenerateByWeights(incrementAmount, bldgDistribution);
            _simulator.IncrementBldgs(bldgAdditionVector);
            UpdateTextLabels();
        }

        private void UpdateBldgSizeText(float updatedValue) {
            _textBldgSize.text = "Capacity: " + Constants.EmploymentSizes[(int)updatedValue];
        }

        // This is used to update the rest of the text labels
        // This also updates the progress bar
        private void UpdateTextLabels() {
            // Total text
            string breakdownText = "COMMERCIAL: " + _simulator.OccupantCount + " out of " + _simulator.OccupantMax + " max employees spread over " + _simulator.TotalBuildings + " buildings.";
            _textTotal.text = breakdownText;

            // Breakdown text
            // These labels show the number of buildings by size and are displayed
            for (int i = 0; i < _textBldgCount.Length; i++) {
                _textBldgCount[i].text = _simulator[i].ToString();
            }

            // Progress bar
            _progressBar.UpdateFill(0, _simulator.OccupantMax, _simulator.OccupantCount);
        }
    }
}