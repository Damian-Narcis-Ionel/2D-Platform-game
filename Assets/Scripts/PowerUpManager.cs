using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    //PlayerScrip reference//
    private PlayerScript playerScript;

    //===Power-Up amount===//
    public int hpAmount = 30;
    public int powerAmount = 5;
    public int speedAmount = 2;
    public float attackSpeedAmount = .2f;
    public int critAmount = 10;


    // Start is called before the first frame update
    void Start()
    {
        playerScript = GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.name == "HpPills(Clone)" || other.gameObject.name == "HpPills")
        {
            PlayerScript.currentHp += hpAmount;
            //playerScript.hpText.text= "HEALTH: " + playerScript.currentHp.ToString();
            playerScript.healthBar.SetHealth((int)PlayerScript.currentHp);
            
            Destroy(other.gameObject);
            
        }

        if (other.gameObject.name == "PowerPotion(Clone)" || other.gameObject.name == "PowerPotion")
        {
            playerScript.attackDamage += powerAmount;
            Destroy(other.gameObject);
        }
        
        if(other.gameObject.name == "SpeedPotion(Clone)" || other.gameObject.name == "SpeedPotion")
        {
            playerScript.moveSpeed += speedAmount;
            Destroy(other.gameObject);
        }

        if(other.gameObject.name == "AttackSpeedBuff(Clone)" || other.gameObject.name == "AttackSpeedBuff")
        {
            playerScript.attackRate -= attackSpeedAmount;
            Destroy(other.gameObject);
        }

        if(other.gameObject.name == "CritPills(Clone)" || other.gameObject.name== "CritPills")
        {
            playerScript.critChanse += critAmount;
            PlayerScript.notBuffedCrit += critAmount;
            Destroy(other.gameObject);
        }

    }
}
