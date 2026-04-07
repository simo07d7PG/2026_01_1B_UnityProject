using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public GameObject missilePrefab;

    [Header("스폰 타이밍 설정")]
    public float minSpawnInterval = 0.5f; // 최소 생성 간격
    public float maxSpawnInterval = 2.0f; // 최대 생성 간격

    public float timer = 0f;
    public float nextSpawnTime;


    [Header("동전 스폰 확률 설정")]
    public int coinSpawnChance = 50;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > nextSpawnTime)
        {
            SpawnObject();
            timer = 0f;         // 시간 초기화
            SetNextSpawnTime(); // 함수 실행 
        }
    }

    void SpawnObject()
    {
        Transform spawnTransform = transform;
        int randomValue = Random.Range(0, 100);
        if (randomValue < coinSpawnChance)
            Instantiate(missilePrefab, spawnTransform.position, spawnTransform.rotation);
        else
            Instantiate(coinPrefab, spawnTransform.position, spawnTransform.rotation);
    }
    void SetNextSpawnTime() // 최소 - 최대 사이의 랜덤시간 설정
    {
        nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
    }
}
