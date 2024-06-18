using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    public static SceneTransitioner Instance;


    public GameObject Fade;
    public string ExitFlag = "Exit";
    public string EnterFlag = "Enter";
    public AnimationClip fadeOutClip;
    
    private Animator animator;


    private float exitTime = 1f;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Fade != null)
        {
            animator = Fade.GetComponent<Animator>();


            Fade.SetActive(true);
        }
        if (fadeOutClip)
            exitTime = fadeOutClip.length;

    }


    public void LoadNextLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex +1);
    }


    public void ReLoadLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadLevel(int index)
    {
        if (animator != null)
        {
            animator.SetTrigger(ExitFlag);
        }


        StartCoroutine(Wait(index, exitTime));

    }


    private IEnumerator Wait(int index, float duration)
    {
        yield return new WaitForSeconds(duration);
        SceneManager.LoadScene(index);
    }
}
