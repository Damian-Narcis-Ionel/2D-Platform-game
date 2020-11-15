using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashScript : MonoBehaviour
{
    private Rigidbody2D rb;
    public float dashSpeed;
    private float dashTime;
    public float startDashTime;
    private int direction;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dashTime = startDashTime;
    }

    // Update is called once per frame
    void Update()
    {
        
            Dash();
        
        

    }

    void Dash()
    {
        
            if (direction == 0)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    direction = 1;
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    direction = 2;
                }

            }
            else
            {
                if (dashTime <= 0)
                {
                    direction = 0;
                    dashTime = startDashTime;
                    rb.velocity = Vector2.zero;
                }
                else
                {
                    dashTime -= Time.deltaTime;
                    if (direction == 1)
                    {
                        rb.velocity = Vector2.left * dashSpeed;
                        transform.localScale = new Vector2(-.5f, transform.localScale.y);
                    }
                    else if (direction == 2)
                    {
                        rb.velocity = Vector2.right * dashSpeed;
                    transform.localScale = new Vector2(.5f, transform.localScale.y);
                }
                }
            }
        
    }
}
