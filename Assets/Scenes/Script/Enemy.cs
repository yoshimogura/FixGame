using UnityEngine;

public class 敵 : MonoBehaviour
{
    public Transform player;
    float chaseRange = 10f;
    float speed = 2f;
    float attackRange = 1f;

    private float timer;
    public float attackCooldown = 1f;

    private Animator animator;
    Player PlayerHP;

    private bool isChasing = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // 追跡開始
        if (!isChasing && distance < chaseRange)
        {
            isChasing = true;
        }
        // 追跡終了
        else if (isChasing && distance > chaseRange + 1f)
        {
            isChasing = false;
        }

        if (isChasing && distance > attackRange)
        {
            Vector3 target = new Vector3(player.position.x, transform.position.y, player.position.z);
            Vector3 direction = (target - transform.position).normalized;

            transform.position += direction * speed * Time.deltaTime;
            transform.LookAt(target);

            animator.Play("Scene");
        }

        // 攻撃処理
        if (distance <= attackRange)
        {
            timer += Time.deltaTime;

            if (timer >= attackCooldown)
            {
                Attack();
                timer = 0f;
            }
        }
        else
        {
            timer = attackCooldown;
        }
    }

    void Attack()
    {
        Debug.Log("攻撃した");
        PlayerHP = GameObject.Find("Player").GetComponent<Player>();
        PlayerHP.hp -= 10;
    }
}
