using UnityEngine;

public class MyCharacter : MonoBehaviour
{
    public int health = 100;  // 체력을 선언
    public float timer = 1.0f; // 타이머 설정
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health += 100; // 시작 시 체력에 100을 더한다
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            timer = 1.0f;
            health -= 20;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            health += 2;
        }

        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
