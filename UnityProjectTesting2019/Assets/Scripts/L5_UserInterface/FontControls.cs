using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Because I'm lazy and I don't wanna have to set aesthetic properties each time
// This class can be made to be a generalized aesthetics toggling class; basically it can
// change the aesthetics of the UI between 2 or more presets

namespace PlayerControls {

    public class FontControls : MonoBehaviour {

        // Refences to UI Objects
        //[Header("UI Panels")]
        //[SerializeField] private Image[] _uiPanels;

        // Aesthetic properties
        [Header("A E S T H E T I C  P R O P S")]
        public Color _textColor;
        //public Color _residentialColor;
        //public Color _commercialColor;
        //public Color _civicColor;
        //public Color _uiColor;

        public Font _defaultFont;
        public Font _altFont;

        private bool _altFontEnabled;
        private List<Image> _uiPanels = new List<Image>();
        private List<Text>  _uiText   = new List<Text> ();

        private void Start() {
            _altFontEnabled = false;

            // If this script is directly attached to the UI canvas, then all of the UI
            // panels are children of the canvas; also, GetComponentsInChildren is recursive
            // so it also gets the children of the children and so on (basically everything
            // in the UI of a particular UI element type)
            gameObject.GetComponentsInChildren(_uiText);
            _uiPanels = GetUIPanels();


            //SetPanelColors();
            SetTextColors();
            SetDefaultFont();
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Space) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))) {
                ToggleFont();
            }
        }

        // This function basically gets all the image UI objects with the appropriate tags
        // Current tags are:
        // UI-GENERAL
        // UI-RESIDENTIAL
        // UI-COMMERCIAL
        // UI-CIVIC
        private List<Image> GetUIPanels() {
            List<Image> tempList = new List<Image>();
            List<Image> returnList = new List<Image>();
            gameObject.GetComponentsInChildren(tempList);
            foreach (Image i in tempList) {
                if (i.tag != "Untagged") returnList.Add(i);
            }
            return returnList;
        }

        // Self-explanatory...
        //private void SetPanelColors() {
        //    foreach (Image i in _uiPanels) {
        //        switch (i.tag) {
        //            case "UI-RESIDENTIAL": i.color = _residentialColor; break;
        //            case "UI-COMMERCIAL" : i.color = _commercialColor ; break;
        //            case "UI-CIVIC"      : i.color = _civicColor      ; break;
        //            default:               i.color = _uiColor         ; break;
        //        }
        //    }
        //}

        // Self-explanatory...
        private void SetTextColors() {
            foreach (Text t in _uiText) t.color = _textColor;
        }

        // Self-explanatory...
        private void SetDefaultFont() {
            foreach (Text t in _uiText) t.font = _defaultFont;
        }

        // Self-explanatory...
        private void SetAlternateFont() {
            foreach (Text t in _uiText) t.font = _altFont;
        }

        // Self-explanatory...
        private void ToggleFont() {
            if (_altFontEnabled) {
                _altFontEnabled = false;
                SetDefaultFont();
            } else {
                _altFontEnabled = true;
                SetAlternateFont();
            }
        }
    }
}