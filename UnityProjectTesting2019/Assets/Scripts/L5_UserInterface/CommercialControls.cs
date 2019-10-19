using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GeneralScripts;

// This script should inherit from MonoBehaviour and therefore should be attached to a UI object
// Attach this to the relevant UI panel and have this script be composed of its relevant simulator and evaluator

namespace PlayerControls {

    public class CommercialControls : MonoBehaviour {

        // Main UI objects
        [Header("Main UI Objects and Parameters")]
        [SerializeField] private Button   _buttonIncrement;
        [SerializeField] private Button[] _buttonDecrement;
        [SerializeField] private Text     _textTotal;
        [SerializeField] private Text     _textIncrementAmount;
        //[SerializeField] private GameObject _objectWithSimulator;

        [Header("Secondary UI Objects and Parameters")]
        [SerializeField] private Image       _sliderPanel;
        [SerializeField] private ProgressBar _progressBar;

        // Other private members
        private CommercialSimulator _simulator;
        //private int _incrementAmount = 1;
        private IncrementSliderControls _sliderCtrl;

        // Start is called before the first frame update
        void Start() {
            // Set up main stuff
            _simulator   = new CommercialSimulator();
            _sliderCtrl  = _sliderPanel.GetComponent<IncrementSliderControls>();
            UpdateTextLabels();

            // Set up increment button functionality
            _buttonIncrement.GetComponentInChildren<Text>().text = "Increment commercial";
            _buttonIncrement.onClick.AddListener(Increment);

            // Set up decrement button functionality
            // https://forum.unity.com/threads/how-to-addlistener-featuring-an-argument.266934/
            for (int i = 0; i < _buttonDecrement.Length; i++) {
                int j = i;
                _buttonDecrement[i].GetComponent<Button>().onClick.AddListener(() => Decrement(j));
            }
        }

        // For debugging
        private void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                _simulator.IncrementUnits(UnityEngine.Random.Range(1, 16));
                UpdateTextLabels();
            }
        }

        // Clicking this button adds commercial buildings to the city wherein the size of the building
        // is dependeng on the total occupancy of all existing buildings (EG, a size-8 bldg won't be
        // built if the number of already-occupied stores is too small)
        private void Increment() {
            // Pick a random number; this will be used as the building size index
            // Indices 0-5 correspond to sizes 1, 2, 3, 4, 6, 8
            // Later on, this should be replaced with weighted probabilities where, in general,
            // larger occupancies (not building count) increase the probability of building a
            // higher capacity building
            int randomNumber    = UnityEngine.Random.Range(0, _buttonDecrement.Length);
            int incrementAmount = _sliderCtrl.IncrementAmount;
            _simulator.IncrementBldgs(incrementAmount, randomNumber);
            UpdateTextLabels();
        }

        // Clicking this button decrements housing from the city; increments are basically random
        // but decrements are not, so the user has full control over what structures can be demolished;
        // this function therefore requires a parameter so the simulator knows which housing type to demolish
        private void Decrement(int type) {
            //Debug.Log("[CommercialControls]: Attempting to decrement " + _incrementAmount + " bldgs from index " + type);
            int incrementAmount = _sliderCtrl.IncrementAmount;
            _simulator.IncrementBldgs(-incrementAmount, type);
            UpdateTextLabels();
        }

        //// This is used for the slider to change the increment amount
        //// The parameter is the updated value of the slider itself, which is then used for the label
        //private void UpdateIncrementAmount(float updatedValue) {
        //    _incrementAmount = (int)updatedValue;
        //    _textIncrementAmount.text = "Increment: " + updatedValue;
        //}

        // This is used to update the rest of the text labels
        private void UpdateTextLabels() {
            IZonableBuilding zoningBreakdown = _simulator.ZoningBreakdown;

            // Total text
            string totalText = "COMMERCIAL: " + _simulator.UnitCount + " out of " + _simulator.UnitMax + " max labor units spread over " + zoningBreakdown.TotalBuildings + " buildings.";
            _textTotal.text = totalText;

            // Breakdown text
            // These labels show the number of buildings by size and are displayed on the decrement buttons
            for (int i = 0; i < _buttonDecrement.Length; i++) {
                _buttonDecrement[i].GetComponentInChildren<Text>().text = zoningBreakdown[i].ToString();
            }
            
            // Progress bar
            _progressBar.UpdateFill(0, _simulator.UnitMax, _simulator.UnitCount);
        }
    }
}