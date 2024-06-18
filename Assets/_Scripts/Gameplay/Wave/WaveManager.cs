using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public delegate void WaveStart(WaveConfig config);
    public static event WaveStart OnConfigStart;
    public delegate void WaveChange();
    public static event WaveChange OnWaveEnd;
    public static event WaveChange OnWaveStart;
    public delegate void RunEnd();
    public static event RunEnd OnGameDone;

    public static WaveManager Instance;

    [SerializeField] private Transform spawn;
    [SerializeField] private WaveConfig[] waveConfigs;

    private GameManager gameManager;
    private int waveNumber = 1;
    private int configIndex = 0;
    private Coroutine spawnCoroutine;
    private float configElapsedTime = 0;
    private bool spawnNextConfig = false;
    private bool outOfWaves = false;
    private bool isPaused = false;

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
        //GameManager.OnPaused += GameManager_OnPaused;
    }

    private void GameManager_OnPaused(bool isPaused)
    {
        this.isPaused = isPaused;
        
    }

    private void ContinueButton_OnContinue()
    {
        Debugger.Log(Debugger.AlertType.Info, $" --- Wave {waveNumber} Begin! ---");
        OnWaveStart?.Invoke();
        spawnNextConfig = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.Instance.GetIsGamePaused() == false)
        {
            SpawnWaves();
            if (outOfWaves)
            {
                if (CheckIfNoEnemies())
                {
                    OnGameDone?.Invoke();
                    gameObject.SetActive(false);
                }
            }
        } else
        {
            if (spawnCoroutine != null)
            {

                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
        }
    }

    void SpawnWaves()
    {

        if (configIndex < waveConfigs.Length)
        {
            
            
            
            //Checks if this config still  belongs to the same wave number
            if (waveConfigs[configIndex].waveNumber != waveNumber)
            {
                spawnNextConfig = false;
                Debugger.Log(Debugger.AlertType.Info, $" --- Wave {waveNumber} Completed! ---");
                if (spawnCoroutine != null)
                {
                    StopCoroutine(spawnCoroutine);
                    spawnCoroutine = null;
                }
                OnWaveEnd?.Invoke();
                waveNumber++;
            }


            if (spawnNextConfig)
            {
                configElapsedTime += Time.deltaTime;
                //Coroutine configRoutine = null;
                if (configElapsedTime <= waveConfigs[configIndex].duration)
                {
                    if (spawnCoroutine == null)
                    {
                        if(waveConfigs[configIndex] is AlertWaveConfig)
                        {
                            AlertWaveConfig config = (AlertWaveConfig)waveConfigs[configIndex];
                            spawnCoroutine = StartCoroutine(SpawnCurrentConfig());
                            configIndex++;
                            AlertManager.instance.ShowAlert(config.title, config.description, config.icon);
                            return;

                        }

                        OnConfigStart?.Invoke(waveConfigs[configIndex]);
                        spawnCoroutine = StartCoroutine(SpawnCurrentConfig());
                    }

                    //yield return new WaitForSeconds(waveConfigs[configIndex].duration);
                }
                else
                {
                    if (spawnCoroutine != null)
                    {
                        StopCoroutine(spawnCoroutine);
                        spawnCoroutine = null;
                    }

                    configIndex++;
                    configElapsedTime = 0;


                }
            }


            
        } else
        {
            //OnGameDone?.Invoke();
            outOfWaves = true;
        }

        

    }

    private bool CheckIfNoEnemies()
    {
        Transform enemyHolder = gameManager.GetEnemyHolder();
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

    private IEnumerator SpawnCurrentConfig()
    {
        if (configIndex < waveConfigs.Length)
        {
            while (true)
            {
                float wait = waveConfigs[configIndex].timeBetweenSpawns;

                GameObject randomEnemyPrefab = RollEnemyType(waveConfigs[configIndex]);
                if (randomEnemyPrefab != null)
                {
                    SpawnPrefab(randomEnemyPrefab);
                }
                else
                {
                    Debugger.Log( Debugger.AlertType.Warning , $"{name} tried to spawn an enemy prefab but recieved a null object");
                }
                yield return new WaitForSeconds(wait);
            }

        }
    }

    private void SpawnPrefab(GameObject prefab)
    {
        if (spawn != null)
        {

            // Spawn the enemy prefab 
            GameObject go = Instantiate(prefab, gameManager.GetEnemyHolder());
            go.transform.position = spawn.position;
        }
    }

    //Returns a random Enemy Prefab based on the rarity assigned to it in given wave config 
    private GameObject RollEnemyType(WaveConfig config)
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
       // GameManager.OnPaused -= GameManager_OnPaused;
    }
}
