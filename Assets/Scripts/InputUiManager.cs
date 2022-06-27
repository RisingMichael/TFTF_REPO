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

    private const int lastStringCap = 7;
    private const int lineSize = 35;

    private LinkedList<string> lastStrings;

    private void Awake()
    {
        lastStrings = new LinkedList<string>();

        TextInputManager.OnChangeActiveState += ShowInputCanvas;
        TextInputManager.OnTimerUpdated += UpdateTimerBar;
        TextInputManager.OnInputReceived += UpdateLastStrings;
        

        startBarWidth = timeBarObject.GetComponent<RectTransform>().rect.width;
    }

    private void UpdateLastStrings(string lastStr, float notImportant)
    {
        lastStrings.AddLast(lastStr);
        if (lastStrings.Count > lastStringCap) lastStrings.RemoveFirst();

        string result = "";
        foreach (string str in lastStrings)
        {
            string line = "- " + str + '\n';

            if (line.Length > lineSize)
            {
                line = line.Substring(0, lineSize);
                line += "...\n";
            }

            result += line;
        }       

        lastStringsObject.GetComponent<TMP_Text>().text = result;
    }

    private void ShowInputCanvas(bool show) => canvasObject.SetActive(show);

    private void UpdateTimerBar(float percentage)
    {
        RectTransform rectT = timeBarObject.GetComponent<RectTransform>();
        rectT.sizeDelta = new Vector2(startBarWidth * percentage, rectT.rect.height);
    }

}
