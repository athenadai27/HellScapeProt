using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossActivate : MonoBehaviour
{
    [SerializeField] GameObject boss;
    [SerializeField] GameObject bossHealthBar;
    // Start is called before the first frame update
    void Start()
    {
        boss.SetActive(false);
        bossHealthBar.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            boss.SetActive(true);
            bossHealthBar.SetActive(true);
        }
        gameObject.SetActive(false);
    }
}
