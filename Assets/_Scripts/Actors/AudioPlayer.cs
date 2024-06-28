using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioManager;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource SFXSource;
    [SerializeField] SoundEfect shootSFX;
    [SerializeField] SoundEfect criticalHurt;
    [SerializeField] SoundEfect hurtSFX;
    [SerializeField] SoundEfect dieSFX;
    [SerializedDictionary("Buff ID", "SFX")]
    [SerializeField] SerializedDictionary<int, SoundEfect> buffSFX = new SerializedDictionary<int, SoundEfect>();   

    private Turret turret;
    private Health health;
    private BuffManager buffManager;
    // Start is called before the first frame update
    void Start()
    {
        turret = GetComponent<Turret>();
        if(turret != null)
        {
            turret.OnTurretShoot += Turret_OnTurretShoot;
        }
        health = GetComponent<Health>();
        if(health != null)
        {
            health.onHealthDamage += Health_onHealthDamage;
            health.onHealthZero += Health_onHealthZero;
        }
        buffManager = GetComponent<BuffManager>();
        if(buffManager != null )
        {
            buffManager.OnBuffChange += BuffManager_OnBuffApply;
        }
    }

    private void BuffManager_OnBuffApply(int buffID, bool isActive)
    {
        if(isActive)
        {
            if (buffSFX.ContainsKey(buffID))
            {
                PlaySound(buffSFX[buffID]);
            }
            
        }
    }

    private void Health_onHealthZero(GameObject killer)
    {
        PlaySound(dieSFX);
    }

    private void Health_onHealthDamage(float amount, bool isCritical)
    {
        if (isCritical)
        {
            PlaySound(criticalHurt);
        } else
        {
            PlaySound(hurtSFX);
        }
        
    }

    private void Turret_OnTurretShoot()
    {
        PlaySound(shootSFX);
    }

    private void PlaySound(SoundEfect sfx)
    {
        float pitch = 1;
        if (sfx.randomPitch)
        {
            pitch = Random.Range(sfx.minPitch, sfx.maxPitch);
        }
        SFXSource.pitch = pitch;
        SFXSource.PlayOneShot(sfx.audioClip);
    }

    private void OnDestroy()
    {
        if (turret != null)
        {
            turret.OnTurretShoot -= Turret_OnTurretShoot;
        }
        if (health != null)
        {
            health.onHealthDamage -= Health_onHealthDamage;
            health.onHealthZero -= Health_onHealthZero;
        }
        if (buffManager != null)
        {
            buffManager.OnBuffChange -= BuffManager_OnBuffApply;
        }
    }
}
