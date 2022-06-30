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
    private GameObject timeBarObject;

    private float startBarWidth;

    private void Awake()
    {
        TextInputManager.OnChangeActiveState += ShowInputCanvas;
        TextInputManager.OnTimerUpdated += UpdateTimerBar;
        TextInputManager.OnSavedStringsChanged += UpdateLastStrings;
        

        startBarWidth = timeBarObject.GetComponent<RectTransform>().rect.width;
    }

    private void UpdateLastStrings(string lastStrings)
    {
        lastStringsObject.GetComponent<TMP_Text>().text = lastStrings;
    }

    private void ShowInputCanvas(bool show) => canvasObject.SetActive(show);

    private void UpdateTimerBar(float percentage)
    {
        RectTransform rectT = timeBarObject.GetComponent<RectTransform>();
        rectT.sizeDelta = new Vector2(startBarWidth * percentage, rectT.rect.height);
    }

}
