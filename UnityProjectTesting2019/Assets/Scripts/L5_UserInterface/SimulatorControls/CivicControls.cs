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
        public Button[] _subincrement;
        public Button[] _subdecrement;

        // For displaying information back into the UI
        public Text[] _textBldgCount;
        public Text   _textTotal;
        public Text   _textBldgType;
        public Text   _textBldgCost;

        public Slider _sliderBldgType;

        // Strings for use with labels; for clarity:
        // - With education, these are "School type", "school students", and "seats"
        // - With health, these are "Facility type", "patients", and "openings"
        // - With safety (IE, fire/police departments), these are "Dept type", "weekly incidents", and "response capacity"
        // For the first string, it's not necessary to add a colon since the code adds one in
        [Header("Labels")]
        public string _labelBuildingType    = "Building type";
        public string _labelPersonToServe   = "clients";
        public string _labelGeneralSeatName = "seats";

        // Other private members/variables
        private ICivicControls _simulator;
        private IncrementSliderControls _incrementSlider;

        // Awake function is for setting up the listeners and slider parameters
        void Awake() {
            // Set up listeners
            _buttonIncrement.onClick.AddListener(() => IncrementBuildings());
            _buttonDecrement.onClick.AddListener(() => DecrementBuildings());
            _sliderBldgType.onValueChanged.AddListener(UpdateBldgTypeText);

            // Set up listeners for the sub-buttons
            for (int i = 0; i < _subincrement.Length; i++) {
                int j = i;
                _subincrement[i].onClick.AddListener(() => IncrementSeats(j));
                _subdecrement[i].onClick.AddListener(() => DecrementSeats(j));
            }

            // Set up slider
            _sliderBldgType.minValue = 0;
            _sliderBldgType.maxValue = _textBldgCount.Length - 1;
            _sliderBldgType.wholeNumbers = true;
        }

        // This is to receive the simulator and increment slider from the game loop, and to set
        // all the labels afterwards
        public void SetSimulator(ICivicControls simulator, IncrementSliderControls incrementSlider) {
            _incrementSlider = incrementSlider;
            _simulator = simulator;
            _textBldgType.text = $"{_labelBuildingType}:\n{_simulator.ConstBuildingTypes[0]}";
            UpdateTextLabels();
        }

        public void UpdateTextLabels() {
            // Total text
            string breakdownText = $"{_simulator.CivicName}: ";
            int numTypes = _simulator.SeatsFilled.Length;

            // Depending on how many building types there are, special formatting is needed
            // For one type, just print the one type
            // For two types, format it as "ABC and XYZ"
            // For three types, format it as "ABC, XYZ, and 123" using a for loop
            // Also note that I'm imposing a limit at three building types because that's as much as I can fit on the screen
            // NOTE THAT ALL THE ENUMS ARE IN ALLCAPS, SO USE ToLower() TO CONVERT TO LOWERCASE
            if (numTypes == 1) {
                breakdownText += $"{_simulator.SeatsFilled[0]} {_simulator.ConstBuildingTypes[0].ToLower()}";
            } else if (numTypes == 2) {
                breakdownText += $"{_simulator.SeatsFilled[0]} {_simulator.ConstBuildingTypes[0].ToLower()} and {_simulator.SeatsFilled[0]} {_simulator.ConstBuildingTypes[0].ToLower()}";
            } else {
                for (int i = 0; i < numTypes; i++) {
                    breakdownText += $"{_simulator.SeatsFilled[i]} {_simulator.ConstBuildingTypes[i].ToLower()}";
                    if (i != numTypes - 1) breakdownText += ", ";       // So that every entry is separated by commas
                    if (i == numTypes - 2) breakdownText += "and ";     // ...except that the last entry gets an "and" between it and its (Oxford) comma
                }
            }
            breakdownText += $" {_labelPersonToServe}";
            _textTotal.text = breakdownText;

            // Seat counts
            for (int i = 0; i < _textBldgCount.Length; i++) {
                string updatedText = $"{_simulator.BuildingVector[i]} ({_simulator.SeatCountVector[i]}/{_simulator.SeatMaxVector[i]} {_labelGeneralSeatName})";
                _textBldgCount[i].text = updatedText;
            }
        }

        // I don't want to use the increment slider for civic services, so this will
        // exclusively increment and decrement by 1
        private void IncrementBuildings() {
            int bldgType = (int)_sliderBldgType.value;
            _simulator.IncrementBuildings(1, bldgType);
            UpdateTextLabels();
        }

        private void DecrementBuildings() {
            int bldgType = (int)_sliderBldgType.value;
            _simulator.IncrementBuildings(-1, bldgType);
            UpdateTextLabels();
        }

        // That said, I will be using the increment slider for adjusting the number of seats
        private void IncrementSeats(int bldgType) {
            int incrementAmt = _incrementSlider.IncrementAmount;
            _simulator.IncrementSeats(incrementAmt, bldgType);
            UpdateTextLabels();
        }

        private void DecrementSeats(int bldgType) {
            int incrementAmt = _incrementSlider.IncrementAmount;
            _simulator.IncrementSeats(-incrementAmt, bldgType);
            UpdateTextLabels();
        }

        // For slider
        private void UpdateBldgTypeText(float updatedValue) {
            int bldgType = (int)_sliderBldgType.value;
            _textBldgType.text = $"{_labelBuildingType}:\n{_simulator.ConstBuildingTypes[bldgType]}";
        }
    }
}