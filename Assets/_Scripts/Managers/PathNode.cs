using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PathNode : MonoBehaviour
{
    [SerializedDictionary]
    [SerializeField] public SerializedDictionary<PathNode, int> childNodes = new SerializedDictionary<PathNode, int>(); 
    public Transform location;

}
