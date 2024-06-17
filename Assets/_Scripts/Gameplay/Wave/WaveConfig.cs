using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tower Game/Wave Config")]
public class WaveConfig : ScriptableObject
{
    public int waveNumber = 0;
    public float duration = 120f;
    public float timeBetweenSpawns = 1f;
    [SerializedDictionary("Enemy Prefab", "Weight")]
    public SerializedDictionary<GameObject, int> weightTable = new SerializedDictionary<GameObject, int>();
}
