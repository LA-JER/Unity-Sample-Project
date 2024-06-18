using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsPage : MonoBehaviour
{
    public static StatsPage Instance;

    [SerializeField] private GameObject screen;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI totalKills;
    [SerializeField] private TextMeshProUGUI totalDamage;
    [SerializeField] private TextMeshProUGUI totalSpent;
    [SerializeField] private TextMeshProUGUI totalElapsedTime;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        if(screen != null)
        {
            screen.SetActive(false);
        }
        else
        {
            Debugger.Log(Debugger.AlertType.Warning, "No screen object set for stats page");
        }
    }

    public void ToggleScreen()
    {
        if(screen != null)
        {
            totalKills.text = DataHandler.GetFlagInt(DataHandler.Flag.totalKills).ToString();
            totalDamage.text = DataHandler.GetFlagInt(DataHandler.Flag.totalDamage).ToString();
            totalSpent.text = DataHandler.GetFlagInt(DataHandler.Flag.totalSpent).ToString();
            totalElapsedTime.text = DataHandler.GetFlagInt(DataHandler.Flag.totalTime).ToString();

            screen.SetActive(!screen.activeSelf);
        }
    }

}
