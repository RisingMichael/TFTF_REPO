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
    private const string apiAdress = "https://tftf-new.herokuapp.com/classification/";

    private const float meleePushForce = 0.8f;
    private const float rangePushForce = 2.5f;
    private const float meleeCooldown = 0.3f;
    private const float rangeCooldown = 1.0f;

    private const string rangedString = "range";
    private const string meleeString = "melee";
    private const string noWeaponString = "other";

    private const int minDamage = 0;
    private const float strLengthToDamageMod = 0.34f;
    private const float maxTypeSpeedDamageMod = 1.5f;
    private const float typoDamageMod = 0.5f;

    private const int minAmountUses = 1;
    private const int rangeAmountUses = 3;
    private const int meleeAmountUses = 5;

    private const float mediumDamageThreshold = 2.0f; 
    private const float highDamageThreshold = 4.0f;

    private Sprite duckSprite;

    private Sprite badMeleeWeaponSprite;
    private Sprite mediumMeleeWeaponSprite;
    private Sprite goodMeleeWeaponSprite;

    private Sprite rangeWeaponSprite;

    private Sprite badProjectileSprite;
    private Sprite mediumProjectileSprite;
    private Sprite goodProjectileSprite;


    private void Awake()
    {
        TextInputManager.OnInputReceived += ReadWeaponData;

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

    private void ReadWeaponData(string givenInput, float percTimeSpent)
    {
        StartCoroutine(GetRequest(givenInput, percTimeSpent));
    }

    private int CalculateDamage(string givenInput, float percTimeSpent, ReadData data)
    {
        if (data.type == noWeaponString) return minDamage;
        if (data.recognizedWord.Length == 0) return minDamage;

        //base damage
        int damage = 1;

        //increase damage using the length of the given string
        damage += (int)(givenInput.Length * strLengthToDamageMod);

        //modify damage using the time spent typing in the string
        int addDamage = 0;
        if (percTimeSpent > 0.7f) addDamage = 1;
        if (percTimeSpent < 0.3f) addDamage = -1;
        damage += addDamage;

        //modify damage using amount of typos to damage
        damage -= data.typoAmount;
        damage = Mathf.Max(1, damage);

        //modify damage using the last input strings
        LinkedList<string> lastInputs = GameManager.instance.textInputManager.lastInputStrings;
        //TODO: actually do the calculation based on last inputs here

        return damage;
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
                    damage = CalculateDamage(givenInput, percTimeSpent, data);
                    weaponSprite = ChooseWeaponSprite(damage, data);
                    projectileSprite = ChooseProjectileSprite(damage, data);
                    isRanged = data.type == rangedString;
                    if (data.type == rangedString) cooldown = rangeCooldown;
                    if (data.type == rangedString) pushForce = rangePushForce;

                    break;
            }

            //create weapondata using the values
            WeaponData weaponData = new WeaponData(givenInput, uses, value, damage,
                pushForce, cooldown, isRanged, weaponSprite, projectileSprite);

            GameManager.instance.player.GetComponentInChildren<Weapon>().InitializeWithNewWeapon(weaponData);

        }
    }

    private class ReadData
    {
        public string recognizedWord;
        public string type;
        public int typoAmount;
    }
}



