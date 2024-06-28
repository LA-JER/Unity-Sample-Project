using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable 
{
    public enum PlayArea
    {
        Ground,
        Air,
        Sea,
    }
    bool IsTargetable();

    PlayArea GetPlayableArea();
}
