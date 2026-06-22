using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 20f; 
    float rotateSpeed = 120f;
    float jumpForce = 1f;

    private Vector2 moveInput;
    private Rigidbody rb;
    private bool isGrounded = true;
    private Animator animator;
    public int hp = 100;
    public GameObject hitBox;
    int maxVelocity=12;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        hitBox.SetActive(false);
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = transform.forward * moveInput.y;

        if (isGrounded)
        {
            // 【地上】キビキビ動かしたいので VelocityChange のまま
            Vector3 targetVelocity = moveDirection * moveSpeed;
            rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
        }
        else
        {
            rb.AddForce(moveDirection * moveSpeed * 0.3f, ForceMode.Force);
        }

        // --- 回転処理 ---
        float rotation = moveInput.x * rotateSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotation, 0));

        // アニメーション設定
        animator.SetFloat("Speed", Mathf.Abs(moveInput.y) * 2f);

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            return;

        // --- 最高速度の制限（水平方向のみ） ---
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (horizontalVelocity.magnitude > maxVelocity) 
        {
            Vector3 limitedHorizontalVelocity = horizontalVelocity.normalized * maxVelocity;
            rb.linearVelocity = new Vector3(limitedHorizontalVelocity.x, rb.linearVelocity.y, limitedHorizontalVelocity.z);
        }
            
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

    private void OnCollisionEnter(Collision collision)
    {
        // 地面判定（簡易的）
        isGrounded = true;
        if (collision.gameObject.CompareTag("GateHitJudgment"))
        {
            Debug.Log("ゲートきた");
        }
    }

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