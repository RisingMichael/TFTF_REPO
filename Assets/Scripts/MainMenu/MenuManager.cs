using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) ExitGame();
    }

    public void StartGame() => SceneManager.LoadScene("Main");

    public void ExitGame() => Application.Quit();

}
