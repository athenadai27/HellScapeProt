using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossAnimator : MonoBehaviour
{
    public float waitTime;
    public int attackNum;
    public Animator animator;
    public int totalAction;
    public int minAttackCount = 3;
    public int maxAttackCount = 6;

    private float resetTime;
    private int actionCount = 0;
    private bool attacked;
    private int attackCount = 0;
    private int currentMaxAttack = 4;

    // Start is called before the first frame update
    void Start()
    {
        resetTime = waitTime;
        //StartCoroutine(bossController());
    }

    // Update is called once per frame
    void Update()
    {

        // Idle - once run + attack > totalAction
        if (actionCount >= totalAction)
        {
            animator.Play("idle_b");
            StartCoroutine(waitTimer(0));
        }
        // Run
        else if (attackCount >= currentMaxAttack)
        {
            currentMaxAttack = Random.Range(minAttackCount, maxAttackCount + 1);
            animator.Play("run_b");
            StartCoroutine(waitTimer(1));
        }
        // Attack
        else if (!attacked)
        {
            int randomize = Random.Range(1, attackNum + 1);
            animator.Play("attack" + randomize);
            attacked = true;
            StartCoroutine(waitTimer(2));
        }

        //Debug.Log(actionCount);
        //yield return new WaitForSeconds(0.0f);
    }

    private IEnumerator waitTimer(int actionStatus)
    {
        // Idle
        if(actionStatus == 0)
        {
            yield return new WaitForSeconds(resetTime);
            actionCount = 0;
        }

        // Run
        if(actionStatus == 1)
        {
            yield return new WaitForSeconds(resetTime);
            attackCount = 0;
        }

        // Attack
        if(actionStatus == 2)
        {
            yield return new WaitForSeconds(resetTime);
            attacked = false;
            attackCount++;
            actionCount++;
        }
    }
}
