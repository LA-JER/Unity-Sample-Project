using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderItemCondition : ItemCondition
{
    public CircleCollider2D circle;
    public List<string> tagsAffected = new List<string>();
    public bool affectsSelf = false;

    private StatManager statManager;
    private GameObject owner;

    public List<Transform> targets = new List<Transform>();

    // Start is called before the first frame update
   

    private void OnTriggerStay2D(Collider2D collision)
    {
        if( collision != null && owner != null)
        {
            foreach(string  tag in tagsAffected)
            {
                if( collision.CompareTag(tag) )
                {
                    if ((collision.gameObject == owner && affectsSelf) || collision.gameObject != owner)
                    {
                        //Trigger(new List<GameObject> { collision.gameObject });
                        if (!targets.Contains(collision.transform))
                        {
                            targets.Add(collision.transform);
                        }
                    }

                    
                }
            }
        }
    }

    public void Initialize( GameObject owner, StatManager manager)
    {
        if (manager != null && circle != null)
        {
            this.owner = owner;
            statManager = manager;
            circle.radius = statManager.GetStat(StatManager.Stat.range);
        }
    }

    private void FixedUpdate()
    {
        if (statManager != null && circle != null)
        {
           // circle.radius = statManager.GetStat(StatManager.Stat.range);
        }
    }

    public override void TransferOwnership(GameObject newOwner)
    {
        throw new System.NotImplementedException();
    }
}
