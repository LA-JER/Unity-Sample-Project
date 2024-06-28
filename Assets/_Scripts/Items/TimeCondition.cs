using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCondition : ItemCondition
{
    public float cooldown;
    public bool repeats = true;

    private void Start()
    {
        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        while (true)
        {
            yield return new WaitForSeconds(cooldown);
            Trigger();
            if (!repeats) yield break;
        }
        
    }

    public override void TransferOwnership(GameObject newOwner)
    {
        StopAllCoroutines();
        StartCoroutine(Cooldown());
    }
}
