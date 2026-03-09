using UnityEngine;

public class StageLiftMove : MonoBehaviour
{
    float moveDistance = 14f;   // 往復距離
    float moveSpeed = 0.5f;

    private Rigidbody rb;
    private Vector3 startPos;
    private float timer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        startPos = transform.position;
    }

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime * moveSpeed;
        float offset = Mathf.Sin(timer) * moveDistance;
        Vector3 targetPos = startPos + transform.right * offset;

        rb.MovePosition(targetPos);
    }


}
