using UnityEngine;

public class Wind : MonoBehaviour
{
    [Header("風の設定")]
    float windStrength = 3f;

    // トリガー（エリア内）に誰かが入っている間、ずっと実行される
    private void OnTriggerStay(Collider other)
    {
        // 触れたオブジェクトに Rigidbody が付いているかチェック
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(Vector3.up * windStrength, ForceMode.Force);
        }
    }
}
