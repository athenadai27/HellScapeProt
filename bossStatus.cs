using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossStatus : MonoBehaviour
{
    public GameObject bossHealth;

    private float healthMultiplier = 0.1f;
    private float healthTotal = 1.0f;

    //For Medium difficulty
    private screenShake shake;
    private SpriteRenderer sr;
    private float colorWait = 0.2f;
    private Color white;

    Collider2D boss;//to use isTouching()
    Collider2D player;//to use isTouching()
    public bool hasPunched;//to see if a punch has been thrown
    public GameObject playerObject;//to reference player scripts
    public bool _isGrounded;//to reset the punch
    public float _jumpNum;//combo system counter on second jump
    public bool didCombo;
    [SerializeField] GameObject winObj;

    void Start()
    {
        shake = GameObject.FindGameObjectWithTag("ShakeTest").GetComponent<screenShake>();
        sr = GetComponent<SpriteRenderer>();
        white = sr.color;
        boss = gameObject.GetComponent<Collider2D>();
        Debug.Log("b: " + boss);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        hasPunched = false;
        didCombo = false;
    }

/*Athena's damage code
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            UnityEngine.Debug.Log("Hit Boss!");
            shake.CamShake();
            StartCoroutine("colorChange");
            healthTotal = healthTotal - healthMultiplier;
            bossHealth.GetComponent<healthManip>().changeHealth(healthTotal);
            if (healthTotal <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
*/

    void Update()//
    {
        Debug.Log(boss.IsTouching(player));
        if(boss.IsTouching(player) && Input.GetKeyDown(KeyCode.F) && !hasPunched)//punch if not punched
        {

            UnityEngine.Debug.Log("Hit Boss!");
            shake.CamShake();
            StartCoroutine("colorChange");
            healthTotal = healthTotal - healthMultiplier;
            bossHealth.GetComponent<healthManip>().changeHealth(healthTotal);
            if (healthTotal <= 0)
            {
                Debug.Log("Health Zero!");
                winObj.SetActive(true);
                Destroy(gameObject);
                playerObject.GetComponent<playerController>().startGameOver(3);
            }
            hasPunched = true;
        }
        Debug.Log(boss);
        Debug.Log(player);
        _isGrounded = playerObject.GetComponent<playerController>().getGrounded();//grounded status
        _jumpNum = playerObject.GetComponent<playerController>().getJump();//jump number status(refer Athena's code)
        if (_isGrounded)//punch and combo reset after grounding
        {
            hasPunched = false;
            didCombo = false;
        }
        if (_jumpNum == 0 && !didCombo)//if he hasn't done the combo and is on the second jump, do combo
        {
            hasPunched = false;
            didCombo = true;//can't do combo anymore
        }

    }

    IEnumerator colorChange()
    {
        sr.color = new Color(1, 0, 0);
        yield return new WaitForSeconds(colorWait);
        sr.color = white;
        //UnityEngine.Debug.Log("Color Changed!");
    }
}
