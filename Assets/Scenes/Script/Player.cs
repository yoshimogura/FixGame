using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 0.2f; 
    float rotateSpeed = 120f;
    float jumpForce = 6f;

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
        // --- 移動処理 (AddForce + Impulse) ---
        // キャラクターの正面方向に対して力を加える
        Vector3 moveDirection = transform.forward * moveInput.y;
        
        rb.AddForce(moveDirection * moveSpeed, ForceMode.Impulse);

        // --- 回転処理 ---
        float rotation = moveInput.x * rotateSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotation, 0));

        // アニメーション設定
        animator.SetFloat("Speed", Mathf.Abs(moveInput.y) * 2f);

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            return;
        if (rb.linearVelocity.magnitude > maxVelocity) {
            rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;
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