using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// The TimeControls script is used to count up game ticks and also adjusts the
// speed at which ticks count up; note that the Timekeeper script is only used
// to interpret the tick count into a meaningful time and date

namespace PlayerControls {

    [System.Serializable]
    public class TimeControls : MonoBehaviour {

        // Main UI objects
        [Header("Main UI Objects and Parameters")]
        [SerializeField] private Image  _image;
        [SerializeField] private Slider _speedSlider;
        [SerializeField] private Text   _textDate;
        [SerializeField] private Text   _textSpeed;
        [SerializeField] private int    _tickCount;

        [Header("Secondary UI Objects and Parameters")]
        [SerializeField] private Image _sliderPanel;

        // Aesthetic properties
        [Header("A E S T H E T I C  P R O P S")]
        [SerializeField] private Color _color;

        // Test items
        //[Header("Test Items, if any")]

        // Start is called before the first frame update
        void Start() {
        
        }

        // Update is called once per frame
        // TODO: Have this be done via coroutine instead
        void Update() {
            _tickCount++;
            _textDate.text = "Date: " + Timekeeper.SimpleDate(_tickCount);
        }
    }
}
