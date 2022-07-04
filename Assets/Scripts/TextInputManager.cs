using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class TextInputManager : MonoBehaviour
{
    /// <summary>
    /// float given: the percentage of time left (range 0 - 1)
    /// </summary>
    public static event Action<float> OnTimerUpdated;

    /// <summary>
    /// bool given: true := is active; false := not active
    /// </summary>
    public static event Action<bool> OnChangeActiveState;

    /// <summary>
    /// string given: the input string
    /// float given: the remaining seconds left in percentage (range 0 - 1)
    /// </summary>
    public static event Action<string, float> OnInputReceived;

    /// <summary>
    /// string given: the last seven concatenated input strings 
    /// </summary>
    public static event Action<string> OnSavedStringsChanged;

    public const float timeLimitInSec = 3.0f;
    public const float timeBufferInSec = 1.0f;
    public const float tutorialBufferInSec = 5.0f;
    private const float slowDownMod = 0.0001f;
    private const float coolDown = 1.0f;

    private const int lastStringCap = 7;
    private const int lineSize = 35;

    private bool textInputActivated = false;
    private bool canBeActivated = true;

    private int usageCounter = 0;

    private LinkedList<string> lastStrings;

    [SerializeField]
    private GameObject inputFieldObject;

    private float secsLeft = timeLimitInSec + timeBufferInSec;

    public bool isActive { get => textInputActivated; }

    public LinkedList<string> lastInputStrings { get => lastStrings; }

    private void Awake()
    {
        lastStrings = new LinkedList<string>();
    }

    private void Update()
    {
        if (!textInputActivated) return;

        //everything else in the game is slowed down
        //so we have to speed the TimeInputManager back up
        secsLeft -= Time.unscaledDeltaTime;
        secsLeft = Mathf.Max(secsLeft, 0.0f);

        //update the ui but only every 0.005 seconds and not every frame
        int secCounter = (int)(secsLeft * 100.0f);
        if (secCounter % 5 == 0) OnTimerUpdated?.Invoke(percentageOfTimeLeft); 

        if (secsLeft == 0.0f) DeactivateTextInput();
    }

    public void ActivateTextInput()
    {
        if (!canBeActivated || textInputActivated) return;        
        canBeActivated = false;

        float actualTutorialBuffer = Mathf.Max(0.0f, tutorialBufferInSec - (float)usageCounter);
        secsLeft = timeLimitInSec + timeBufferInSec + actualTutorialBuffer;
        usageCounter++;

        Time.timeScale = slowDownMod; //slow down game

        textInputActivated = true;
        OnChangeActiveState?.Invoke(true); // update Ui
        inputFieldObject.GetComponent<TMP_InputField>().ActivateInputField(); //so that the player can immediately type in something
    }

    public void DeactivateTextInput()
    {
        if (!textInputActivated) return;
        textInputActivated = false;
        string input = inputFieldObject.GetComponent<TMP_InputField>().text;

        inputFieldObject.GetComponent<TMP_InputField>().text = ""; //empty text input
        inputFieldObject.GetComponent<TMP_InputField>().DeactivateInputField();

        OnChangeActiveState?.Invoke(false); //deactivate Ui
        Time.timeScale = 1.0f;
        if (input.Length > 0)
        {
            OnInputReceived?.Invoke(input, percentageOfTimeLeft); //send out input and the time spent 
            SaveString(input);
        }
        StartCoroutine(CoolDown());
    }

    private void SaveString(string lastStr)
    {
        //add string to string list
        lastStrings.AddLast(lastStr);
        if (lastStrings.Count > lastStringCap) lastStrings.RemoveFirst();

        //create concatenated string to send to the UI
        string displayedString = "";
        foreach (string str in lastStrings)
        {
            string line = "- " + str + '\n';

            if (line.Length > lineSize)
            {
                line = line.Substring(0, lineSize);
                line += "...\n";
            }

            displayedString += line;
        }

        OnSavedStringsChanged?.Invoke(displayedString);
    }

    public float percentageOfTimeLeft { get => Mathf.Min(timeLimitInSec, secsLeft) / timeLimitInSec; }

    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(coolDown);
        canBeActivated = true;
    }

}
