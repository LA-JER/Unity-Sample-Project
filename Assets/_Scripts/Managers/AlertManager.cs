using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class AlertManager : MonoBehaviour
{
    public delegate void Alert(bool isShown);
    public static event Alert OnAlert;

    public static AlertManager instance;

    [SerializeField] private GameObject alertScreen;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Image icon;
    [SerializeField] private AnimationClip fadeOutClip;

    private Animator animator;
    private float exitTime = 1f;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = alertScreen.GetComponent<Animator>();
        if (fadeOutClip)
            exitTime = fadeOutClip.length;
        alertScreen.SetActive(false);
    }

    public void ShowAlert(string title, string description, Sprite sprite)
    {
        if (alertScreen == null) return;

        this.title.text = title;
        this.description.text = description;
        this.icon.sprite = sprite;
        Debugger.Log(Debugger.AlertType.Info, "Showing Alert!");
        OnAlert?.Invoke(true);
        alertScreen.SetActive(true);
    }

    public void HideAlert()
    {
        if (alertScreen == null) return;
        Debugger.Log(Debugger.AlertType.Info, "Hiding Alert!");
        OnAlert?.Invoke(false);
        animator.SetTrigger("Exit");
        StartCoroutine(DelayDisable(exitTime));
    }

    private IEnumerator DelayDisable( float duration)
    {
        yield return new WaitForSeconds(duration);
        alertScreen.SetActive(false);
        //Time.timeScale = 1;
        yield return null;
    }
}
