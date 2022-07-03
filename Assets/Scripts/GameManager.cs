using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    /// <summary>
    /// value int: the current amount of coins
    /// </summary>
    public static Action<int> OnCoinsChanged;

    //used when a non weapon is requested
    private const int smallCoinPenalty = 1;
    private const int smallHealthPenalty = 1;

    //used when no string was entered
    private const int bigCoinPenalty = 5;
    private const int bigHealthPenalty = 5;

    // Ressources
    public List<Sprite> playerSprites;
    public List<Sprite> weaponSprites;
    public List<int> weaponPrices;
    public List<int> xpTable;


    // References
    public Player player;
    public FloatingTextManager floatingTextManager;


    public TextInputManager textInputManager { get => GetComponent<TextInputManager>(); }
    public DisplayUiManager displayUiManager { get => GetComponent<DisplayUiManager>(); }
    public InputUiManager inputUiManager { get => GetComponent<InputUiManager>(); }


    // Logic
    [SerializeField]
    private int coins = 0;

    private void Awake()
    {
        instance = this;
        //if(GameManager.instance != null)
        //{
        //    Destroy(gameObject);
        //    return;
        //}


        //SceneManager.sceneLoaded += LoadState;
        //DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    public void print(string msg) => Debug.Log(msg);

    public void AddCoins(int newCoins)
    {
        coins += newCoins;
        OnCoinsChanged?.Invoke(coins);
    }

    public void EnactCoinPenalty(bool isBigPenalty)
    {
        int healthPenalty = smallHealthPenalty;
        int coinPenalty = smallCoinPenalty;

        if (isBigPenalty)
        {
            healthPenalty = bigHealthPenalty;
            coinPenalty = bigCoinPenalty;
        }

        if (coins == 0)
        {
            Damage dmg = new Damage
            {
                damageAmount = healthPenalty,
                origin = player.transform.position,
                pushForce = 0.0f
            };

            player.ReceiveDamage(dmg);
        }

        coins -= coinPenalty;
        coins = Mathf.Max(0, coins);
        OnCoinsChanged?.Invoke(coins);
    }

    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration);
    }

    //gets called when you press the "Again" button on the game over screen
    public void ReloadGame()
    {
        //reloads the scene!
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void SaveState()
    {
        string s = "";

        s += "0" + "|";
        s += coins.ToString() + "|";
        s += "0";

        PlayerPrefs.SetString("SaveState", s);
    }



    public void LoadState(Scene scene, LoadSceneMode mode)
    {
        if (!PlayerPrefs.HasKey("SaveState"))
            return;


        string[] data = PlayerPrefs.GetString("SaveState").Split('|');

        // TODO: Change player skin
        //coins = int.Parse(data[1]); //TODO BUG: this gave 45 coins at every start of the main scene
        // TODO: Change weapon level
    }
}
