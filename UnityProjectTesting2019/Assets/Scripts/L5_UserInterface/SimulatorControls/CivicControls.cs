using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimulatorInterfaces;

namespace PlayerControls {

    public class CivicControls : MonoBehaviour {

        // Main UI objects
        [Header("Main UI Objects and Parameters")]
        // Main increment and decrement buttons and buliding type slider
        public Button _buttonIncrement;
        public Button _buttonDecrement;
        public Button[] _buttonSubincrement;
        public Button[] _buttonSubdecrement;
        public Button[] _buttonQuickSwitch;

        // For displaying information back into the UI
        public Text[]      _textBldgCount;
        public Text        _textTotal;
        public Text        _textBldgType;
        public Text        _textBldgCost;
        public ProgressBar _progressBar;
        public Slider      _sliderBldgType;

        // Strings for use with labels; for clarity:
        // - With education, these are "School type", "school students", and "seats"
        // - With health, these are "Facility type", "patients", and "openings"
        // - With safety (IE, fire/police departments), these are "Dept type", "weekly incidents", and "response capacity"
        // For the first string, it's not necessary to add a colon since the code adds one in
        [Header("Labels")]
        public string _labelBuildingType    = "Building type";
        public string _labelPersonToServe   = "clients";
        public string _labelGeneralSeatName = "seats";
        public string _labelGeneralSeatNameSingular = "seat";

        // Other private members/variables
        private ICivicControls _simulator;
        private Slider _incrementSlider;
        private FundingManager _fundingMgr;

        private string[] _breakdownTextBuffer;

        // Awake function is for setting up the listeners and slider parameters
        void Awake() {
            // Set up listeners
            _buttonIncrement.onClick.AddListener(() => IncrementBuildings());
            _buttonDecrement.onClick.AddListener(() => DecrementBuildings());
            _sliderBldgType.onValueChanged.AddListener(UpdateBldgTypeText);

            // Set up slider
            _sliderBldgType.minValue = 0;
            _sliderBldgType.maxValue = _textBldgCount.Length - 1;
            _sliderBldgType.wholeNumbers = true;

            // Set up listeners for the sub-buttons
            // This also sets up the quick-switch buttons which allow for quickly switching
            // between building types without using the slider
            for (int i = 0; i < _buttonSubincrement.Length; i++) {
                int j = i;
                _buttonSubincrement[i].onClick.AddListener(() => IncrementSeats(j));
                _buttonSubdecrement[i].onClick.AddListener(() => DecrementSeats(j));
                _buttonQuickSwitch[i].onClick.AddListener(() => QuickSwitch(j));
            }
        }

        // This is to receive the simulator and increment slider from the game loop, and to set
        // all the labels afterwards
        public void SetSimulator(ICivicControls simulator, Slider incrementSlider, FundingManager fundingManager) {
            _incrementSlider = incrementSlider;
            _fundingMgr = fundingManager;
            _simulator = simulator;

            _breakdownTextBuffer = new string[_simulator.ConstBuildingTypes.Length];

            _textBldgType.text = $"{_labelBuildingType}: {_simulator.ConstBuildingTypes[0]}\n({_simulator.ConstBuildingSeats[0]} {_labelGeneralSeatName})";
            UpdateTextLabels();
            UpdateBldgTypeText(0);
        }

        public void UpdateTextLabels() {
            // Update the progress bar
            int bldgType = (int)_sliderBldgType.value;
            _progressBar.UpdateFill(0, _simulator.SeatCountVector[bldgType], _simulator.SeatsFilled[bldgType]);

            // Update the breakdown text with the proper text for the current building type
            // Due to the limited space available on the progress bar, only the breakdown text for
            // the currently selected building type is displayed; the breakdown text for all
            // of the building types are generated at once and stored in a buffer, and the current
            // value of _sliderBldgType is used as the index for that buffer to show the breakdown
            // text
            for (int i = 0; i < _simulator.ConstBuildingTypes.Length; i++) {
                string personsToServe = $"{_simulator.ConstBuildingTypes[i].ToLower()} {_labelPersonToServe}";    // To make it easier to write out the string
                //_breakdownTextBuffer[i] = $"{_simulator.CivicName}: {_simulator.SeatsFilled[i]} out of {_simulator.SeatsNeeded[i]} {personsToServe} (out of {_simulator.SeatCountVector[i]} {_labelGeneralSeatName} available)";
                _breakdownTextBuffer[i] = $"{_simulator.CivicName}: {_simulator.SeatsFilled[i]} (out of {_simulator.SeatsNeeded[i]} {personsToServe}) out of {_simulator.SeatCountVector[i]} {_labelGeneralSeatName} available";
            }
            int index = (int)_sliderBldgType.value;
            _textTotal.text = _breakdownTextBuffer[index];

            // Seat counts
            for (int i = 0; i < _textBldgCount.Length; i++) {
                string updatedText = $"{_simulator.BuildingVector[i]} ({_simulator.SeatCountVector[i]}/{_simulator.SeatMaxVector[i]} {_labelGeneralSeatName})";
                _textBldgCount[i].text = updatedText;
            }
        }

        // I don't want to use the increment slider for civic services, so this will
        // exclusively increment and decrement by 1
        // This action can only be successfully performed if there is sufficient funds to do so
        private void IncrementBuildings() {
            int bldgType = (int)_sliderBldgType.value;
            int bldgSeats = _simulator.ConstBuildingSeats[bldgType];

            if (_fundingMgr.ConstructCivic(bldgSeats, _simulator.CivicID)) {
                _simulator.IncrementBuildings(1, bldgType);
                UpdateTextLabels();
            }
        }

        // This action can only be successfully performed if there is enough funds to do so and the count isn't already 0
        private void DecrementBuildings() {
            int bldgType = (int)_sliderBldgType.value;
            int bldgSeats = _simulator.ConstBuildingSeats[bldgType];

            if (_fundingMgr.ConstructCivic(bldgSeats, _simulator.CivicID) && _simulator.BuildingVector[bldgType] != 0) {
                _simulator.IncrementBuildings(-1, bldgType);
                UpdateTextLabels();
            }
        }

        // That said, I will be using the increment slider for adjusting the number of seats
        private void IncrementSeats(int bldgType) {
            int incrementAmount = (int)_incrementSlider.value;
            _simulator.IncrementSeats(incrementAmount, bldgType);
            UpdateTextLabels();
        }

        private void DecrementSeats(int bldgType) {
            int incrementAmount = (int)_incrementSlider.value;
            _simulator.IncrementSeats(-incrementAmount, bldgType);
            UpdateTextLabels();
        }

        // For quickly switching between building sizes
        private void QuickSwitch(int i) {
            _sliderBldgType.value = i;
        }

        // For slider
        // This also updates the text on the progress bar
        private void UpdateBldgTypeText(float updatedValue) {
            int bldgType = (int)_sliderBldgType.value;
            int buildingSize = _simulator.ConstBuildingSeats[bldgType];
            _textBldgType.text = $"{_labelBuildingType}: {_simulator.ConstBuildingTypes[bldgType]}\n({buildingSize} {_labelGeneralSeatName})";

            int index = (int)_sliderBldgType.value;
            _textTotal.text = _breakdownTextBuffer[index];
            _textBldgCost.text = $"Cost: {_fundingMgr.CalculateCivicConstructionCost(buildingSize, _simulator.CivicID)}\nDemolition: {_fundingMgr.CalculateCivicDemolitionCost(buildingSize, _simulator.CivicID)}\n(Costs {_fundingMgr.ConstBaseCivicSeatCost} per {_labelGeneralSeatNameSingular} per week to maintain)";
        }

        //// For updating the construction/demolition cost whenever the increment slider is changed
        //private void UpdateConstructionCostText(float updatedValue) {
        //    int buildingSize = _simulator.ConstBuildingSeats[(int)_sliderBldgType.value];
        //    _textBldgCost.text = "Cost: " + _fundingMgr.CalculateZoningConstructionCost(buildingSize) + "\nDemolition: " + _fundingMgr.CalculateZoningDemolitionCost(buildingSize);
        //}
    }
}