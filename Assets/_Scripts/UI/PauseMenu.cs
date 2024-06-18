using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public delegate void PauseHandler(bool isPaused);
    public static event PauseHandler onPauseToggle;

    private bool canPause = true;
    private bool isPaused = false;
    private float previousTimeScale = 1f;

    private void Start()
    {
        if(pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        if (pauseMenuUI != null)
        {
            isPaused = !pauseMenuUI.activeSelf;
            pauseMenuUI.SetActive(isPaused);
            onPauseToggle?.Invoke(isPaused);
            // Pause or resume the game based on the pause state
            Time.timeScale = isPaused ? 0f : previousTimeScale;

        }
    }

    public void ToggleDamageNumbers()
    {
        int current = DataHandler.GetFlagInt(DataHandler.Flag.showDamageNumbers);
        int toggle = current > 0 ? 0 : 1;
        DataHandler.SetFlag(DataHandler.Flag.showDamageNumbers, toggle);
    }


    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
        //SceneTransitioner.Instance.LoadLevel(0);
        Time.timeScale = 1f;
    }
    public void Restart()
    {
        //SceneTransitioner.Instance.ReloadLevel();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }
}
