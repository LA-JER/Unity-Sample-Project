using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public delegate void PauseHandler(bool isPaused);
    public static event PauseHandler onPauseToggle;
    public static event PauseHandler onWorldStop;

    [SerializeField] private TextMeshProUGUI fullscreenText;

    private bool shouldPause = false;
    private float previousTimeScale = 1f;
    private bool isMenuUp = true;
    private bool toggle = false;

    private void Start()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        if (pauseMenuUI != null)
        {
            shouldPause = !pauseMenuUI.activeSelf;
            pauseMenuUI.SetActive(shouldPause);
            onPauseToggle?.Invoke(shouldPause);
            // Pause or resume the game based on the pause state
            Time.timeScale = shouldPause ? 0f : previousTimeScale;

        }
    }

    public void TogglePause()
    {

        //isPaused = !pauseMenuUI.activeSelf;
        //pauseMenuUI.SetActive(isPaused);
        //onPauseToggle?.Invoke(isPaused);
        // Pause or resume the game based on the pause state
        
        //Time.timeScale = shouldPaused ? 0f : previousTimeScale;
        toggle = !toggle;
        //onPauseToggle?.Invoke(toggle);
        onWorldStop?.Invoke(toggle);

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

    public void ToggleFullscreenMode()
    {
        // Toggle the fullscreen mode
        Screen.fullScreen = !Screen.fullScreen;

        // Update the button text based on the new fullscreen mode
        if (fullscreenText != null)
        {

            fullscreenText.text = Screen.fullScreen ? "Enter Fullscreen" : "Exit Fullscreen";
        }
    }

    public void ToggleMenu(Animator animator)
    {
        if (animator == null) return;

        if (isMenuUp)
        {
            animator.SetTrigger("Exit");
            
        } else
        {
            animator.SetTrigger("Entry");
        }
        isMenuUp = !isMenuUp;
    }
}
