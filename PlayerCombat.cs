using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerCombat : MonoBehaviour
{

    public Animator a;

    public Transform attackpoint;
    public float attackrange = 0.5f;
    public LayerMask enemylayers;
    public int attackdamage = 10;

    public float attackrate = 2f;
    float nextattacktime = 0f;

    void Update()
    {
        if (Time.time >= nextattacktime)
        {
            if (CrossPlatformInputManager.GetButtonDown("Attack"))
            {
                //a.SetTrigger("Attack");
                //StartCoroutine(TimeDelay());
                Attack();
                nextattacktime = Time.time + 1f / attackrate;
            }
        }


    }
    void Attack()
    {
        //animation
        a.SetTrigger("Attack");
        //detect enemies in range
        Collider2D[] hitenemies = Physics2D.OverlapCircleAll(attackpoint.position, attackrange, enemylayers);
        //damage
        foreach (Collider2D enemy in hitenemies)
        {
            enemy.GetComponent<enemy>().takedamage(attackdamage);
        }
    }
    void OnDrawGizmosSelected()
    {
        if (attackpoint == null)
            return;

        Gizmos.DrawWireSphere(attackpoint.position, attackrange);
    }
}
//IEnumerator TimeDelay()
//{
    //yield return new WaitForSeconds(5);
//}


