using System.Collections;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioSource buttonClick;
    [SerializeField] private GameObject statsPage;
    [SerializeField] private TextMeshProUGUI fullscreenText;
    public void Play()
    {
        if(buttonClick != null)
        {
            buttonClick.Play();
        }
        SceneTransitioner.Instance.LoadNextLevel();
    }

    public void ViewStats()
    {
        if (buttonClick != null)
        {
            buttonClick.Play();
        }
        StatsPage.Instance.ToggleScreen();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ToggleFullscreenMode()
    {
        if (buttonClick != null)
        {
            buttonClick.Play();
        }
        // Toggle the fullscreen mode
        Screen.fullScreen = !Screen.fullScreen;

        // Update the button text based on the new fullscreen mode
        if (fullscreenText != null)
        {

            fullscreenText.text = Screen.fullScreen ? "Enter Fullscreen" : "Exit Fullscreen";
        }
    }
}
