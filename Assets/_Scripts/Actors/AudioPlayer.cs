using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioManager;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource SFXSource;
    [SerializeField] SoundEfect shootSFX;
    [SerializeField] SoundEfect hurtSFX;
    [SerializeField] SoundEfect dieSFX;

    private Turret turret;
    private Health health;
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
    }

    private void Health_onHealthZero(GameObject killer)
    {
        PlaySound(dieSFX);
    }

    private void Health_onHealthDamage(float amount, bool isCritical)
    {
        PlaySound(hurtSFX);
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
        SFXSource.clip = sfx.audioClip;
        SFXSource.Play();
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
    }
}
