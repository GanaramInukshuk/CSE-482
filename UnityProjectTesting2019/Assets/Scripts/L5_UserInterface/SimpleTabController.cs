using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This script is for adding a simple tabbed menu system to the UI
// All this needs is an array of buttons and an array of tabbed pages corresponding
// to the buttons

public class SimpleTabController : MonoBehaviour {

    public Button[] tabButtons;
    public Image [] tabPages;
    public int initialTabToOpen = 0;

    // The start function sets the first page to be displayed by default
    private void Start() {
        for (int i = 0; i < tabButtons.Length; i++) {
            tabPages[i].gameObject.SetActive(true);
            tabPages[i].gameObject.SetActive(false);
        }
        tabButtons[initialTabToOpen].interactable = false;
        tabPages[initialTabToOpen].gameObject.SetActive(true);

        // Set up listeners to each button
        for (int i = 0; i < tabButtons.Length; i++) {
            int j = i;
            tabButtons[i].onClick.AddListener(() => SwapPages(j));
        }
    }

    // This goes through all the pages and hides them except for the one specified by
    // the page number, THEN specifically disables that page's corresponding button while
    // enabling all other buttons
    // Note: it's actually buttonName.interactable, not buttonName.enabled
    // I mean, it's pointless to swap pages when you're already on the page you want to swap to
    public void SwapPages(int pageNumber) {
        for (int i = 0; i < tabButtons.Length; i++) {
            if (pageNumber == i) {
                tabPages[i].gameObject.SetActive(true);
                tabButtons[i].interactable = false;
            } else {
                tabPages[i].gameObject.SetActive(false);
                tabButtons[i].interactable = true;
            }
        }
    }
}
