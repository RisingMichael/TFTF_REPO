using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Collectable
{
    public Sprite emptyChest;
    public int coinsAmount = 5;

    protected override void OnCollect()
    {
        if(!collected)
        {
            collected = true;
            GetComponent<SpriteRenderer>().sprite = emptyChest;
            GameManager.instance.AddCoins(coinsAmount);
            GameManager.instance.ShowText("+" + coinsAmount.ToString() + " Coins!", 60, Color.yellow, transform.position, Vector3.up * 25, 3.0f);
            Debug.Log("Granted " + coinsAmount + " Coins!");
        }
    }
}
