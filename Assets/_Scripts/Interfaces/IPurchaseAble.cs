using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPurchaseAble 
{
    int GetPrice();

    void Activate(GameObject owner = null);
}
