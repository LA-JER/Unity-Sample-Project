using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadTimerManager : MonoBehaviour
{
    public static ReloadTimerManager instance;

    [SerializeField] private GameObject timerHolder;
    [SerializeField] private GameObject sliderPrefab;
    public Vector2 offset;

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
        Turret.OnReload += Owner_OnReload;
    }

    private void Owner_OnReload(Vector2 position, float duration)
    {
        if (sliderPrefab == null) return;
        if (duration <= 0) return;

        GameObject sliderInstance = Instantiate(sliderPrefab, timerHolder.transform);
        sliderInstance.transform.position = position + offset;

        TemporaryTimer tiemr = sliderInstance.GetComponent<TemporaryTimer>();
        tiemr.Initialize(duration);
    }

    

    private void OnDestroy()
    {
        Turret.OnReload -= Owner_OnReload;
    }
}
