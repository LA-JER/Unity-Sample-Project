using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffAreaOfEffect : MonoBehaviour
{
    [SerializeField] private GameObject auraPrefab;
    [SerializeField] private string targetTag;
    [SerializeField] private Color color;
    [SerializeField] private float cooldown;
    [SerializeField] private float range;
    [SerializeField] private float lifespan;
    [SerializeField] private Buff buff;
    


    private void Start()
    {
        StartCoroutine(Aura());
    }



   private IEnumerator Aura()
    {
        while(true)
        {
            GameObject obj = Instantiate(auraPrefab, transform);
            AuraTick auraInstance = obj.GetComponent<AuraTick>();
            if(auraInstance != null)
            {
                auraInstance.Initialize(this, buff, color, range, lifespan, targetTag);
            }

            yield return new WaitForSeconds(cooldown);
        }
    }


   
}
