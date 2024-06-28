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
    [SerializeField] private SoundEfect gameWinSFX;
    [SerializeField] private SoundEfect fastForwardSFX;


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
        //Enemy.OnEnemyDeath += Enemy_OnEnemyDeath;
        GameManager.OnGameEnd += Base_OnBaseDie;
        FastForwardButton.OnFastForward += FastForwardButton_OnFastForward;
        FastForwardButton.OnNormal += FastForwardButton_OnNormal;
    }

    private void FastForwardButton_OnNormal()
    {
        StopSound(fastForwardSFX);
    }

    private void FastForwardButton_OnFastForward()
    {
        PlaySound(fastForwardSFX, true);
    }

    private void Base_OnBaseDie(bool win)
    {
        if (win)
        {
            PlaySound(gameWinSFX);
        } else
        {
            PlaySound(gameOverSFX);
        }
        musicSource.Stop();
        
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
    private void WaveManager_OnWaveStart(WaveGroup wave, bool h)
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


    void PlaySound(SoundEfect soundEfect, bool loops = false)
    {
        float pitch = 1;
        if (soundEfect.randomPitch)
        {
            pitch = UnityEngine.Random.Range(soundEfect.minPitch, soundEfect.maxPitch);
        }

        AudioSource audioSource = null;
        switch (soundEfect.soundType)
        {
            case SoundType.GamePlay:
                audioSource = gameSFXSource;
                break;
            case SoundType.UI:
                audioSource = uiSFXSource;
                break;
            case SoundType.Music:
                audioSource = musicSource;
                break;
            default: break;
        }

        if (audioSource == null) return;
        audioSource.pitch = pitch;
        audioSource.loop = loops;
        if (loops)
        {
            audioSource.clip = soundEfect.audioClip;
            audioSource.Play();
        } else
        {
            //Debug.Log("test");
            audioSource.PlayOneShot(soundEfect.audioClip);
        }

    }

    void StopSound(SoundEfect soundEfect)
    {
        
        switch (soundEfect.soundType)
        {
            case SoundType.GamePlay:
                gameSFXSource.Stop();
                gameSFXSource.loop = false;
                break;
            case SoundType.UI:
                uiSFXSource.Stop();
                uiSFXSource.loop = false;
                break;
            case SoundType.Music:
                musicSource.Stop();
                musicSource.loop = false;
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
        //Enemy.OnEnemyDeath -= Enemy_OnEnemyDeath;
        GameManager.OnGameEnd -= Base_OnBaseDie;
        FastForwardButton.OnFastForward -= FastForwardButton_OnFastForward;
        FastForwardButton.OnNormal -= FastForwardButton_OnNormal;
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
