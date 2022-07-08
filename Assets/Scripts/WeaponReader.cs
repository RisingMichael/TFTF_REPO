using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;

public class WeaponReader : MonoBehaviour
{
    /// <summary>
    /// string given: the last seven concatenated input strings 
    /// </summary>
    public static event Action<string> OnSavedStringsChanged;

    private const string apiAdress = "https://tftf-new.herokuapp.com/classification/";

    private const float meleePushForce = 25.0f;
    private const float rangePushForce = 2.0f;
    private const float meleeCooldown = 0.3f;
    private const float rangeCooldown = 1.0f;

    private const string rangedString = "range";
    private const string meleeString = "melee";
    private const string noWeaponString = "other";

    private const int minDamage = 0;
    private const int minWeaponDamage = 1;
    private const int lengthDamageStep = 3;
    private const int damagePerLengthStep = 5;

    private const int timeDamageBase = 6;

    private const int minAmountUses = 1;
    private const int rangeAmountUses = 3;
    private const int meleeAmountUses = 5;

    private const float mediumDamageThreshold = 10.0f; 
    private const float highDamageThreshold = 20.0f;

    private Sprite duckSprite;

    private Sprite badMeleeWeaponSprite;
    private Sprite mediumMeleeWeaponSprite;
    private Sprite goodMeleeWeaponSprite;

    private Sprite rangeWeaponSprite;

    private Sprite badProjectileSprite;
    private Sprite mediumProjectileSprite;
    private Sprite goodProjectileSprite;


    private const int lastStringCap = 7;
    private const int lineSize = 35;
    private LinkedList<string> lastStrings;

    public LinkedList<string> lastInputStrings { get => lastStrings; }

    private void Awake()
    {
        lastStrings = new LinkedList<string>();

        //read out sprites directly from the resources folder
        Sprite[] spriteAtlas = Resources.LoadAll<Sprite>("Artwork\\Atlas");
        foreach (Sprite sprite in spriteAtlas)
        {
            switch (sprite.name)
            {
                case "Weapon_0":
                    badMeleeWeaponSprite = sprite;
                    break;
                case "Weapon_3":
                    mediumMeleeWeaponSprite = sprite;
                    break;
                case "Weapon_6":
                    goodMeleeWeaponSprite = sprite;
                    break;
                default: break;
            }
        }
        duckSprite = Resources.Load<Sprite>("Artwork\\Duck");
        rangeWeaponSprite = Resources.Load<Sprite>("Artwork\\Bow");
        badProjectileSprite = Resources.Load<Sprite>("Artwork\\Projectile1");
        mediumProjectileSprite = Resources.Load<Sprite>("Artwork\\Projectile2");
        goodProjectileSprite = Resources.Load<Sprite>("Artwork\\Projectile3");
    }

    public void ReadWeaponData(string givenInput, float percTimeSpent)
    {
        //enact penalty here
        if (givenInput.Length == 0)
        {
            GameManager.instance.EnactCoinPenalty(true);
            return;
        }

        StartCoroutine(GetRequest(givenInput, percTimeSpent));
    }

    private int CalculateDamage(string givenInput, float percTimeSpent, bool isDuplicate, ReadData data)
    {
        if (data.type == noWeaponString) return minDamage;
        if (data.recognizedWord.Length == 0) return minDamage;

        //base damage
        int damage = minWeaponDamage;

        //increase damage using the length of the given string
        damage += (int)Math.Round((float)givenInput.Length / (float)lengthDamageStep) * damagePerLengthStep;

        //modify damage using the time spent typing in the string
        int timeDamage = (int)Math.Round(timeDamageBase * percTimeSpent);
        if (percTimeSpent < 0.25f) timeDamage = 0;
        damage += timeDamage;

        //modify damage using amount of typos to damage
        int typoDmgReduction = (damage / 4) * data.typoAmount;
        damage -= typoDmgReduction;

        //modify damage using the last input strings    
        if (isDuplicate) damage /= 2;

        damage = Mathf.Max(minWeaponDamage, damage);
        return damage;
    }

    private bool DecideIfDuplicate(string givenInput)
    {
        int numberOfSimilarities = 0;
        foreach (string i in lastInputStrings)
        {
            if (i == givenInput) numberOfSimilarities++;
        }

        return numberOfSimilarities > 0;
    }

    private float CalculatePushForce(int damage, bool isRanged)
    {
        if (!isRanged)
        {
            if (damage >= highDamageThreshold) return meleePushForce * 1.4f;
            if (damage >= mediumDamageThreshold) return meleePushForce * 1.8f;
            return meleePushForce;
        }

        if (damage >= highDamageThreshold) return rangePushForce * 1.5f;
        if (damage >= mediumDamageThreshold) return rangePushForce * 2.0f;
        return rangePushForce;
    }

    private int CalculateUses(string givenInput, ReadData data)
    {
        if (data.type == noWeaponString) return minAmountUses;
        if (data.recognizedWord.Length == 0) return minAmountUses;

        if (data.type == rangedString) return rangeAmountUses;

        return meleeAmountUses;
    }

    private Sprite ChooseWeaponSprite(float damage, ReadData data)
    {
        if (data.type == noWeaponString) return duckSprite;
        if (data.type == rangedString) return rangeWeaponSprite;

        if (damage >= highDamageThreshold) return goodMeleeWeaponSprite;
        if (damage >= mediumDamageThreshold) return mediumMeleeWeaponSprite;
        return badMeleeWeaponSprite;
    }

    private Sprite ChooseProjectileSprite(float damage, ReadData data)
    {
        if (data.type == noWeaponString) return null;
        if (data.type != rangedString) return null;

        if (damage >= highDamageThreshold) return goodProjectileSprite;
        if (damage >= mediumDamageThreshold) return mediumProjectileSprite;
        return badProjectileSprite;
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

    IEnumerator GetRequest(string givenInput, float percTimeSpent)
    {    
        string processedInput = givenInput.Replace(' ', '_');
        string fullUriString = apiAdress + processedInput;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(fullUriString))
        {
            yield return webRequest.SendWebRequest();

            int uses = minAmountUses;
            int value = 1;
            int damage = minDamage;
            Sprite weaponSprite = duckSprite;
            Sprite projectileSprite = null;
            bool isRanged = false;
            float cooldown = meleeCooldown;
            float pushForce = meleePushForce;
            bool isDuplicate = false;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                    //if an error occured a duck is produced 
                    break;
                case UnityWebRequest.Result.Success:

                    //if request is successful, the weapon is read out 
                    ReadData data = JsonUtility.FromJson<ReadData>(
                        webRequest.downloadHandler.text);

                    uses = CalculateUses(givenInput, data);
                    isDuplicate = DecideIfDuplicate(givenInput);
                    damage = CalculateDamage(givenInput, percTimeSpent, isDuplicate, data);
                    weaponSprite = ChooseWeaponSprite(damage, data);
                    projectileSprite = ChooseProjectileSprite(damage, data);
                    isRanged = data.type == rangedString;
                    pushForce = CalculatePushForce(damage, isRanged);
                    if (isRanged) cooldown = rangeCooldown;
                    if (data.type == noWeaponString) GameManager.instance.EnactCoinPenalty(false);

                    break;
            }

            //create weapondata using the values
            WeaponData weaponData = new WeaponData(givenInput, uses, value, damage,
                pushForce, cooldown, isRanged, weaponSprite, projectileSprite, isDuplicate);

            GameManager.instance.player.GetComponentInChildren<Weapon>().InitializeWithNewWeapon(weaponData);
            if (givenInput.Length > 0) SaveString(givenInput);
        }
    }

    private class ReadData
    {
        public string recognizedWord;
        public string type;
        public int typoAmount;
    }
}



