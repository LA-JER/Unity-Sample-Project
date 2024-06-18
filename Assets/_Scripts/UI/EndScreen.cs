using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

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
        totalDamage.text = NumberShorthand.FormatNumber(damage);
        totalSpent.text = spent.ToString();
        totalElapsedTime.text = FormatTime(elapsed);
        screen.SetActive(true);
        
    }

    public static string FormatTime(float timeInSeconds)
    {
        if (timeInSeconds < 0)
        {
            throw new ArgumentOutOfRangeException("Time cannot be negative.");
        }

        // Convert seconds to hours, minutes, and remaining seconds
        int hours = (int)Mathf.Floor(timeInSeconds / 3600f); // Floor ensures whole numbers
        int minutes = (int)Mathf.Floor((timeInSeconds % 3600f) / 60f);
        int seconds = (int)(timeInSeconds % 60f);

        // Format the time string with leading zeros for hours and minutes if necessary
        string formattedTime = $"{hours:00}:{minutes:00}:{seconds:00}";

        return formattedTime;
    }
}
