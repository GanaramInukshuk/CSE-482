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

    // The start function sets the first page to be displayed by default
    private void Start() {
        for (int i = 0; i < tabButtons.Length; i++) {
            if (i == 0) tabPages[i].gameObject.SetActive(true);
            else tabPages[i].gameObject.SetActive(false);
        }

        // Set up listeners to each button
        for (int i = 0; i < tabButtons.Length; i++) {
            int j = i;
            tabButtons[i].onClick.AddListener(() => SwapPages(j));
        }
    }

    public void SwapPages(int pageNumber) {
        for (int i = 0; i < tabButtons.Length; i++) {
            if (pageNumber == i) tabPages[i].gameObject.SetActive(true);
            else tabPages[i].gameObject.SetActive(false);
        }
    }
}
