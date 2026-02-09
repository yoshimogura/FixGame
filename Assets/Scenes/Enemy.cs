using UnityEngine;

public class 敵 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform player;
    float chaseRange =12f;//追跡開始の距離
    float speed=2f;
    float AtackRange=1f;
    private float timer;
    public float attackCooldown = 1f; // 攻撃の間隔
    private Animator animator;


    void Start(){
        animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        //距離図ってる↓
        float distance = Vector2.Distance(transform.position, player.position);
        
        if (distance < chaseRange && distance > AtackRange)
        {
            Vector3 target = new Vector3(player.position.x, transform.position.y, player.position.z);
            Vector3 direction = (target - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            transform.LookAt(target);
            animator.Play("Scene");

        }
        if (distance <= AtackRange)
{
    timer += Time.deltaTime;

    // 攻撃距離に入った瞬間に即攻撃
    if (timer >= attackCooldown)
    {
        Attack();
        timer = 0f;
    }
}
else
{
    // 距離から離れたら timer をリセット
    timer = attackCooldown;
}

    }
    void Attack(){
        Debug.Log("攻撃した");
    }
}
