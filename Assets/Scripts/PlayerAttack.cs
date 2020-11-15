using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private float timeBtwAttack;
    public float startTimeBtwAttack;

    public Animator animator;
    
    public float attackRange;
    
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) ;
        {
            Attack();
        }
    }

    void Attack()
    {
        //Attack animation
        animator.SetTrigger("Attack");
        //Detect enemies in range
        //Damage enemies
    }
}
