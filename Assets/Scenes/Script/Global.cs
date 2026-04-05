using UnityEngine;
using TMPro;

public class Global : MonoBehaviour
{
    public TextMeshProUGUI HPText;
    public Player PlayerHP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        PlayerHP = GameObject.Find("Player").GetComponent<Player>();
        HPText.text =PlayerHP.hp.ToString("F1"); 

        
    }
}
