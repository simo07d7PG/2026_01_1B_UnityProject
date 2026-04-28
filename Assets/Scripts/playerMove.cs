using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMove : MonoBehaviour
{
    [Header("기본 이동 설정")]
    public float moveSpeed = 5.0f;                                      // 이동 속도 변수 설정
    public float jumpForce = 7.0f;                                      // 점프 힘 값을 준다 
    public Rigidbody rb;                                                // 플레이어 강체 선언

    [Header("점프 개선 설정")]
    public float fallMultiplier = 2.5f;                                 // 하강 중력 배율
    public float lowJumpMultiplier = 2.0f;                              // 짧은 점프 배율

    [Header("지면 감지 설정")]
    public float coyoteTime = 0.15f;                                    // 지면 관성 시간
    public float coyoteTimeCounter;                                     // 관성 타이머
    public bool realGround = true;                                      // 실제 지면 상태
    public bool isGrounded = true;                                      // 땅에 있는지 체크하는 변수

    [Header("글라이더 설정")]
    public GameObject gliderObject;
    public float gliderFallSpeed = 1.0f;
    public float gliderMoveSpeed = 7.0f;
    public float gliderMaxTime = 5.0f;
    public float gliderTimeLeft;
    public bool isGliding = false;


    public int coinCount = 0;
    public float turnSpeed = 5.0f;                                     // 먹은 코인의 개수를 확인하는 변수

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // rb = gameObject.GetComponent<Rigidbody>();
        coyoteTimeCounter = 0.0f;

        if (gliderObject != null)
        {
            gliderObject.SetActive(false);
        }

        gliderTimeLeft = gliderMaxTime;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGroundedState();

        // 움직임 입력
        float moveHorizontal = Input.GetAxis("Horizontal");             // 수평 이동
        float moveVertical = Input.GetAxis("Vertical");                 // 수직 이동

        // G키로 글라이더 제어 (누르는 동안 활성화)

        if (Input.GetKey(KeyCode.G) && !isGrounded && gliderTimeLeft > 0)            // 키를 누름ㄴ서 땅에 있지 않고 글라이더 남은 시간이 있을 때
        {
            if (!isGliding)                      // 글라이더 활성화
            {
                EnableGlider();
            }

            // 글라이더 사용 시간 감소
            gliderTimeLeft -= Time.deltaTime;

            // 글라이더 시간이 다 되면 비활성화
            if (gliderTimeLeft <= 0)
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
            // 착시 점프 높이 구현
            if (rb.linearVelocity.y < 0)
            {
                rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime; // 하강 시 중력 강화
            }
            else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }

        }

        // 이동 방햑 벡터
        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);        // 이동 방향 감지

        if (movement.magnitude > 0.1f)                                   // 입력이 있을 때만 회전
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);      // 이동 방향을 바라보도록 부드럽게 회전
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        rb.linearVelocity = new Vector3(moveHorizontal * moveSpeed, rb.linearVelocity.y, moveVertical * moveSpeed);



        if (Input.GetButtonDown("Jump") && isGrounded)                   // && 두 값을 만족할 때 -> (스페이스 버튼을 눌렀을때와 isGrounded 가 True 일때
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);     // 위쪽으로 설정한 힘 만큼 물체에 힘을 준다 
            isGrounded = false;
            realGround = false;
            coyoteTimeCounter = 0;                                   // 점프를 하는 순간 땅에서 떨어졌기 때문에 false 로 한다
        }
    }

    private void OnCollisionEnter(Collision collision)                  // 충돌 처리 함수
    {
        if (collision.gameObject.tag == "Ground")                       // 충돌이 일어난 물체의 Tag가 Ground 인 경우
        {
            realGround = true;                                          // 땅과 충돌하면 True 가 된다
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")                       // 충돌이 일어난 물체의 Tag가 Ground 인 경우
        {
            realGround = true;                                          // 땅과 충돌하면 True 가 된다
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")                       // 충돌이 일어난 물체의 Tag가 Ground 인 경우
        {
            realGround = false;                                          // 땅과 충돌하면 True 가 된다
        }
    }

    private void OnTriggerEnter(Collider other)                         // 트리거 영역 안에 들어왔나를 검사하는 함수
    {
        if (other.CompareTag("Coin"))                                    // 코인 트리거와 충돌 하면
        {
            coinCount++;                                                // 코인 변수 1을 올린다
            Destroy(other.gameObject);                                  // 코인 오브젝트를 파괴한다
        }
    }

    void UpdateGroundedState()
    {
        if (realGround)
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
            {
                isGrounded = false;
            }
        }
    }

    // 글라이더 활성화 함수

    void EnableGlider()
    {
        isGliding = true;

        // 글라이더 오브젝트 표시

        if (gliderObject != null)
        {
            gliderObject.SetActive(true);
        }

        // 하강 속도 초기화
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, -gliderFallSpeed, rb.linearVelocity.z);
    }

    void DisableGlider()
    {
        isGliding = false;

        // 글라이더 오브젝트 숨기기\
        if (gliderObject != null)
        {
            gliderObject.SetActive(false);
        }

        // 즉시 낙하하도록 중력 적용

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
    }
    void ApplyGliderMovement(float horizontal, float vertical)
    {
        Vector3 gliderVelocity = new Vector3(
                horizontal * gliderMoveSpeed,
                -gliderFallSpeed,
                vertical * gliderMoveSpeed);

        rb.linearVelocity = gliderVelocity;
    }
}