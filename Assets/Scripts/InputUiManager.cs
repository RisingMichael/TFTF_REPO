using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputUiManager : MonoBehaviour
{
    [SerializeField]
    private GameObject canvasObject;

    [SerializeField]
    private GameObject lastStringsObject;

    [SerializeField]
    private GameObject lastStringsTitleObject;

    private const string lastStringsTitle = "Last \"Items\" that you wanted:";

    [SerializeField]
    private GameObject timeBarObject;

    private bool isFirstDisplay = true;

    private float startBarWidth;

    private void Awake()
    {
        TextInputManager.OnChangeActiveState += ShowInputCanvas;
        TextInputManager.OnTimerUpdated += UpdateTimerBar;
        TextInputManager.OnSavedStringsChanged += UpdateLastStrings;
        

        startBarWidth = timeBarObject.GetComponent<RectTransform>().rect.width;
    }

    private void OnDestroy()
    {
        TextInputManager.OnChangeActiveState -= ShowInputCanvas;
        TextInputManager.OnTimerUpdated -= UpdateTimerBar;
        TextInputManager.OnSavedStringsChanged -= UpdateLastStrings;
    }

    private void UpdateLastStrings(string lastStrings)
    {
        if (isFirstDisplay)
        {
            lastStringsTitleObject.GetComponent<TMP_Text>().text = lastStringsTitle;
            lastStringsObject.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.TopLeft;
            isFirstDisplay = false;
        }
        lastStringsObject.GetComponent<TMP_Text>().text = lastStrings;        
    }

    private void ShowInputCanvas(bool show) => canvasObject.SetActive(show);

    private void UpdateTimerBar(float percentage)
    {
        RectTransform rectT = timeBarObject.GetComponent<RectTransform>();
        rectT.sizeDelta = new Vector2(startBarWidth * percentage, rectT.rect.height);
    }

}
