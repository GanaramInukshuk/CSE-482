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
        [Header("Main UI Objects")]
        [SerializeField] private Button      _buttonPause;
        [SerializeField] private Button      _buttonDate;
        [SerializeField] private Image       _image;
        [SerializeField] private Slider      _sliderSpeed;
        [SerializeField] private ProgressBar _progressBar;

        [Header("Main Parameters")]
        [SerializeField] private int  _tickCount;
        [SerializeField] private int  _yearOffset = 2000;      // So that the counter can start at, say, year 2000

        // Aesthetic properties
        [Header("A E S T H E T I C  P R O P S")]
        [SerializeField] private Color _color;
        [SerializeField] private Font  _defaultFont;
        [SerializeField] private Font  _altFont;

        [Header("Other Parameters")]
        [SerializeField] private int    _fineIncrements = 100;      // Subdivisions; EG, if the slider goes from 1 to 5 and the slider goes by increments of 0.01
        [SerializeField] private float  _textBlinkDuration = 1.0f;  // Blink period
        [SerializeField] private bool   _timePaused;                // For pausing/unpausing time
        [SerializeField] private bool   _altTimeFormat;
        [SerializeField] private string _timeFormat = "00.00";      // Time format

        // Test items
        [Header("Debug")]
        //[SerializeField] private float  _secondsPerTick = 1f / 30;      // For debugging

        private float  _lastRecordedTimeScale;
        private Text   _textButtonPause;
        private Text   _textButtonDate;
        private string _pauseText = "PAUSED";       // The text for the pause button to show when the game is paused
        

        // Start is called before the first frame update
        void Start() {
            // Set important member values
            _timePaused = false;
            _altTimeFormat = false;
            _lastRecordedTimeScale = 1;

            // Cache references to other UI objects
            _textButtonPause = _buttonPause.GetComponentInChildren<Text>();
            _textButtonDate  = _buttonDate .GetComponentInChildren<Text>();

            // Set UI parameters here
            _sliderSpeed.minValue = 0;
            _sliderSpeed.maxValue = 5 * _fineIncrements;
            _sliderSpeed.value = _sliderSpeed.minValue;
            _sliderSpeed.wholeNumbers = true;
            _textButtonPause.text = "SPEED: 1.00";

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
            // Update date/time text
            _textButtonDate.text = (_altTimeFormat) ? ("Day: " + (Timekeeper.EpisodicDay(_tickCount) + 1) + " (" + Timekeeper.EpisodicTime(_tickCount) + ":" + UnityEngine.Random.Range(0, 59).ToString("00") + ")") : ("Date: " + Timekeeper.SimpleDate(_tickCount, _yearOffset));
            _tickCount++;

            // Update progress bar
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
        // Also note that the slider has a max of 500, which should result in increments of 0.01;
        // the variable _lastRecordedSliderSetting takes the updated slider value and divides it
        // by 100; when the timescale needs to be changed, it can simply be used directly
        // Also note that the scale is logarithmic; instead of the scale going from 1-5, it goes
        // from 2^0 to 2^5, so it basically goes faster exponentially the higher the slider value
        private void UpdateTimeScale(float updatedValue) {
            _lastRecordedTimeScale = Mathf.Pow(2, updatedValue / _fineIncrements);
            if (!_timePaused) Time.timeScale = _lastRecordedTimeScale;
            if (_textButtonPause.text != _pauseText) _textButtonPause.text = "SPEED: " + _lastRecordedTimeScale.ToString(_timeFormat);
        }

        // Toggle button for pausing the game
        private void TogglePause() {
            if (!_timePaused) {
                _timePaused = !_timePaused;
                StartCoroutine(AlternatePauseText());
                Time.timeScale = 0;
            } else {
                _timePaused = !_timePaused;
                Time.timeScale = _lastRecordedTimeScale;
                _textButtonPause.text = "SPEED: " + _lastRecordedTimeScale.ToString(_timeFormat);
            }
        }

        // Toggle how the date is displayed
        // Current options are:
        // - SYM454 calendar (without leap year calculations)
        // - "Episodic Time"
        private void ToggleTimeFormat() {
            //if (_altTimeFormat) _altTimeFormat = !_altTimeFormat;
            /*else*/ _altTimeFormat = !_altTimeFormat;
        }

        // Coroutine to alternate between PAUSED and time speed when the game is paused
        // Since the coroutine needs to run while timescale is zero (the game is paused),
        // use WaitForSecondsRealtime instead
        private IEnumerator AlternatePauseText() {
            WaitForSecondsRealtime wfs = new WaitForSecondsRealtime(_textBlinkDuration);
            while (_timePaused) {
                _textButtonPause.text = _pauseText;
                yield return wfs;
                _textButtonPause.text = "SPEED: " + _lastRecordedTimeScale.ToString(_timeFormat);
                yield return wfs;
            }
        }
    }
}
