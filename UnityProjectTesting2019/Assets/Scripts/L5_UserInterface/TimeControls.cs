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

        // Some private contsants
        float _defaultTimeBetweenTicks = 1f / 20;

        // Main UI objects
        [Header("Main UI Objects")]
        [SerializeField] private Button _buttonPause;
        [SerializeField] private Button _buttonDate;
        [SerializeField] private Image  _image;
        [SerializeField] private Slider _sliderSpeed;
        [SerializeField] private ProgressBar _progressBar;

        [Header("Main Parameters")]
        [SerializeField] private int    _tickCount;
        [SerializeField] private bool   _timeActive;     // For pausing/unpausing time
        [SerializeField] private bool   _altTimeFormat;

        // Aesthetic properties
        [Header("A E S T H E T I C  P R O P S")]
        [SerializeField] private Color _color;
        [SerializeField] private Font  _defaultFont;
        [SerializeField] private Font  _altFont;

        // Test items
        [Header("Debug")]
        //[SerializeField] private float  _secondsPerTick = 1f / 30;      // For debugging

        private int  _lastRecordedSliderSetting;
        private Text _textButtonPause;
        private Text _textButtonDate;

        // Start is called before the first frame update
        void Start() {
            // Set important member values
            _timeActive = true;
            _altTimeFormat = false;
            _lastRecordedSliderSetting = 0;

            // Cache references to other UI objects
            _textButtonPause = _buttonPause.GetComponentInChildren<Text>();
            _textButtonDate  = _buttonDate .GetComponentInChildren<Text>();

            // Set UI parameters here
            _sliderSpeed.minValue = 0;
            _sliderSpeed.maxValue = 5;
            _sliderSpeed.value = _sliderSpeed.minValue;
            _sliderSpeed.wholeNumbers = true;
            _textButtonPause.text = "SPEED: " + 1;

            // Add event listeners here
            _sliderSpeed.onValueChanged.AddListener(UpdateTimeScale);
            _buttonPause.onClick.AddListener(TogglePause);
            _buttonDate .onClick.AddListener(ToggleTimeFormat);

            // Set progress bar
            _progressBar.UpdateFill(0, Timekeeper._ticksPerEpisodicDay, 0);
        }

        // FixedUpdate occurs ever 0.02 seconds (20 milliseconds), resulting in an effective tickrate
        // of 50 ticks per second
        // Apparently this is far cleaner than a coroutine; the tickCount increments by 1 per call
        // and that count is used for the date; note that the order of operations is important here
        // (updating the text THEN incrementing the tick count); doing this the other way results in
        // a phantom 29 or 36 occurring for a brief moment (EG, 36 FEB or JAN 29 when it should be
        // 01 MAR or 01 FEB)
        private void FixedUpdate() {
            _textButtonDate.text = (_altTimeFormat) ? ("Day: " + (Timekeeper.EpisodicDay(_tickCount) + 1) + " (" + Timekeeper.EpisodicTime(_tickCount) + ":" + UnityEngine.Random.Range(0, 59).ToString("00") + ")") : ("Date: " + Timekeeper.SimpleDate(_tickCount));
            _tickCount++;
            int tickProgress = _tickCount % Timekeeper._ticksPerEpisodicDay;
            _progressBar.UpdateFill(tickProgress);
        }

        //private IEnumerator IncrementTickCount() {
        //    while (_clockActive) {
        //        Debug.Log(Time.timeScale);

        //        _tickCount++;
        //        _textDate.text = "Date: " + Timekeeper.SimpleDate(_tickCount) + " (" + Timekeeper.EpisodicTime(_tickCount) + ":" + Random.Range(0, 59).ToString("00") + ")";
        //        yield return new WaitForSeconds(_defaultTimeBetweenTicks);
        //    }
        //}

        // For adjusting the time scale in the game (which effectively adjusts the speed of time)
        // Note that the time slider can be adjusted while the game is paused
        private void UpdateTimeScale(float updatedValue) {
            if (_timeActive) {
                Time.timeScale = Mathf.Pow(2, updatedValue);
                _textButtonPause.text = "SPEED: " + Time.timeScale;
            } else {
                _lastRecordedSliderSetting = (int)_sliderSpeed.value;
            }
        }

        // Toggle button for pausing the game
        private void TogglePause() {
            // Clicking to pause the game
            if (_timeActive) {
                Time.timeScale = 0;
                _textButtonPause.text = "PAUSED";
                _timeActive = false;
                _lastRecordedSliderSetting = (int)_sliderSpeed.value;
            } else {
                Time.timeScale = Mathf.Pow(2, _lastRecordedSliderSetting);
                _textButtonPause.text = "SPEED: " + Time.timeScale;
                _timeActive = true;
            }
        }

        // Toggle how the date is displayed
        // Current options are:
        // - SYM454 calendar (without leap year calculations)
        // - "Episodic Time"
        private void ToggleTimeFormat() {
            if (_altTimeFormat) _altTimeFormat = false;
            else _altTimeFormat = true;
        }
    }
}
