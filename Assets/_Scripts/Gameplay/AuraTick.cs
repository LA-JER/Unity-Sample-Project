using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AuraTick : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public CircleCollider2D circleCollider;

    private BuffAreaOfEffect owner;
    private Buff buff;
    private string targetTag;
    public List<GameObject> hitObjects = new List<GameObject>();
    private float internalCooldown = 3;
    

    public void Initialize(BuffAreaOfEffect owner, Buff buff, Color color, float range, float lifespan, string tag)
    {
        this.owner = owner;
        this.buff = buff;
        spriteRenderer.color = color;
        circleCollider.radius = range;
        spriteRenderer.transform.localScale *= range;
        this.targetTag = tag;
        Destroy(gameObject, lifespan);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("test");
        //null checks

        if (collision == null) return;
        if (targetTag == null) return;

        //check this is the desired target
        if (!collision.CompareTag(targetTag)) return;



        //avoid hitting the same target while on cooldown
        if (hitObjects.Contains(collision.gameObject)) return;

        //special case for turret colliders
        if (targetTag == "TurretSpace")
        {
            //the collider which trigger this is a child of the turret
            ApplyDebuff(collision.transform.parent.gameObject);
        }
        else
        {
            ApplyDebuff(collision.gameObject);
        }


        StartCoroutine(CoolDown(collision.gameObject));
    }


    public void ApplyDebuff(GameObject collision)
    {
        if (collision == null) return;


        BuffManager buffManager = collision.GetComponent<BuffManager>();
        if (buffManager != null)
        {
            //Debugger.Log(Debugger.AlertType.Info, $"Boss is debuffing {collision.gameObject}");
            buffManager.ApplyTimedBuff(buff);
        }

    }

    
    private IEnumerator CoolDown(GameObject hit)
    {
        if (hit == null) yield break;

        hitObjects.Add(hit);
        yield return new WaitForSeconds(internalCooldown);
        hitObjects.Remove(hit);

    }
    
}
