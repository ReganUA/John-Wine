using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner instance;

    [SerializeField] private UnityEvent[] onInteract;
    private int OnInteractIndex = 0;

    [SerializeField] private float[] waveDelay;
    private int waveDelayIndex = 0;
    [SerializeField] private int[] waveSize;
    private int waveSizeIndex = 0;

    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private GameObject[] spawnPoints;

    [HideInInspector] public List<GameObject> wave = new();

    private bool infiniteSpawn = false;
    private void Start()
    {
        instance = this;
    }
    public void StartWave()
    {
        StartCoroutine(SpawnWave(waveDelay[waveDelayIndex], waveSize[waveSizeIndex]));

        if (waveDelayIndex < waveDelay.Length - 1)
        {
            waveDelayIndex++; waveSizeIndex++;
        }
    }
    private IEnumerator SpawnWave(float delay, int waveSize)
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < waveSize; i++)
        {
            int randomEnemy = Random.Range(0, enemyPrefabs.Length);
            int randomSpawn = Random.Range(0, spawnPoints.Length);

            GameObject newEnemy = Instantiate(enemyPrefabs[randomEnemy], spawnPoints[randomSpawn].transform.position, Quaternion.identity);
            wave.Add(newEnemy);
        }
    }
    public void EnemyDied(GameObject enemy)
    {
        wave.Remove(enemy);

        if (wave.Count == 0)
        {
            onInteract[OnInteractIndex].Invoke();

            if (infiniteSpawn == true)
            {
                StartWave();
                return;
            }
            else
            {
                if (OnInteractIndex < onInteract.Length - 1)
                {
                    OnInteractIndex++;
                }
                else
                {
                    return;
                }
            }      
        }
    }
    public void EnableInfiniteWaves()
    {
        infiniteSpawn = true;
    }
    public void DisableInfiniteWaves()
    {
        infiniteSpawn = false;
    }
}
