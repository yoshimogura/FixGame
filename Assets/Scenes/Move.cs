

using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    float moveSpeed = 10f;
    float rotateSpeed = 120f; // ← 回転速度
    float jumpForce = 10f;

    private Vector2 moveInput;
    private Rigidbody rb;
    private bool isGrounded = true;
    private Animator animator;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>(); // ← ここで取得

    }
    void FixedUpdate()
    {
        Vector3 move = Quaternion.Euler(0, 0, 0) * transform.forward
        * moveInput.y * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }

    void Update()
    {
        // W/S：前後移動
        Vector3 forwardMove = Quaternion.Euler(0, 0, 0) * transform.forward * moveInput.y * moveSpeed * Time.deltaTime;
        //ジャンプ
        transform.Translate(forwardMove, Space.World);

        // A/D：左右回転
        float rotation = moveInput.x * rotateSpeed * Time.deltaTime;
        transform.Rotate(0, rotation, 0);

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
        // 地面に触れたらジャンプ可能に
        isGrounded = true;
    }

}

