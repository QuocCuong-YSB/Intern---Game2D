using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Prefabs")]
    [SerializeField] private GameObject[] groundObstacles;
    [SerializeField] private GameObject[] airObstacles;
    [SerializeField] private GameObject[] destructibleObstacles;

    [Header("Spawn Settings")]
    [SerializeField] private float minSpawnInterval = 1f;
    [SerializeField] private float maxSpawnInterval = 2.5f;
    [SerializeField] private float spawnXPosition = 10f;
    [SerializeField] private float groundYPosition = -1.5f;
    [SerializeField] private float airYPosition = 1.5f;

    [Header("Difficulty")]
    [SerializeField] private float minIntervalDecrease = 0.8f;
    [SerializeField] private float obstacleSpeed = 5f;

    private float timer;
    private float currentMinInterval;
    private float currentMaxInterval;

    private void OnEnable()
    {
        currentMinInterval = minSpawnInterval;
        currentMaxInterval = maxSpawnInterval;
        timer = Random.Range(currentMinInterval, currentMaxInterval);
    }

    private void Update()
    {
        if (!DinoGameManager.instance.isPlaying) return;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SpawnObstacle();
            UpdateDifficulty();
            timer = Random.Range(currentMinInterval, currentMaxInterval);
        }
    }

    private void SpawnObstacle()
    {
        float speedMultiplier = DinoGameManager.instance.CurrentSpeed / 5f;

        int roll = Random.Range(0, 100);

        if (roll < 40 && groundObstacles.Length > 0)
        {
            SpawnFromArray(groundObstacles, new Vector3(spawnXPosition, groundYPosition, 0));
        }
        else if (roll < 70 && airObstacles.Length > 0)
        {
            SpawnFromArray(airObstacles, new Vector3(spawnXPosition, airYPosition, 0));
        }
        else if (destructibleObstacles.Length > 0)
        {
            SpawnFromArray(destructibleObstacles, new Vector3(spawnXPosition, groundYPosition, 0));
        }
    }

    private void SpawnFromArray(GameObject[] array, Vector3 position)
    {
        GameObject obj = GetPooledObject(array);
        if (obj != null)
        {
            obj.transform.position = position;
            obj.SetActive(true);
        }
    }

    private GameObject GetPooledObject(GameObject[] array)
    {
        int index = Random.Range(0, array.Length);
        GameObject prefab = array[index];

        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeInHierarchy && child.name.StartsWith(prefab.name))
            {
                return child.gameObject;
            }
        }

        GameObject newObj = Instantiate(prefab, transform);
        newObj.name = prefab.name;
        return newObj;
    }

    private void UpdateDifficulty()
    {
        float speedRatio = DinoGameManager.instance.CurrentSpeed / 5f;
        currentMinInterval = Mathf.Max(0.4f, minSpawnInterval / speedRatio);
        currentMaxInterval = Mathf.Max(0.8f, maxSpawnInterval / speedRatio);
    }
}
