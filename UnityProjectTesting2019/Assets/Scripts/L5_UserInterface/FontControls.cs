using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Because I'm lazy and I don't wanna have to set aesthetic properties eah time

namespace PlayerControns {

    public class FontControls : MonoBehaviour {

        // Refences to UI Objects
        //[Header("UI Panels")]
        //[SerializeField] private Image[] _uiPanels;

        // Aesthetic properties
        [Header("A E S T H E T I C  P R O P S")]   
        [SerializeField] private Color _textColor;
        [SerializeField] private Color _residentialColor;
        [SerializeField] private Color _commercialColor;
        [SerializeField] private Color _civicColor;
        [SerializeField] private Color _uiColor;

        [SerializeField] private Font _defaultFont;
        [SerializeField] private Font _altFont;

        private bool _altFontEnabled;
        private List<Image> _uiPanels = new List<Image>();
        private List<Text>  _uiText   = new List<Text> ();

        private void Start() {
            _altFontEnabled = false;

            // If this script is directly attached to the UI canvas, then all of the UI
            // panels are children of the canvas
            gameObject.GetComponentsInChildren(_uiText);
            _uiPanels = GetUIPanels();


            SetPanelColors();
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
        private void SetPanelColors() {
            foreach (Image i in _uiPanels) {
                switch (i.tag) {
                    case "UI-RESIDENTIAL": i.color = _residentialColor; break;
                    case "UI-COMMERCIAL" : i.color = _commercialColor ; break;
                    case "UI-CIVIC"      : i.color = _civicColor      ; break;
                    default:               i.color = _uiColor         ; break;
                }
            }
        }

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