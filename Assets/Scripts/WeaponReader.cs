using UnityEngine;
using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;

public class WeaponReader
{
    private const float pushForce = 0.2f;
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

    private Weapon weaponLogic;

    private Sprite duckSprite;

    private Sprite badMeleeWeaponSprite;
    private Sprite mediumMeleeWeaponSprite;
    private Sprite goodMeleeWeaponSprite;

    private Sprite rangeWeaponSprite;

    private Sprite badProjectileSprite;
    private Sprite mediumProjectileSprite;
    private Sprite goodProjectileSprite;

    public WeaponReader(Weapon weaponLogic)
    {
        this.weaponLogic = weaponLogic;
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
        APIHelper.ReadData data = APIHelper.GetData(givenInput);

        //calculate every value
        int uses = CalculateUses(givenInput, data);
        int value = 1;
        int damage = CalculateDamage(givenInput, percTimeSpent, data);
        Sprite weaponSprite = ChooseWeaponSprite(damage, data);
        Sprite projectileSprite = ChooseProjectileSprite(damage, data);
        bool isRanged = data.type == rangedString;

        float cooldown = meleeCooldown;
        if (data.type == rangedString) cooldown = rangeCooldown;

        //create weapondata using the values
        WeaponData weaponData = new WeaponData(givenInput, uses, value, damage,
            pushForce, cooldown, isRanged, weaponSprite, projectileSprite);

        weaponLogic.InitializeWithNewWeapon(weaponData);
    }

    private int CalculateDamage(string givenInput, float percTimeSpent, APIHelper.ReadData data)
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

        return damage;
    }

    private int CalculateUses(string givenInput, APIHelper.ReadData data)
    {
        if (data.type == noWeaponString) return minAmountUses;
        if (data.recognizedWord.Length == 0) return minAmountUses;

        if (data.type == rangedString) return rangeAmountUses;

        return meleeAmountUses;
    }

    private Sprite ChooseWeaponSprite(float damage, APIHelper.ReadData data)
    {
        if (data.type == noWeaponString) return duckSprite;
        if (data.type == rangedString) return rangeWeaponSprite;

        if (damage >= highDamageThreshold) return goodMeleeWeaponSprite;
        if (damage >= mediumDamageThreshold) return mediumMeleeWeaponSprite;
        return badMeleeWeaponSprite;
    }

    private Sprite ChooseProjectileSprite(float damage, APIHelper.ReadData data)
    {
        if (data.type == noWeaponString) return null;
        if (data.type != rangedString) return null;

        if (damage >= highDamageThreshold) return goodProjectileSprite;
        if (damage >= mediumDamageThreshold) return mediumProjectileSprite;
        return badProjectileSprite;
    }
}

public static class APIHelper
{
    private const string apiAdress = "https://tftf-new.herokuapp.com/classification/";

    public class ReadData
    {
        public string recognizedWord;
        public string type;
        public int typoAmount;
    }

    public static ReadData GetData(string input)
    {
        string processedInput = input.Replace(' ', '_');
        string fullUriString = apiAdress + processedInput;
        try
        {
            //contact server
            //TODO (Maybe): thread this part of the code since this part costs a lot of performance
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullUriString);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string json = reader.ReadToEnd();
            return JsonUtility.FromJson<ReadData>(json);
        }
        catch (Exception)
        {
            return new ReadData { recognizedWord = "", type = "other", typoAmount = 0 };
        }


    }

}



