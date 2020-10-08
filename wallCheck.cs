using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wallCheck : MonoBehaviour
{
    private GameObject Player;
    private playerController pc;
    private Collider2D wall;
    private bool canWallJump;

    // Start is called before the first frame update
    void Start()
    {
        Player = gameObject.transform.parent.gameObject;
        pc = Player.GetComponent<playerController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if ((collision.collider.tag == "BreakableWall")&&(pc.isDashing))
        {

            //break the wall
            Vector3 pos = collision.GetContact(0).point +( pc.direction * Vector2.right *.1f);
            UnityEngine.Debug.Log(pos);
            Vector3Int tilePos = pc.breakableTilemap.WorldToCell(pos);
            pc.breakableTilemap.SetTile(tilePos, null);
            //Destroy(collision.collider.gameObject);
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            Player.GetComponent<playerController>().isWalled = true;
            //wall = collision.gameObject.GetComponent<Collider2D>();
            //Player.GetComponent<playerController>().animator.SetBool("isJump", false);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            Player.GetComponent<playerController>().isWalled = false;
        }
    }
}
