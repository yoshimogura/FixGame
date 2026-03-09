using UnityEngine;

public class Stage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Rigidbody rb;
    float speed = 30f;

    void FixedUpdate()
    {
        Quaternion rot = Quaternion.Euler(0, 0, speed * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * rot);
    }

}
