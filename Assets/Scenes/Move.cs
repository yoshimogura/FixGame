using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    float moveSpeed = 7f;
    float rotateSpeed = 120f;
    float jumpForce = 6f;

    private Vector2 moveInput;
    private Rigidbody rb;
    private bool isGrounded = true;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        // 前後移動（Rigidbodyで統一）
        Vector3 move = transform.forward * moveInput.y * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);

        // 左右回転（Rigidbodyで回転させる）
        float rotation = moveInput.x * rotateSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotation, 0));

        animator.SetFloat("Speed", Mathf.Abs(moveInput.y) * 2f);
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

    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }
}
