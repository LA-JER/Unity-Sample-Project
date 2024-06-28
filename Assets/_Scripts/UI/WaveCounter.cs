using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class WaveCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string pretext = "Wave : ";

    private void Awake()
    {
        WaveManager.OnWaveStart += WaveManager_OnWaveStart;
    }

    private void WaveManager_OnWaveStart(WaveGroup wave, bool h)
    {
        text.text = pretext + wave.waveNumber;
    }


    private void OnDestroy()
    {
        WaveManager.OnWaveStart -= WaveManager_OnWaveStart;
    }
}
