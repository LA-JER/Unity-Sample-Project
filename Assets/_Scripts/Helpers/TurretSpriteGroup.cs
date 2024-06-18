using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TurretSpriteGroup
{
    public enum TurretSpriteType
    {
        Bottom,
        Middle,
        Top,
    }


    [SerializedDictionary("Sprite Type", "Sprite")][SerializeField]
    public SerializedDictionary<TurretSpriteType, Sprite> turretSprites = new SerializedDictionary<TurretSpriteType, Sprite> ();
}
