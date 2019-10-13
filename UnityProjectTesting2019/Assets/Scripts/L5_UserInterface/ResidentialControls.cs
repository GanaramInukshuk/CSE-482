using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GeneralScripts;

// This script should inherit from MonoBehaviour and therefore should be attached to a UI object
// Attach this to the relevant UI panel and have this script be composed of its relevant simulator and evaluator

namespace PlayerControls {

    [System.Serializable]
    public class ResidentialControls : MonoBehaviour {

        // Main UI objects
        [Header("Main UI Objects and Parameters")]
        [SerializeField] private Button   _buttonIncrement;
        [SerializeField] private Button[] _buttonDecrement;
        [SerializeField] private Image    _image;
        //[SerializeField] private Slider   _incrementSlider;
        [SerializeField] private Text     _textTotal;
        [SerializeField] private Text     _textIncrementAmount;
        //[SerializeField] private GameObject _objectWithSimulator;

        [Header("Secondary UI Objects and Parameters")]
        [SerializeField] private Image _sliderPanel;

        // Aesthetic properties
        [Header("A E S T H E T I C  P R O P S")]
        [SerializeField] private Color _color;

        // Test items
        [Header("Test Items, if any")]

        // Other private members
        private ResidentialSimulator _simulator;
        private IncrementSliderControls _sliderCtrl;

        // Start is called before the first frame update
        void Start() {
            // Set up main stuff
            _simulator   = new ResidentialSimulator();
            _image.color = _color;
            _sliderCtrl  = _sliderPanel.GetComponent<IncrementSliderControls>();
            UpdateTextLabels();

            // Set up increment button functionality
            _buttonIncrement.GetComponentInChildren<Text>().text = "Increment residential";
            _buttonIncrement.onClick.AddListener(Increment);

            // Set up decrement button functionality
            // https://forum.unity.com/threads/how-to-addlistener-featuring-an-argument.266934/
            for (int i = 0; i < _buttonDecrement.Length; i++) {
                int j = i;
                _buttonDecrement[i].GetComponent<Button>().onClick.AddListener(() => Decrement(j));
            }

            //// Set up slider
            //_incrementSlider.minValue = 1;
            //_incrementSlider.maxValue = 64;
            //_incrementSlider.wholeNumbers = true;
            //_incrementSlider.onValueChanged.AddListener(UpdateIncrementAmount);
        }

        // Clicking this button adds commercial buildings to the city wherein the size of the building
        // is dependeng on the total population of the city (EG, a size-8 bldg won't be built if the
        // population is too small to support it)
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
            string totalText = "RESIDENTIAL ZONING: " + zoningBreakdown.MaxZoningUnits + " total living units spread over " + zoningBreakdown.TotalBuildings + " residential buildings.";
            _textTotal.text = totalText;

            // Breakdown text
            // These labels show the number of buildings by size and are displayed on the decrement buttons
            for (int i = 0; i < _buttonDecrement.Length; i++) {
                _buttonDecrement[i].GetComponentInChildren<Text>().text = zoningBreakdown[i].ToString();
            }
        }
    }
}