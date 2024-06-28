using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> specialItems = new List<GameObject>();
    [SerializeField] private List<GameObject> items = new List<GameObject>();

    private GameObject owner;
    // Start is called before the first frame update
    void Start()
    {

        InitializeSpecials();
        owner = gameObject;
    }


    public void AddItemsFromPrefabs(List<GameObject> prefabs)
    {
        foreach(GameObject prefab in prefabs)
        {
            GameObject iteminstance = Instantiate(prefab, transform); 
            Item item = iteminstance.GetComponent<Item>();
            if(item != null)
            {
                items.Add(item.gameObject);
                //item.OnEquipmentReady += Item_OnEquipmentReady;
                item.owner = owner;
            }
        }
        
    }

    public void TransferItem(GameObject itemObject, bool isSpecial)
    {
        if (itemObject == null) return;
        if (items.Contains(itemObject))
        {
            Debugger.Log(Debugger.AlertType.Warning, $"{owner.name}'s item manager has {itemObject.name} already. are you sure this intended?");
        } else
        {
            itemObject.transform.parent = transform;
            items.Add(itemObject);

            if( isSpecial )
            {
                specialItems.Add(itemObject);
            }

            Item item = itemObject.GetComponent<Item>();
            item.TranferOwnership(owner);
        }
        

        

    }

    public List<GameObject> GetSpecialItems()
    {
        List<GameObject> list = new List<GameObject>();
        foreach (GameObject item in specialItems)
        {
            if (item != null)
            {
                list.Add(item.gameObject);
            }
        }

        return list;
    }

    public void RemoveItem(Item item)
    {
        if( items.Contains(item.gameObject) )
        {
            items.Remove(item.gameObject);
            //item.OnEquipmentReady -= Item_OnEquipmentReady;
        }
    }

    private void InitializeSpecials()
    {
        if (specialItems != null && specialItems.Count >0)
        {
            if (!items.Contains(specialItems[0]))
            {
                items.Add(specialItems[0]);
            }
            
        }
    }
}
