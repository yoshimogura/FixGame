using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 9f; 
    float rotateSpeed = 120f;
    float jumpForce = 1f;
    int maxVelocity = 12;
    Vector3 StartPosition;
    Quaternion StartRotate;

    [Header("Ground Check Settings")]
    [SerializeField] float groundCheckDistance = 3f;
    [SerializeField] LayerMask groundMask;
    Vector3 groundNormal = Vector3.up;
    private bool isGrounded = true;
    [SerializeField] float airMaxVelocity = 10f;
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
        StartPosition= transform.position;
        StartRotate=transform.rotation;
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
            rb.linearVelocity = targetVelocity;
        }
        else
        {
            // 空中での進みたい方向（水平方向）を計算
            Vector3 airMoveDir = transform.forward * moveInput.y;

            rb.AddForce(airMoveDir * moveSpeed * 0.1f, ForceMode.Force);
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
        if(transform.position.y<0){
            ReSpawn();
        }
    }
    private bool CheckGround()
    {
        // 出発地点を「足元」ではなく、キャラクターの「お腹の高さ（上方に0.5m）」に変更
        Vector3 origin = transform.position + Vector3.up * 0.8f;
        RaycastHit hit;

        // レイを飛ばす際、自分自身のオブジェクト(gameObject)を無視する設定を追加
        // ※光線の長さは、高くなった分（0.5m）を考慮して、少し長めに設定します
        float maxDistance = groundCheckDistance + 0.8f;

        // レイキャストを実行（groundMaskの設定が正しければこれで確実に床を検知します）
        if (Physics.Raycast(origin, Vector3.down, out hit, maxDistance, groundMask))
        {
            // 念のため、当たった相手が自分自身（Player）じゃない場合のみ地面とする
            if (hit.collider.gameObject != this.gameObject)
            {
                groundNormal = hit.normal;
                return true;
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
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }
    public void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            animator.SetTrigger("Attack");
            Debug.Log("攻撃！");
        }
    }
    public void OnRespawn(InputValue value){
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
     void ReSpawn(){
        rb.linearVelocity = Vector3.zero;  
        rb.angularVelocity = Vector3.zero;
        transform.position=StartPosition;
        transform.rotation=StartRotate;
    }

    /// 足元からレイを飛ばして接地状態と地面の法線をチェックする
    

    public void EnableHitBox()
    {
        hitBox.SetActive(true);
        Invoke("DisableHitBox", 0.1f);
    }

    void DisableHitBox()
    {
        hitBox.SetActive(false);
    }
}