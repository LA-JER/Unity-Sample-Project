using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class AlertManager : MonoBehaviour
{
    public delegate void Alert(bool isShown);
    public static event Alert OnAlert;

    public static AlertManager instance;

    [SerializeField] private GameObject alertScreen;
    [SerializeField] private List<Image> alertImages = new List<Image>();
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Image icon;
    [SerializeField] private AnimationClip fadeOutClip;

    private Animator animator;
    private float exitTime = 1f;

    private bool clickable = true;
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

    public void ShowAlert(string title, string description, Sprite sprite, Color color)
    {
        if (alertScreen == null) return;
        clickable = true;
        this.title.text = title;
        this.description.text = description;
        this.icon.sprite = sprite;
        SetBGColor(color);
        Debugger.Log(Debugger.AlertType.Info, "Showing Alert!");
        OnAlert?.Invoke(true);
        alertScreen.SetActive(true);
    }

    public void HideAlert()
    {
        if(clickable)
        {
            clickable = false;
            if (alertScreen == null) return;
            Debugger.Log(Debugger.AlertType.Info, "Hiding Alert!");
            OnAlert?.Invoke(false);
            animator.SetTrigger("Exit");
            StartCoroutine(DelayDisable(exitTime));
        }
        
    }

    private IEnumerator DelayDisable( float duration)
    {
        yield return new WaitForSeconds(duration);
        alertScreen.SetActive(false);
        //Time.timeScale = 1;
        yield return null;
    }

    void SetBGColor( Color color)
    {
        foreach(Image image in alertImages)
        {
            if (image == null)
            {
                continue;
            }
            image.color = color;
        }
    }
}
