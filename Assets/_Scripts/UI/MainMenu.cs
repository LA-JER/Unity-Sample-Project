using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioSource buttonClick;
    [SerializeField] private GameObject statsPage;

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
        StatsPage.Instance.ToggleScreen();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
