using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDrawer : MonoBehaviour
{
    [SerializeField] private GameObject DamageNumberPrefab;
    [SerializeField] private GameObject CriticalDamageNumberPrefab;
    [SerializeField] private Transform spawn;
    [SerializeField] private float randomDistance = .35f;

    // Start is called before the first frame update
    void Start()
    {
        Health.OnDamageDraw += Health_OnDamageDraw;
    }

    private void Health_OnDamageDraw(Vector3 POS, float amount, bool isCritical)
    {
        if (DamageNumberPrefab != null && spawn != null )
        {
            Vector3 numberPosition = POS;
            Vector3 startPOS = numberPosition + new Vector3(Random.Range(-randomDistance, randomDistance), Random.Range(randomDistance, randomDistance), 0f);
            if (!isCritical)
            {
                GameObject damageNumber = Instantiate(DamageNumberPrefab, spawn);
                damageNumber.transform.position = startPOS;
                DamageNumber d = damageNumber.GetComponent<DamageNumber>();
                d.Initialize(amount);


            }
            else
            {
                GameObject damageNumber = Instantiate(CriticalDamageNumberPrefab, spawn);
                damageNumber.transform.position = startPOS;
                DamageNumber d = damageNumber.GetComponent<DamageNumber>();
                d.Initialize(amount);


            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        Health.OnDamageDraw -= Health_OnDamageDraw;
    }
}
