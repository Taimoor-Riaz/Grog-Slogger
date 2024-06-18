using System;
using System.Collections.Generic;
using UnityEngine;

public class CostumersManager : MonoBehaviour
{
    [SerializeField] private int maxCostumers;
    [SerializeField] private float custumerRate;
    [SerializeField] private Costumer costumerPrefab;
    [SerializeField] private ChairsManager tablesManager;
    [SerializeField] private EnemiesManager enemiesManager;

    [SerializeField] private Transform spawnPoint;

    public bool IsPaused { get; private set; }

    private float timeBetweenCostumersSpawn;
    private float lastCostumerSpawnTime;

    private List<Costumer> spawnedCostumers;
    private List<Costumer> costumersInWave;

    private bool isWaveActive;

    void OnEnable()
    {
        enemiesManager.OnWaveStart += OnWaveStart;
        enemiesManager.OnWaveCompleted += OnWaveCompleted;
    }

    private void OnWaveCompleted()
    {
        isWaveActive = false;
        foreach (var costumer in costumersInWave)
        {
            costumer.StateMachine.ChangeState(typeof(CostumerGoingToTableState));
        }
    }

    private void OnWaveStart()
    {
        isWaveActive = true;
        costumersInWave = new List<Costumer>();
        for (int i = 0; i < spawnedCostumers.Count; i++)
        {
            StateMachine<Costumer> machine = spawnedCostumers[i].StateMachine;
            Type key = machine.CurrentState.GetType();
            if (key != typeof(CostumerGoingBackState))
            {
                costumersInWave.Add(spawnedCostumers[i]);
                spawnedCostumers[i].StateMachine.ChangeState(typeof(CostumerDuringWaveState));
            }
        }
    }

    void OnDisable()
    {
        enemiesManager.OnWaveStart -= OnWaveStart;
        enemiesManager.OnWaveCompleted -= OnWaveCompleted;
    }

    private void Start()
    {
        timeBetweenCostumersSpawn = 1 / custumerRate;
        spawnedCostumers = new List<Costumer>();
        lastCostumerSpawnTime = Time.time;
    }
    void Update()
    {
        if ((spawnedCostumers.Count >= maxCostumers) || isWaveActive || IsPaused)
        {
            return;
        }
        if (Time.time - lastCostumerSpawnTime >= timeBetweenCostumersSpawn)
        {
            lastCostumerSpawnTime = Time.time;
            SpawnCostumer();
        }
    }
    public Costumer SpawnCostumer()
    {
        if ((spawnedCostumers.Count >= maxCostumers))
        {
            Debug.LogError("This souldn't be called. There's no Space for more Costumers");
        }
        Chair table = tablesManager.GetRandomTable();
        if (table == null)
        {
            Debug.Log("No tables available");
            return null;
        }

        Costumer costumer = Instantiate(costumerPrefab, spawnPoint.position, Quaternion.identity);
        costumer.Initialize(table, spawnPoint.position);
        spawnedCostumers.Add(costumer);

        costumer.OnCostumerReturned += () => spawnedCostumers.Remove(costumer);
        return costumer;
    }
    public void Pause()
    {
        IsPaused = true;
    }
    public void Resume()
    {
        IsPaused = false;
    }
}