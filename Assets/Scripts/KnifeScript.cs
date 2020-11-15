using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeScript : MonoBehaviour
{
    
    public float moveSpeed = 10f;
    private Transform rightTarget;
    private Transform leftTarget;
    private Collider2D collider;
    
    // Start is called before the first frame update
    void Start()
    {
        rightTarget.position = new Vector2(86.19f, -3.594918f);
        leftTarget.position = new Vector2(-42.35f, -3.594918f);
        collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.localScale.x > 0)
        {
            //If we are talking about the right knife
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(191.19f, -3.594918f), moveSpeed * Time.deltaTime);
            
        }
        else
        {
            //If we are talking about the left knife
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(-140.35f, -3.594918f), moveSpeed * Time.deltaTime);
            
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!(other.gameObject.CompareTag("Player")))
        {
            Destroy(gameObject);            
        }
        else
        {
            Physics2D.IgnoreCollision(other.gameObject.GetComponent<Collider2D>(), collider);
        }
        
    }
}
