using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    [SerializeField] private List<Drone> drones;

    public event Action OnWaveCompleted;
    public event Action OnWaveStart;

    private List<Robot> aliveRobots = new List<Robot>();

    public void SpawnWave(int amount)
    {
        for (int i = 0; i < amount && i < drones.Count; i++)
        {
            var drone = Instantiate(drones[i]);
            drone.gameObject.SetActive(true);
            aliveRobots.Add(drone.Robot);

            drone.Robot.OnRobotDeath += Robot_OnRobotDeath;
        }

        OnWaveStart?.Invoke();
    }

    private void Robot_OnRobotDeath(Robot obj)
    {
        aliveRobots.Remove(obj);
        if (aliveRobots.Count == 0)
        {
            OnWaveCompleted?.Invoke();
        }
    }
}
