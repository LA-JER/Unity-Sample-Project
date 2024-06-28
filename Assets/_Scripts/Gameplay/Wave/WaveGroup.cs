using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Tower Game/Wave Group")]
public class WaveGroup : ScriptableObject
{
    public int waveNumber = 1;
    public WaveConfig[] configs;
    public bool continues = false;
}
