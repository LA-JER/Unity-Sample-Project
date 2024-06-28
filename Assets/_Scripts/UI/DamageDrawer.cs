using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DamageDrawer : MonoBehaviour
{
    [SerializeField] private GameObject DamageNumberPrefab;
    [SerializeField] private GameObject CriticalDamageNumberPrefab;
    [SerializeField] private Transform spawn;
    [SerializeField] private float randomDistance = .35f;
   // [SerializeField] private float offsetStep = 0.25f;

   // private float offset = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        Health.OnDamageDraw += Health_OnDamageDraw;
    }

    private void Health_OnDamageDraw(Vector3 position, float amount, bool isCritical)
    {
        if (DamageNumberPrefab == null || spawn == null) return;
        
        //check player prefs if we should show damage numbers
        if (DataHandler.GetFlagInt(DataHandler.Flag.showDamageNumbers) != 1) return;

        //StartCoroutine(OffsetCooldown());
        Vector3 startPosition = position + new Vector3(
                Random.Range(-randomDistance, randomDistance) ,
                Random.Range(-randomDistance, randomDistance) ,
                0
            );

        GameObject prefab = isCritical ? CriticalDamageNumberPrefab : DamageNumberPrefab;
        GameObject damageNumber = Instantiate(prefab, spawn);
        damageNumber.transform.position = startPosition;

        DamageNumber damageComponent = damageNumber.GetComponent<DamageNumber>();
        damageComponent.Initialize(amount);

    }


    private void OnDestroy()
    {
        Health.OnDamageDraw -= Health_OnDamageDraw;
    }
}
