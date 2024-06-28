using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombineManager : MonoBehaviour
{
    public static CombineManager instance;

    public delegate void ConsumerReady(GameObject consumer, GameObject eaten);
    public static event ConsumerReady OnPossibleConsume;
    public static event ConsumerReady OnNoConsume;
    public static event ConsumerReady OnConsume;


    public GameObject consumer;
    public GameObject toBeConsumed;
    private bool dragging = false;

    private void Awake()
    {
        InputHandler.OnMouseDragEvent += PrepareToConsume;
        InputHandler.OnMouseDragStopEvent += InputHandler_OnMouseDragStopEvent;
        InputHandler.OnMouseEnterEvent += InputHandler_OnMouseEnterEvent;
        InputHandler.OnMouseExitEvent += InputHandler_OnMouseExitEvent;
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void InputHandler_OnMouseExitEvent(GameObject owner)
    {
        if (!dragging) return;
        if(owner == toBeConsumed) return;
        OnNoConsume?.Invoke(consumer, toBeConsumed);
        consumer = null;
    }

    private void InputHandler_OnMouseEnterEvent(GameObject owner)
    {
        if (!dragging) return;
        if (owner == toBeConsumed) return;
        consumer = owner;

        OnPossibleConsume?.Invoke(consumer, toBeConsumed);

    }

    private void InputHandler_OnMouseDragStopEvent()
    {
        dragging = false;
        Consume(consumer, toBeConsumed);
        toBeConsumed = null;
        consumer = null;
    }

    private void Consume(GameObject consumer,  GameObject eaten)
    {
        if (consumer == null || eaten == null || consumer == eaten) return;
        TransferItems(consumer, eaten);
        TransferProjectileModules(consumer, eaten);

        OnConsume?.Invoke(consumer, eaten);
        Destroy(eaten);

    }

    //items are instanced per turret, so need to transfer ownership
    private void TransferItems(GameObject consumer, GameObject eaten)
    {
        ItemManager consumerItemMan = consumer.GetComponent<ItemManager>();
        ItemManager eatenItemMan = eaten.GetComponent<ItemManager>();

        //list of special items owned by the object about to be consumed
        List<GameObject> eatenSpecials = eatenItemMan?.GetSpecialItems();

        //transfer all items to the new owner (eater)
        foreach (GameObject item in eatenSpecials)
        {
            consumerItemMan?.TransferItem(item, true);
        }
    }

    //projectile modules are instanced per projectile, so the consumer only needs a reference to the prefab
    private void TransferProjectileModules(GameObject consumer, GameObject eaten)
    {
        DamageElementManager consumerManager = consumer.GetComponent<DamageElementManager>();
        DamageElementManager eatenManager = eaten.GetComponent<DamageElementManager>();

        if (eatenManager == null || consumerManager == null) return;
        List<GameObject> hitEffects = eatenManager.GetEffectPrefabs();
        GameObject projectileMove = eatenManager.GetProjectileMovement();
        hitEffects.Add(projectileMove);

        consumerManager.AddNewModules(hitEffects);

        

    }

    private void PrepareToConsume(GameObject owner)
    {
        if(owner == null) return;

        toBeConsumed = owner;
        dragging = true;
    }



    private void OnDestroy()
    {
        InputHandler.OnMouseDragEvent -= PrepareToConsume;
        InputHandler.OnMouseDragStopEvent -= InputHandler_OnMouseDragStopEvent;
        InputHandler.OnMouseEnterEvent -= InputHandler_OnMouseEnterEvent;
        InputHandler.OnMouseExitEvent -= InputHandler_OnMouseExitEvent;
    }
}
