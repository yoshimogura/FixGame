using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Rigidbody rb;
    float speed = 80f;

    void FixedUpdate()
    {
        Quaternion rot = Quaternion.Euler(0, -speed * Time.fixedDeltaTime, 0);
        rb.MoveRotation(rb.rotation * rot);
    }
}
