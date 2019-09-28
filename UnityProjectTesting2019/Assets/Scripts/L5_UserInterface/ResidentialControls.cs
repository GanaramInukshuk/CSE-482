using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ResidentialScripts;

// This script should inherit from MonoBehaviour and therefore should be attached to a UI object
// Attach this to the relevant UI panel and have this script be composed of its relevant simulator and evaluator

namespace PlayerControls {

    [System.Serializable]
    public class ResidentialControls : MonoBehaviour {

        [Header("Main Labels")]

        // Text label to display total housing
        [SerializeField]
        private Text _labelTotalHousing;

        // Text array to display housing by type; the code's the same for each type, so an array is used
        // so it can be looped; this also helps if additional housing sizes are added
        [SerializeField]
        private Text[] _labelHousingType;

        [Header("References to Other Controls")]

        // For testing purposes?
        // By doing this, this script can directly control the ResidentialSimulator, wherever it's attached
        [SerializeField]
        private GameObject _objectWithResidentialSimulator;

        [Header("Test Items")]

        // Text label for testing purposes
        [SerializeField]
        private Text _textToTest;

        // Manager and evaluator
        private ResidentialSimulator _resSim;

        // Start is called before the first frame update
        void Start() {
            _resSim = _objectWithResidentialSimulator.GetComponent<ResidentialSimulator>();
            GenerateTextLabels();
        }

        // Clicking this button adds housing to the city; currently, this adds a single house
        // but should add an nplex instead depending on the population; think of how Minecraft's
        // fortune mechanic works here
        public void ResidentialIncrement() {
            Debug.Log("[ResidentialControls]: Increment button pressed.");

            if (_resSim != null && _textToTest != null) {
                _resSim.IncrementBldgs(1, 0);
                GenerateTextLabels();

                _resSim.PrintDebugString();
            } else Debug.Log("[ResidentialControls]: Could not find ResidentialSimulator or text label(s).");
        }

        // Clicking this button decrements housing from the city; increments are basically random
        // but decrements are not, so the user has full control over what structures can be demolished;
        // this function therefore requires a parameter so the ResSim knows which housing type to demolish
        public void ResidentialDecrement(int type) {
            Debug.Log($"[ResidentialControls]: Decrement button pressed for housing type {(Constants.HOUSINGSIZE)type}.");

            if (_resSim != null) {
                _resSim.IncrementBldgs(-1, 0);
                GenerateTextLabels();

                _resSim.PrintDebugString();
            } else Debug.Log("[ResidentialControls]: Could not find ResidentialSimulator or text label(s).");
        }

        // Private helper classes
        private void GenerateTextLabels() {
            IHousing hsgBreakdown = _resSim.HousingBreakdown;

            // There should be a label for each level of housing, but for testing
            // purposes, there's just one label
            string newText = 
                "Total housing: " + hsgBreakdown.TotalHousing +
                ", Single houses: " + hsgBreakdown[0] +
                ", Duplexes: "      + hsgBreakdown[1] +
                ", Triplexes: "     + hsgBreakdown[2] + 
                ", Fourplexes: "    + hsgBreakdown[3];
            _textToTest.text = newText;
        }
    }
}