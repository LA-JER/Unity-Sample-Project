using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : Enemy
{

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.CompareTag("Home"))
            {
                Health health = collision.GetComponent<Health>();
                if (health != null)
                {
                    Attack(health);
                    Destroy(gameObject);
                }
            }
            
        }
    }

}
