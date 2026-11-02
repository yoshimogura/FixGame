using UnityEngine;

public class RotateLift : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   [Header("回転の設定")]
    [SerializeField] Vector3 rotationSpeed = new Vector3(0f, 30f, 0f); // 1秒間の回転角度

    private Rigidbody liftRb;

    void Start()
    {
        // リフト自身のRigidbodyを取得
        liftRb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // ★【重要】transform.Rotate ではなく、物理演算（MoveRotation）で安全に回転させる
        Quaternion deltaRotation = Quaternion.Euler(rotationSpeed * Time.fixedDeltaTime);
        liftRb.MoveRotation(liftRb.rotation * deltaRotation);
    }

    // プレイヤーが上に乗っている間、ずっと実行される
    private void OnCollisionStay(Collision collision)
    {
        // 乗っているのがプレイヤーかチェック
        Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
        
        if (playerRb != null)
        {
            // 1. リフトの回転によって、その地点が「1秒間にどれくらいの速度で動いているか」を計算
            //（外側の席ほどスピードが速くなる観覧車のような計算です）
            Vector3 contactPoint = collision.GetContact(0).point;
            Vector3 liftVelocity = Vector3.Cross(rotationSpeed * Mathf.Deg2Rad, contactPoint - transform.position);

            // 2. プレイヤーの水平速度（X, Z）に、リフトの動く速度をそのまま足し算する
            Vector3 finalVelocity = playerRb.linearVelocity;
            finalVelocity.x += liftVelocity.x;
            finalVelocity.z += liftVelocity.z;

            // 3. 計算した速度をプレイヤーに適用する
            playerRb.linearVelocity = new Vector3(finalVelocity.x, playerRb.linearVelocity.y, finalVelocity.z);
        }
    }
}
