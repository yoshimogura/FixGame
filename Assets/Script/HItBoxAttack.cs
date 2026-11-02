using UnityEngine;

public class HItBoxAttack : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        
    }
        void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("殴った");
            Destroy(other.gameObject); // 敵を倒す
        }
    }

}
