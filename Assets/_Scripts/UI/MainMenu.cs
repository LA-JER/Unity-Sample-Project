using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioSource buttonClick;


    public void Play()
    {
        if(buttonClick != null)
        {
            buttonClick.Play();
        }
        SceneTransitioner.Instance.LoadNextLevel();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
