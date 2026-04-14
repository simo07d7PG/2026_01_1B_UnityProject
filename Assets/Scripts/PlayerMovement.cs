using NUnit.Framework.Constraints;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f; // 플레이어 이동 속도
    public float jumpForce = 5.0f; // 플레이어 힘 값
    public float turnSpeed = 1.0f; // 플레이어 회전 속도

    public float fallMultiplier = 2.5f;
    public float lowJujmpMultiplier = 2.0f;

    public float coyoteTime = 0.15f;
    public float coyoteTimeCounter;
    public bool realGrounded = true;

    public GameObject gliderObject;
    public float gliderFallSpeed = 1.0f;
    public float gliderMoveSpeed = 7.0f;
    public float gliderMaxTime = 5.0f;
    public float gliderTimeLeft;
    public bool isGliding = false;
    
    public Rigidbody rb;           // RigidBody

    public bool isGrounded = true; // 플레이어 점프 가능

    public int coinCount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coyoteTimeCounter = 0;

        if(gliderObject != null)
        {
            gliderObject.SetActive(false);
        }
        gliderTimeLeft = gliderMaxTime;
    }

    // Update is called once per frame
    void Update()
    {

        UpdateGroundedState();
        float moveHorizontal = moveSpeed * Input.GetAxis("Horizontal"); // 움직임 입력 (가로)
        float moveVertical   = moveSpeed * Input.GetAxis("Vertical");   // 움직임 입력 (세로)

        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);

        if(movement.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.G) && !isGrounded && gliderTimeLeft > 0)
        {
            if (!isGliding)
            {
                EnableGlider();
            }
            gliderTimeLeft -= Time.deltaTime;
            if(gliderTimeLeft <= 0)
            {
                DisableGlider();
            }
        }
        else if (isGliding)
        {
            DisableGlider();
        }

        if (isGliding)
        {
            ApplyGliderMovement(moveHorizontal, moveVertical);
        }
        else
        {
            rb.linearVelocity = new Vector3(moveHorizontal, rb.linearVelocity.y, moveVertical); // 속도값으로 직접 이동

            if(rb.linearVelocity.y < 0)
            {
                rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJujmpMultiplier - 1) * Time.deltaTime;
            }
        }

        if (Input.GetButtonDown("Jump") && isGrounded) // 강체 상태이며 점프 버튼이 감지되었을 때
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // 설정한 힘 만큼 위로 물체에 힘을 준다
            realGrounded = false;
            isGrounded = false; // 땅에 떨어졌기 때문에 false
            coyoteTimeCounter = 0;
        }

        if (isGrounded)
        {
            if (isGliding)
            {
                DisableGlider();
            }
            gliderTimeLeft = gliderMaxTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground") // 충돌한 gameObject가 Ground라면 점프 조건 활성
        {
            realGrounded = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            realGrounded = true;
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

    private void UpdateGroundedState()
    {
        if (realGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            isGrounded = true;
        }
        else
        {
            if (coyoteTimeCounter > 0)
            {
                coyoteTimeCounter -= Time.deltaTime;
                isGrounded = true;
            }
            else 
                isGrounded = false;
        }
    }

    private void EnableGlider()
    {
        isGliding = true;
        if (gliderObject != null)
        {
            gliderObject.SetActive(true);
        }
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, -gliderFallSpeed, rb.linearVelocity.z);
    }

    private void DisableGlider()
    {
        isGliding = false;
        if(gliderObject != null)
        {
            gliderObject.SetActive(false);
        }
        rb.linearVelocity = new Vector3(rb.linearVelocity.z, 0, rb.linearVelocity.z);
    }

    void ApplyGliderMovement(float horizontal, float vertical)
    {
        Vector3 gliderVelocity = new Vector3(
            horizontal * gliderMoveSpeed, -gliderFallSpeed, vertical * gliderMoveSpeed);
        rb.linearVelocity = gliderVelocity;
    }
}
