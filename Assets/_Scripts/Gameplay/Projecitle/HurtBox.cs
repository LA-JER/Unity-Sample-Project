using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{

    [SerializeField] Collider2D hurtbox;
    public Projectile projectile;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (projectile == null) return;
        //Debug.Log("proejctile hurt box entered");
        projectile.OnHurtBoxEntered(collision);
    }

}
