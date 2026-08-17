using UnityEngine;

public class ShotEnemy : MonoBehaviour
{
    [Header("参照")]
    public Transform player;          // プレイヤーのTransform
    public GameObject bulletPrefab;   // 発射する弾のプレハブ
    public Transform spawnPoint;      // 弾の出現位置（敵の手元や口など）

    [Header("攻撃設定")]
    public float attackInterval = 2f; // 攻撃のインターバル（秒）
    public float detectionRange = 15f;// プレイヤーを感知する距離
    public float playerHeightOffset = 1.0f; // ★プレイヤーの狙う高さのオフセット（1.0でおよそ胸〜頭）

    private float timer = 0f;

    void Update()
    {
        if (player == null) return;

        // プレイヤーとの距離を計算
        float distance = Vector3.Distance(transform.position, player.position);

        // プレイヤーが射程内にいる場合
        if (distance <= detectionRange)
        {
            // 1. プレイヤーの狙う位置（足元 + 高さオフセット）を計算
            Vector3 targetPosition = player.position + Vector3.up * playerHeightOffset;

            // 2. 敵本体は水平方向だけプレイヤーに向ける（体が上下に傾くのを防ぐため）
            Vector3 lookAtBodyPos = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            transform.LookAt(lookAtBodyPos);

            // 3. 弾の発射口（spawnPoint）があれば、発射口自体をプレイヤーの高さへ向ける
            if (spawnPoint != null)
            {
                spawnPoint.LookAt(targetPosition);
            }

            // タイマーを加算して攻撃
            timer += Time.deltaTime;
            if (timer >= attackInterval)
            {
                Shoot(targetPosition);
                timer = 0f;
            }
        }
    }

    void Shoot(Vector3 targetPosition)
    {
        if (bulletPrefab == null) return;

        // 発射位置の決定
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;
        
        // 弾の向きを「狙う位置（高さ含む）」へ向ける
        Quaternion spawnRotation;
        if (spawnPoint != null)
        {
            spawnRotation = spawnPoint.rotation;
        }
        else
        {
            // spawnPointがない場合は発射位置からターゲットへの回転を計算
            Vector3 direction = (targetPosition - spawnPos).normalized;
            spawnRotation = Quaternion.LookRotation(direction);
        }

        // 弾を生成
        Instantiate(bulletPrefab, spawnPos, spawnRotation);
    }
}
