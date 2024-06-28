using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public delegate void WaveChange(WaveGroup wave, bool isLast);
    public static event WaveChange OnWaveStart;
    public static event WaveChange OnWaveEnd;

    public delegate void WaveEvent();
    public static event WaveEvent OnAllEnemiesKilled;
    public static event WaveEvent OnWaveSkip;

    public static WaveManager Instance;

    [SerializeField] private Transform spawn;
    [SerializeField] private WaveGroup[] waveGroups;

    public int waveIndex = 0;
    public int configIndex = 0;
    public float configElapsedTime = 0;
    //private Coroutine waveRoutine = null;
    //private Coroutine configRoutine = null;

    public int waveStatus = 0; //0 = game not started yet or paused, 1 = spawning, 2 = stopped
    private Dictionary<AlertWaveConfig, bool> alerted = new Dictionary<AlertWaveConfig, bool>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        ContinueButton.OnContinue += ContinueButton_OnContinue;
        GameManager.OnPaused += GameManager_OnPaused;
    }

    private void GameManager_OnPaused(bool isPaused)
    {
        if(isPaused)
        {
            waveStatus = 0;
            StopAllCoroutines();
        } else
        {
            waveStatus = 1;
            //waveRoutine = 
            StartCoroutine(SpawnNextWave());
        }
    }

    private void ContinueButton_OnContinue()
    {
        //waveRoutine =
        StartCoroutine(SpawnNextWave());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(waveStatus == 1)
        {
            configElapsedTime += Time.fixedDeltaTime;
        } 

        //if this is the last wave
        //waves always increment after theyre dones
         if(waveIndex == waveGroups.Length && waveStatus == 2)
        {
            if (IsWaveCompleted())
            {
                Debugger.Log(Debugger.AlertType.Info, "All enemies dead!");
                OnAllEnemiesKilled?.Invoke();
                this.enabled = false;
            }
        }
      
    }


    IEnumerator SpawnNextWave()
    {
        if(waveGroups == null)
        {
            Debugger.Log(Debugger.AlertType.Warning, "Wave Manager had no waves to spawn!");
            yield break;
        }

        waveStatus = 1;
        
        WaveGroup currentWave = waveGroups[waveIndex];
        if(currentWave == null)
        {
            Debugger.Log(Debugger.AlertType.Warning, $"Wave {waveIndex} was null!");
            yield break;
        }
        Debugger.Log(Debugger.AlertType.Info, $" --- Wave {currentWave.waveNumber} Begin! ---");
        bool isLast = waveIndex == waveGroups.Length - 1;
            OnWaveStart?.Invoke(currentWave, isLast);

        //handles spawning each config
        for (int i = configIndex; i < currentWave.configs.Length; i++)
        {
            
            WaveConfig config = currentWave.configs[i];
            if (config == null) continue;

            

            //configRoutine =
            StartCoroutine(SpawnConfig(config, config.duration - configElapsedTime ));
            yield return new WaitForSeconds(config.duration);
            configElapsedTime = 0;
            configIndex++;
        }

         isLast = waveIndex == waveGroups.Length - 1;
        OnWaveEnd?.Invoke(currentWave, isLast);
        configIndex = 0;
        waveIndex++;
        waveStatus = 2;

        if (currentWave.continues)
        {
            OnWaveSkip?.Invoke();
            //waveRoutine =
            StartCoroutine(SpawnNextWave());
        }

        yield break;
    }



    private bool IsWaveCompleted()
    {
        Transform enemyHolder = GameManager.Instance.GetEnemyHolder();
        if(enemyHolder != null)
        {
            int enemies = enemyHolder.transform.childCount;
            if(enemies == 0)
            {
                
                return true;
            }
        }
        return false;
    }

    private IEnumerator SpawnConfig(WaveConfig config, float duration)
    {
        if(config is AlertWaveConfig)
        {
            HandleAlert(config as  AlertWaveConfig);
        }

        float pause = config.timeBetweenSpawns;
        int spawnCount = Mathf.CeilToInt(duration / pause);

        for(int i = 0; i < spawnCount; i++)
        {
            GameObject randomEnemyPrefab = RollEnemyFromConfig(config);
            if (randomEnemyPrefab == null)
            {
                Debugger.Log(Debugger.AlertType.Warning, $"{name} tried to spawn an enemy prefab but recieved a null object");
                continue;
            }

            SpawnPrefab(randomEnemyPrefab);
            yield return new WaitForSeconds(pause);
        }
        yield break;
    }

    private void HandleAlert(AlertWaveConfig config)
    {
        if (alerted.ContainsKey(config)) return;
        alerted.Add(config, true );
        AlertManager.instance.ShowAlert(config.title, config.description, config.icon, config.backgroundColor);
    }

    private void SpawnPrefab(GameObject prefab)
    {
        if (spawn != null)
        {

            // Spawn the enemy prefab 
            GameObject go = Instantiate(prefab, GameManager.Instance.GetEnemyHolder());
            //Transform randomStart = EnemyPathManager.Instance.GetRandomSpawn();
            //go.transform.position = randomStart.position;
            go.transform.position = spawn.position;
        }
    }

    //Returns a random Enemy Prefab based on the rarity assigned to it in given wave config 
    private GameObject RollEnemyFromConfig(WaveConfig config)
    {
        List<GameObject> randomPrefab = RandomSelect<GameObject>.ChooseRandomObjects(config.weightTable, 1);
        if(randomPrefab == null || randomPrefab.Count ==0)
        {
            return null;
        }
        return randomPrefab[0];

    }

    private void OnDestroy()
    {
        ContinueButton.OnContinue -= ContinueButton_OnContinue;
        GameManager.OnPaused -= GameManager_OnPaused;
    }
}
