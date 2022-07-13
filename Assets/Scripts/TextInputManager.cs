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

    public const float timeLimitInSec = 3.0f;
    public const float timeBufferInSec = 1.0f;
    public const float tutorialBufferInSec = 5.0f;
    private const float slowDownMod = 0.0001f;
    private const float coolDown = 1.0f;

    private bool textInputActivated = false;
    private bool canBeActivated = true;

    private int usageCounter = 0;

    [SerializeField]
    private GameObject inputFieldObject;

    private float secsLeft = timeLimitInSec + timeBufferInSec;

    public bool isActive { get => textInputActivated; }

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

        // Play typing music
        GameManager.instance.PlayTypingTrack();
    }

    public void DeactivateTextInput()
    {
        if (!textInputActivated) return;
        textInputActivated = false;
        string input = inputFieldObject.GetComponent<TMP_InputField>().text;

        inputFieldObject.GetComponent<TMP_InputField>().text = ""; //empty text input
        inputFieldObject.GetComponent<TMP_InputField>().DeactivateInputField();

        OnChangeActiveState?.Invoke(false); //deactivate Ui

        GameManager.instance.weaponReader.ReadWeaponData(input, percentageOfTimeLeft);
        Time.timeScale = 1.0f;
        StartCoroutine(CoolDown());

        // Play main theme
        GameManager.instance.PlayMainTrack();
    }

    public float percentageOfTimeLeft { get => Mathf.Min(timeLimitInSec, secsLeft) / timeLimitInSec; }

    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(coolDown);
        canBeActivated = true;
    }

}
