using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f; // 플레이어 이동 속도
    public float jumpForce = 5.0f; // 플레이어 힘 값

    public Rigidbody rb;           // RigidBody

    public bool isGrounded = true; // 플레이어 점프 가능

    public int coinCount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = moveSpeed * Input.GetAxis("Horizontal"); // 움직임 입력 (가로)
        float moveVertical   = moveSpeed * Input.GetAxis("Vertical");   // 움직임 입력 (세로)

        rb.linearVelocity = new Vector3(moveHorizontal, rb.linearVelocity.y, moveVertical); // 속도값으로 직접 이동

        if (Input.GetButtonDown("Jump") && isGrounded) // 강체 상태이며 점프 버튼이 감지되었을 때
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // 설정한 힘 만큼 위로 물체에 힘을 준다
            isGrounded = false; // 땅에 떨어졌기 때문에 false
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground") // 충돌한 gameObject가 Ground라면 점프 조건 활성
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            coinCount++;
            Destroy(other.gameObject);
        }
    }
}
