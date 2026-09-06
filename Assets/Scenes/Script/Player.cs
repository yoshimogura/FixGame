using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 5f; 
    float rotateSpeed = 120f;
    float jumpForce = 7f;
    Vector3 StartPosition;
    Quaternion StartRotate;

    [Header("Ground Check Settings")]
    [SerializeField] float groundCheckDistance = 3f;
    [SerializeField] LayerMask groundMask;
    Vector3 groundNormal = Vector3.up;
    private bool isGrounded = true;
    [SerializeField] float airMaxVelocity = 15f;

    [Header("Player Status")]
    public int hp = 100;
    bool key = false;

    [Header("References")]
    public GameObject hitBox;
    private Vector2 moveInput;
    private Rigidbody rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        hitBox.SetActive(false);
        StartPosition = transform.position;
        StartRotate = transform.rotation;
    }

    void FixedUpdate()
    {
        // レイキャストによる接地判定と法線の取得（スロープ対策）
        isGrounded = CheckGround();

        Vector3 moveDirection = transform.forward * moveInput.y;

        // --- 移動処理 ---
        if (isGrounded)
        {
            // 1. 入力から、地面に対して「水平」な方向（X, Z）を計算
            Vector3 moveDir = transform.forward * moveInput.y; 

            // 2. 地面の傾き（groundNormal）に対して、真横（右方向）のベクトルを計算
            Vector3 rightDir = Vector3.Cross(groundNormal, transform.forward);
            
            // 3. 地面の傾きに100%沿った「正面（進むべき方向）」のベクトルを再計算
            Vector3 slopeDir = Vector3.Cross(rightDir, groundNormal).normalized;

            // 4. 斜面が急なほど速度が落ちるのを防ぐ補正
            float slopeModifier = Vector3.Dot(groundNormal, Vector3.up);
            slopeModifier = Mathf.Max(slopeModifier, 0.1f); 

            // 5. 完全に補正された速度を計算して適用
            Vector3 targetVelocity = slopeDir * (moveSpeed / slopeModifier) * moveInput.y;

            // ★修正1：ジャンプ直後（Y軸の上昇速度があるとき）はY速度を維持する
            if (rb.linearVelocity.y > 0.1f)
            {
                targetVelocity.y = rb.linearVelocity.y;
            }

            rb.linearVelocity = targetVelocity;
        }
        else
        {
            // 空中での進みたい方向（水平方向）を計算
            Vector3 airMoveDir = transform.forward * moveInput.y;

            rb.AddForce(airMoveDir * moveSpeed * 0.5f, ForceMode.Force);
        }

        // --- 回転処理 ---
        float rotation = moveInput.x * rotateSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotation, 0));

        // アニメーション設定
        animator.SetFloat("Speed", Mathf.Abs(moveInput.y) * 2f);

        // 攻撃アニメーション中はこれ以降の速度リミッターを通さない
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            return;

        // --- 最高速度の制限（水平方向のみ） ---
        if (!isGrounded) 
        {
            // 水平方向（X, Z）の速度ベクトルを取り出す
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            
            // 設定した「空中の最大速度」を超えていたら、その速度に固定する
            if (horizontalVelocity.magnitude > airMaxVelocity) 
            {
                Vector3 limitedHorizontalVelocity = horizontalVelocity.normalized * airMaxVelocity;
                
                // Y速度（ジャンプの上昇・落下）はそのまま維持し、移動スピードだけを抑える
                rb.linearVelocity = new Vector3(limitedHorizontalVelocity.x, rb.linearVelocity.y, limitedHorizontalVelocity.z);
            }
        }

        if (transform.position.y < 0)
        {
            ReSpawn();
        }
    }

    private bool CheckGround()
{
    // 胴体の中心（高めの位置）から飛ばす
    Vector3 origin = transform.position + Vector3.up * 1.1f;
    RaycastHit hit;
    
    // 坂道での接地外れを防ぐため、少し長め（1.4f）に設定
    float maxDistance = 1.4f; 

    if (Physics.Raycast(origin, Vector3.down, out hit, maxDistance, groundMask))
    {
        if (hit.collider.gameObject != this.gameObject)
        {
            // 壁ではなく「坂道」と判定できる角度（50度以内）かチェック
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (slopeAngle <= 50f)
            {
                groundNormal = hit.normal;
                return true;
            }
        }
    }

    groundNormal = Vector3.up;
    return false;
}

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (isGrounded)
        {
            // ★修正2：ジャンプした瞬間に水平方向の速度が airMaxVelocity を超えていたら抑える
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            if (horizontalVelocity.magnitude > airMaxVelocity)
            {
                Vector3 limitedHorizontal = horizontalVelocity.normalized * airMaxVelocity;
                rb.linearVelocity = new Vector3(limitedHorizontal.x, rb.linearVelocity.y, limitedHorizontal.z);
            }

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    public void OnAttack(InputValue value)
    {
        Debug.Log("押した");
        if (value.isPressed)
        {
            animator.SetTrigger("Attack");
            Debug.Log("攻撃！");
        }
    }

    public void OnRespawn(InputValue value)
    {
        Debug.Log("リセット押した");
        if (value.isPressed)
        {
            ReSpawn();
        }
    }   

    // 物理的な衝突判定（壁、ドア、敵など固いもの）
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Door") && key)
        {
            Debug.Log("ゲートきた");
            key = false;
            Destroy(collision.gameObject);
        }
    }

    // トリガー（すり抜ける設定）に触れたときの判定（汎用アイテムなど）
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Battery"))
        {
            Debug.Log("鍵ゲット");
            key = true;
            Destroy(other.gameObject);
        }
    }

    void ReSpawn()
    {
        rb.linearVelocity = Vector3.zero;   
        rb.angularVelocity = Vector3.zero;
        transform.position = StartPosition;
        transform.rotation = StartRotate;
    }

    public void EnableHitBox()
    {
        hitBox.SetActive(true);
        Invoke("DisableHitBox", 0.2f);
    }

    void DisableHitBox()
    {
        hitBox.SetActive(false);
    }
}