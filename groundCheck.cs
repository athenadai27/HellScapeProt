using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundCheck : MonoBehaviour
{
    private GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        Player = gameObject.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Platform")
        {
            Player.GetComponent<playerController>().isGrounded = true;
            Player.GetComponent<playerController>().animator.SetBool("isJump", false);
            if (Player.GetComponent<playerController>().hasDoubleJump) //Checks if player has doublejump powerup
            {
                Player.GetComponent<playerController>().jumpNum = 2;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Platform")
        {
            Player.GetComponent<playerController>().isGrounded = false;
            Player.GetComponent<playerController>().animator.SetBool("isJump", true);
        }
    }
}
