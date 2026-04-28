using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;

public class FruitGame : MonoBehaviour
{
    public GameObject[] fruitPrefabs;                                                           // 과일 프리팹 배열 선언
    public float[] fruitSizes = { 0.5f, 0.7f, 0.9f, 1.1f, 1.3f, 1.5f, 1.7f, 1.9f };             // 과일 크기 선언

    public GameObject currentFruit;                                                             // 현재 들고 있는 과일
    public int currentFruitType;                                                                // 현재 들고 있는 과일 타입

    public float fruitStartHeight = 6.0f;                                                       // 과일 시작시 높이 설정
    public float gameWidth = 5.0f;                                                              // 게임판 너비
    public float fruitTimer = -3.0f;
    public bool isGameOver = false;                                                             // 게임 상태
    public Camera mainCamera;                                                                   // 카메라 위치 (마우스 위치 변환에 필요)

    private void Start()
    {
        mainCamera = Camera.main;
        SpawnNewFruit();
    }

    private void Update()
    {
        if (isGameOver) return;

        if (fruitTimer >= 0)
        {
            fruitTimer -= Time.deltaTime;
        }
        if (fruitTimer < 0 && fruitTimer > -2)
        {
            SpawnNewFruit();
            fruitTimer = -3.0f;
        }

        if (currentFruit != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

            Vector3 newPosition = currentFruit.transform.position;
            newPosition.x = worldPosition.x;

            float halfFruitSize = fruitSizes[currentFruitType] / 2f;

            if(newPosition.x < -gameWidth / 2 + halfFruitSize)
            {
                newPosition.x = -gameWidth / 2 + halfFruitSize;
            }
            if (newPosition.x < -gameWidth / 2 + halfFruitSize)
            {
                newPosition.x = -gameWidth / 2 + halfFruitSize;
            }

            currentFruit.transform.position = newPosition;
        }
        if (Input.GetMouseButtonDown(0) && fruitTimer == -3.0f)
        {
            DropFruit();
        }
    }

    void SpawnNewFruit()
    {
        if (!isGameOver)
        {
            currentFruitType = Random.Range(0, 3);                                                                          // 0-2 사이 랜덤 과일 타입

            Vector3 mousePosition = Input.mousePosition;                                                                            // 마우스 위치 받아옴
            Vector3 worldPosition=mainCamera.ScreenToWorldPoint(mousePosition);                                                     // 마우스 2D 위치 -> 3D 월드 위치

            Vector3 spawnPosition = new Vector3(worldPosition.x, fruitStartHeight, 0);                                              // X 좌표만 사용, 나머지 설정 값

            float halfFruitSize = fruitSizes[currentFruitType] / 2f;
            spawnPosition.x = Mathf.Clamp(spawnPosition.x, -gameWidth / 2 + halfFruitSize, gameWidth / 2 - halfFruitSize);

            currentFruit = Instantiate(fruitPrefabs[currentFruitType], spawnPosition, Quaternion.identity);                         // 과일 생성
            currentFruit.transform.localScale = new Vector3(fruitSizes[currentFruitType], fruitSizes[currentFruitType], 1);         // 과일 크기 설정

            Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D>();

            if(rb != null)
            {
                rb.gravityScale = 0f;       // 시작 시 중력 스케일 = 0
            }
        }

    }

    void DropFruit()
    {
        Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 1f;
            currentFruit = null;
            fruitTimer = 1.0f;
        }
    }

    public void MergeFruits(int fruitType, Vector3 position)
    {
        if (fruitType < fruitPrefabs.Length - 1)
        {
            GameObject newFruit = Instantiate(fruitPrefabs[fruitType+1], position, Quaternion.identity);
            newFruit.transform.localScale = new Vector3(fruitSizes[fruitType + 1], fruitSizes[fruitType + 1], 1.0f);
        }
    }
}
