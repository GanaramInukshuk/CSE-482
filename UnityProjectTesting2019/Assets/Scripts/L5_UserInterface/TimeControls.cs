using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// The TimeControls script is used to count up game ticks and also adjusts the
// speed at which ticks count up; note that the Timekeeper script is only used
// to interpret the tick count into a meaningful time and date

namespace PlayerControls {

    [System.Serializable]
    public class TimeControls : MonoBehaviour {

        // Main UI objects
        [Header("Main UI Objects")]
        public Button      _buttonPause;
        public Button      _buttonDate;
        public Image       _image;
        public Slider      _sliderSpeed;
        public ProgressBar _progressBar;

        [Header("Main Parameters")]
        public int _tickCount;
        public int _yearOffset = 2000;      // So that the counter can start at, say, year 2000
        public float _timeOfDayOffset = 0.25f;      // So that the episodic day can start at, say, 06:00
        public int _tickRate = 60;          // Tickrate; divide 1 by this number to get the fixedDelatTime

        // Aesthetic properties
        [Header("A E S T H E T I C  P R O P S")]
        public Color _color;
        public Font  _defaultFont;
        public Font  _altFont;

        [Header("Other Parameters")]
        public int    _fineIncrements    = 100;       // Subdivisions; EG, if the slider goes from 1 to 5 and the slider goes by increments of 0.01
        public float  _textBlinkDuration = 1.0f;      // Blink period
        public bool   _timePaused    = false;         // For pausing/unpausing time
        public bool   _altTimeFormat = false;
        public string _timeFormat    = "00.00";       // Time format

        // Test items
        [Header("Debug")]
        //[SerializeField] private float  _secondsPerTick = 1f / 30;      // For debugging

        // Internal parameters
        private float  _lastRecordedTimeScale;
        private Text   _textButtonPause;
        private Text   _textButtonDate;
        private string _pauseText = "PAUSED";       // The text for the pause button to show when the game is paused
        private string _lastRecordedTimeString1;    // For recording the last time string for the default time format
        private string _lastRecordedTimeString2;    // For recording the last time string for the alternate time format
        private bool   _coroutineActive = false;    // Semaphore for coroutine; prevents it from being activated more than once

        public int TickCount {
            set { _tickCount = Mathf.Max(0, value); }
            get => _tickCount;
        }

        // Start is called before the first frame update
        void Awake() {
            // Set important member values
            _lastRecordedTimeScale = 1;
            //Time.fixedDeltaTime = 1.0f / _tickRate;

            // Cache references to other UI objects
            _textButtonPause = _buttonPause.GetComponentInChildren<Text>();
            _textButtonDate  = _buttonDate .GetComponentInChildren<Text>();

            // Set UI parameters here
            _sliderSpeed.minValue = 0;
            _sliderSpeed.maxValue = 2 * _fineIncrements;
            _sliderSpeed.value = _sliderSpeed.minValue;
            _sliderSpeed.wholeNumbers = false;
            _textButtonPause.text = "SPEED: 1.00";

            // Add event listeners here
            _sliderSpeed.onValueChanged.AddListener(UpdateTimeScale);
            _buttonPause.onClick.AddListener(TogglePause);
            
            _buttonDate .onClick.AddListener(ToggleTimeFormat);

            // Set progress bar
            _progressBar.UpdateFill(0, Timekeeper._ticksPerEpisodicDay, 0);
        }

        // FixedUpdate occurs ever 0.02 seconds (20 milliseconds), resulting in an effective tickrate
        // of 50 ticks per second (note that this is configurable, but I've left it at the default)
        // Apparently this is far cleaner than a coroutine; the tickCount increments by 1 per call
        // and that count is used for the date; note that the order of operations is important here
        // (updating the text THEN incrementing the tick count); doing this the other way results in
        // a phantom 29 or 36 occurring for a brief moment (EG, 36 FEB or JAN 29 when it should be
        // 01 MAR or 01 FEB)
        private void FixedUpdate() {
            // This is to cache the time strings for both time formats
            _lastRecordedTimeString1 = Timekeeper.SimpleDate(_tickCount, _yearOffset);
            _lastRecordedTimeString2 = "Day: " + (Timekeeper.EpisodicDay(_tickCount) + 1) + " (" + Timekeeper.EpisodicTime(_tickCount, _timeOfDayOffset) + /*":" + UnityEngine.Random.Range(0, 59).ToString("00") +*/ ")";

            // Update date/time text
            UpdateTimeText();
            _tickCount++;

            // Update progress bar
            // The progress bar shows the time until the next game update (IE, financial updates)
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
                if (!_coroutineActive) StartCoroutine(AlternatePauseText());
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
        // Note that FixedUpdate changes the time string every time it's called, so when the game is paused, the text can't get updated
        // whenever this is called, hence the additional IF statement that changes the time text when the game is paused
        private void ToggleTimeFormat() {
            _altTimeFormat = !_altTimeFormat;
            if (_timePaused) UpdateTimeText();
        }

        // Coroutine to alternate between PAUSED and time speed when the game is paused
        // Since the coroutine needs to run while timescale is zero (the game is paused),
        // use WaitForSecondsRealtime instead
        private IEnumerator AlternatePauseText() {
            _coroutineActive = true;
            WaitForSecondsRealtime wfs = new WaitForSecondsRealtime(_textBlinkDuration);
            while (_timePaused) {
                _textButtonPause.text = _pauseText;
                yield return wfs;
                _textButtonPause.text = "SPEED: " + _lastRecordedTimeScale.ToString(_timeFormat);
                yield return wfs;
            }
            _coroutineActive = false;
        }

        // Never mind, I've made the update-time-text line of code into its own function
        private void UpdateTimeText() {
            _textButtonDate.text = _altTimeFormat ? _lastRecordedTimeString2 : _lastRecordedTimeString1;
        }

        //private void HoverOverPauseButton() {
        //    _textButtonPause.text = "PAUSE";
        //}
    }
}
