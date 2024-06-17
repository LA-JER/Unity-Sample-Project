using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndScreen : MonoBehaviour
{
    public static EndScreen instance;

    [SerializeField] private GameObject screen;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI totalKills;
    [SerializeField] private TextMeshProUGUI totalDamage;
    [SerializeField] private TextMeshProUGUI totalSpent;
    [SerializeField] private TextMeshProUGUI totalElapsedTime;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(screen != null)
        {
            screen.SetActive(false);
        } else
        {
            Debugger.Log(Debugger.AlertType.Warning, "No screen object set!");
        }
    }

    public void ShowScreen(string message, int kills, float damage, int spent, float elapsed)
    {
        title.text = message;
        totalKills.text = kills.ToString();
        totalDamage.text = damage.ToString();
        totalSpent.text = spent.ToString();
        totalElapsedTime.text = elapsed.ToString();
        screen.SetActive(true);
        
    }
}
