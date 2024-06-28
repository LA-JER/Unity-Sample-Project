using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDebuffCollider : MonoBehaviour
{
    [SerializeField] private BossEnemy owner;

    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && owner != null)
        {
            if (!GameManager.Instance.GetIsGamePaused())
            {
                string tag = owner.debuffTag;
                if (tag != null)
                {
                    if (collision.CompareTag(tag))
                    {
                        owner.ApplyDebuff(collision);
                       // Debug.Log("Debuff!");
                    }
                } 
            }
        }
    }
    */
}
