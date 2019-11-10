using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// For use with zoning controls

namespace PlayerControls {
    
    public class IncrementSliderControls : MonoBehaviour {

        // Main UI objects
        [Header("Main UI Objects")]
        [SerializeField] private Image  _image;
        [SerializeField] private Slider _incrementSlider;
        [SerializeField] private Text   _textIncrementAmount;
        [SerializeField] private int    _sliderMaxValue = 64;

        // Other members
        public int IncrementAmount { private set; get; } = 1;

        private void Awake() {
            _incrementSlider.minValue = 1;
            _incrementSlider.maxValue = _sliderMaxValue;
            _incrementSlider.wholeNumbers = true;
            _incrementSlider.onValueChanged.AddListener(UpdateIncrementAmount);
            _textIncrementAmount.text = "Increment Amt: 1";
        }

        // This is used for the slider to change the increment amount
        // The parameter is the updated value of the slider itself, which is then used for the label
        private void UpdateIncrementAmount(float updatedValue) {
            IncrementAmount = (int)updatedValue;
            _textIncrementAmount.text = "Increment Amt: " + updatedValue;
        }
    }
}