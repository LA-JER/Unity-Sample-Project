using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public enum SoundType
    {
        GamePlay,
        UI,
        Music,
    }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource uiSFXSource;
    [SerializeField] private AudioSource gameSFXSource;
    [SerializeField] private SoundEfect buySuccess;
    [SerializeField] private SoundEfect buyFail;
    [SerializeField] private SoundEfect refundSFX;
    [SerializeField] private SoundEfect placeFail;
    [SerializeField] private SoundEfect placeSucess;
    [SerializeField] private SoundEfect waveStart;
    [SerializeField] private SoundEfect BGM;
    [SerializeField] private SoundEfect enemyDeath;
    [SerializeField] private SoundEfect gameOverSFX;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        UpgradeDisplay.OnTryBuy += UpgradeDisplay_OnTryBuy;
        Shop.OnTryBuy += Shop_OnTryBuy;
        Shop.OnTryPlace += Shop_OnTryPlace;
        Shop.OnRefund += Shop_OnRefund;
        WaveManager.OnWaveStart += WaveManager_OnWaveStart;
        PauseMenu.onPauseToggle += PauseMenu_onPauseToggle;
        Enemy.OnEnemyDeath += Enemy_OnEnemyDeath;
        HomeBase.OnBaseDie += Base_OnBaseDie;
    }

    private void Base_OnBaseDie()
    {
        PlaySound(gameOverSFX);
    }

    private void Enemy_OnEnemyDeath(GameObject souce, Enemy.EnemyRank rank, int value)
    {
        PlaySound(enemyDeath);
    }

    private void PauseMenu_onPauseToggle(bool isPaused)
    {
        if (isPaused)
        {
            musicSource.Pause();
        }
        else
        {
            musicSource.Play();
        } 
        
    }

    private void Shop_OnRefund()
    {
        PlaySound(refundSFX);
    }

    private void Start()
    {
        PlaySound(BGM);
    }
    private void WaveManager_OnWaveStart()
    {
        PlaySound(waveStart);
    }

    private void UpgradeDisplay_OnTryBuy(bool successful)
    {
        if (successful)
        {
            PlaySound(buySuccess);
        }
        else
        {
            PlaySound(buyFail);
        }
    }

    private void Shop_OnTryPlace(bool successful)
    {
        if (successful)
        {
            PlaySound(placeSucess);
        } else
        {
            PlaySound(placeFail);
        }
    }

    private void Shop_OnTryBuy(bool successful)
    {
        if (successful)
        {
            PlaySound(buySuccess);
        }
        else
        {
            PlaySound(buyFail);
        }
    }


    void PlaySound(SoundEfect soundEfect)
    {
        float pitch = 1;
        if (soundEfect.randomPitch)
        {
            pitch = UnityEngine.Random.Range(soundEfect.minPitch, soundEfect.maxPitch);
        }

        switch (soundEfect.soundType)
        {
            case SoundType.GamePlay:
                gameSFXSource.pitch = pitch;
                gameSFXSource.clip = soundEfect.audioClip;
                gameSFXSource.Play();
                break;
            case SoundType.UI:
                uiSFXSource.pitch = pitch; 
                uiSFXSource.clip = soundEfect.audioClip; 
                uiSFXSource.Play(); 
                break;
            case SoundType.Music:
                musicSource.pitch = pitch;
                musicSource.clip = soundEfect.audioClip;
                musicSource.Play();
                break;
            default: break;
        }
    }



    private void OnDestroy()
    {
        UpgradeDisplay.OnTryBuy -= UpgradeDisplay_OnTryBuy;
        Shop.OnTryBuy -= Shop_OnTryBuy;
        Shop.OnTryPlace -= Shop_OnTryPlace;
        Shop.OnRefund -= Shop_OnRefund;
        WaveManager.OnWaveStart -= WaveManager_OnWaveStart;
        PauseMenu.onPauseToggle -= PauseMenu_onPauseToggle;
        Enemy.OnEnemyDeath -= Enemy_OnEnemyDeath;
        HomeBase.OnBaseDie -= Base_OnBaseDie;
    }

    [Serializable]
    public class SoundEfect
    {
        
        public SoundType soundType;
        public AudioClip audioClip;
        public bool randomPitch = false;
        public float minPitch = .95f;
        public float maxPitch = 1.07f;
    }
}
