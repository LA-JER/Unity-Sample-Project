using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPurchaseAble 
{
    int GetPrice();
    int GetTotalSpent();

    void Activate(GameObject owner = null);

    void Deactivate(GameObject owner = null);

    void Destroy();
}
