using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayUiManager : MonoBehaviour
{

    [SerializeField]
    private GameObject healthBarObject;

    [SerializeField]
    private GameObject coinCounterObject;

    [SerializeField]
    private GameObject weaponNameObject;

    [SerializeField]
    private GameObject weaponDisplayObject;

    [SerializeField]
    private GameObject weaponDataObject;

    [SerializeField]
    private GameObject gameOverObject;

    private float startBarWidth;

    private void Awake()
    {
        GameManager.OnCoinsChanged += UpdateCoins;
        Player.OnHealthChanged += UpdateHealthBar;
        Weapon.OnWeaponInfoChanged += UpdateWeaponInfo;
        Player.OnDeath += ShowGameOverWindow;

        startBarWidth = healthBarObject.GetComponent<RectTransform>().rect.width;
    }

    private void OnDestroy()
    {
        GameManager.OnCoinsChanged -= UpdateCoins;
        Player.OnHealthChanged -= UpdateHealthBar;
        Weapon.OnWeaponInfoChanged -= UpdateWeaponInfo;
        Player.OnDeath -= ShowGameOverWindow;
    }

    private void ShowGameOverWindow() => gameOverObject.SetActive(true);

    private void UpdateHealthBar(float percentage)
    {
        RectTransform rectT = healthBarObject.GetComponent<RectTransform>();
        rectT.sizeDelta = new Vector2(startBarWidth * percentage, rectT.rect.height);
    }

    private void UpdateCoins(int coins)
    {
        string result = "x ";
        result += coins.ToString();
        coinCounterObject.GetComponent<TMP_Text>().text = result;
    }

    private void UpdateWeaponInfo(bool changeOnlyUses)
    {
        Weapon weaponLogic = GameManager.instance.player.gameObject.GetComponentInChildren<Weapon>();

        if (weaponLogic.isBroken)
        {
            //hide weapon display
            weaponNameObject.transform.parent.gameObject.SetActive(false);
            return;
        }

        //show weapon display
        weaponNameObject.transform.parent.gameObject.SetActive(true);

        string damageString = "Damage: " + weaponLogic.damagePoint + '\n';

        string type = "melee";
        if (weaponLogic.ranged) type = "ranged";
        string typeString = "Type: " + type + '\n';

        string usesString = "Uses: " + weaponLogic.uses + '\n';
        string duplicateString = "[DUPLICATE]";

        string resultString = damageString + typeString + usesString;
        if (weaponLogic.isDuplicate) resultString += duplicateString;

        weaponDataObject.GetComponent<TMP_Text>().text = resultString;

        if (changeOnlyUses) return;

        weaponNameObject.GetComponent<TMP_Text>().text = weaponLogic.weaponName;

        if (weaponLogic.ranged || weaponLogic.damagePoint == 0)
        {
            weaponDisplayObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100.0f, 100.0f);
        }
        else weaponDisplayObject.GetComponent<RectTransform>().sizeDelta = new Vector2(45.0f, 135.0f);

        weaponDisplayObject.GetComponent<Image>().sprite = weaponLogic.GetComponent<SpriteRenderer>().sprite;
    } 
}
