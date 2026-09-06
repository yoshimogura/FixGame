using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 8f;    
    public float lifeTime = 4f;   
    Player Player;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground")|| other.CompareTag("Ground"))
        { 
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player = GameObject.Find("Player").GetComponent<Player>();
            Debug.Log("命中");
            Player.hp-=10;
            Debug.Log(Player.hp);
            Destroy(gameObject);
        }
    }
}
