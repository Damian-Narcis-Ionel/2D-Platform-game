using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
//using UnityEditor.Build.Reporting;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    //=============================//
    public Animator anim;
    public int maxHealth = 100;
    public float currentHealth;
    public int damage;
    float lockPos = 0;
    private Rigidbody2D body;
    public PlayerScript player;

    //=============================//

    //===[Variables for AI FOLLOW]===//
    //=============================//
    public float speed;
    private Transform target;
    private Transform rotation;
    public float stopDistance = 5;
    private int zAdjust = 0;

    //=============================//


    //===[Attack function variables]===//
    //=============================//
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public float attackDamage;
    public float attackRate = 2f;
    float nextAttackTime = 1.5f;
    private float distanceBetweenPlayer;

    //===[Variables for UI]===//
    //=============================//
    public HpBar healthBar;


    //===[State variables]===//
    //=============================//
    private bool isAttacking = false;
    private bool isDamaged;
    public float BONUS_GRAV = 12f;
    public bool isFatigued = false;

    //===[Variables for VFX]===//   
    //=============================//
    public GameObject blood;
    public GameObject crit;

    //===[Variables for coins drop]===//
    //=============================//
    public GameObject Coin;
    int coinRandomizer;
    int coinDropChanse = 35;
    int coinAmount = 1;



    private bool playerInvincible;


    //===[Variables for getting fatigued]===//
    //=============================//
    public GameObject fatigueIndicator;



    //===[Variables for getting poisoned]===//
    //=============================//
    public bool isPoisoned = false;
    public int poisonDamage = 0;
    public float poisonRate = 2f;
    public GameObject poisonIndicator;


    //===[Variables for getting froze]===//
    //=============================//
    public GameObject freezeIndicator;
    private bool isFrozed = false;


    //===[Variables for corozive]===
    public GameObject coroziveIndicator;
    private bool isCorozive;
    private float coroziveAmplificator = 1f;

    // Start is called before the first frame update
    void Start()
    {


        healthBar.SetMaxHealth((int)maxHealth);

        currentHealth = maxHealth;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rotation = GetComponent<Transform>();
        body = GetComponent<Rigidbody2D>();

        ManageCollisions();
        Indicators();


    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vel = body.velocity;
        vel.y -= BONUS_GRAV * Time.deltaTime;
        body.velocity = vel;


        if (!isDamaged)
        {
            Move();
            if (!isAttacking)
            {

                if (!PlayerScript.isInvulnerable && !isFrozed)
                {
                    Attack();

                }


            }
            RotationControl();
        }

    }

    public void TakeDamage(float damage)
    {
        isDamaged = true;
        currentHealth -= (damage*coroziveAmplificator);
        healthBar.SetHealth((int)currentHealth);
        anim.SetTrigger("Hurt");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        anim.SetTrigger("Dead");
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        spawnCoins();
        Destroy(this.gameObject, 3);
    }

    void Attack()
    {
        bool playerDamaged = target.GetComponent<PlayerScript>().isDamaged;


        if (distanceBetweenPlayer <= attackRange + 1f && isAttacking == false && isDamaged == false && playerInvincible == false)
        {


            if (Time.time >= nextAttackTime)
            {
                if (playerDamaged == false)
                {
                    isAttacking = true;
                    //Attack animation
                    anim.SetTrigger("Attack");

                    //Detect enemies in range
                    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
                    //Damage enemies
                    foreach (Collider2D enemy in hitEnemies)
                    {
                        if (enemy.gameObject.name.Equals("Player"))
                        {
                            enemy.GetComponent<PlayerScript>().takeDamage(attackDamage);
                        }


                    }

                    nextAttackTime = Time.time + attackRate;
                }


            }
        }


    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

    }

    void Move()
    {
        if (!isFrozed)
        {
            anim.SetFloat("Distance", distanceBetweenPlayer);
            //Debug.Log("Distance between players :" + distanceBetweenPlayer);
            //Debug.Log("Att distance: " + attackRange);
            //To lock the rotation
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, lockPos, lockPos);

            //Distance between player and enemy
            distanceBetweenPlayer = Vector2.Distance(transform.position, target.position);
        }

    }

    void RotationControl()
    {
        if (!isFrozed)
        {
            if (target.position.x < transform.position.x)
            {
                rotation.localScale = new Vector3(-1, 1, 1);
            }
            else
                rotation.localScale = new Vector3(1, 1, 1);
            if (Vector2.Distance(transform.position, target.position) > stopDistance)
            {
                var targetPos = new Vector2(target.position.x, transform.position.y);
                transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            }
        }

    }


    void isNotAttacking()
    {
        if (isAttacking)
        {
            isAttacking = false;
        }
    }

    void isNotDamaged()
    {

        if (isDamaged)
        {
            isDamaged = false;
        }

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        float damageToTakeRanged = target.GetComponent<PlayerScript>().rangedAttackDamage;
        if (other.gameObject.CompareTag("Knife"))
        {         
            if (KnifeUpgrades.knifeLvls[0] > 0 && !isFatigued)
            {
                StartCoroutine("GetFatigued");
            }
            if (KnifeUpgrades.knifeLvls[1] > 0 && !isPoisoned)
            {
                StartCoroutine("GetPoisoned");
            }
            if (KnifeUpgrades.knifeLvls[2] > 0 && !isFrozed)
            {
                StartCoroutine("GetFroze");
            }
            if (KnifeUpgrades.knifeLvls[3] > 0 && !PlayerScript.featherBoosted)
            {
                if (currentHealth - damageToTakeRanged <= 0)
                {
                    StartCoroutine("GetFeather");
                }

            }

            if(KnifeUpgrades.knifeLvls[4] > 0)
            {
                if(currentHealth - damageToTakeRanged <= 0)
                {
                    vampiric();
                }
            }

            TakeDamage(damageToTakeRanged);

        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zAdjust);
            zAdjust++;
        }
    }


    public void spawnBlood()
    {
        Instantiate(blood, new Vector2(transform.position.x, transform.position.y + 2f), quaternion.identity);
    }

    public void spawnCrit()
    {

        if (PlayerScript.critParticleSpawner < PlayerScript.playerCurrentCritChanse)
        {
            Instantiate(crit, new Vector2(transform.position.x, transform.position.y + 2f), quaternion.identity);
        }
    }

    void spawnCoins()
    {

        coinRandomizer = UnityEngine.Random.Range(1, 101);
        coinDropChanse = ApplyLuckPower(coinDropChanse);

        if (coinRandomizer <= coinDropChanse)
        {
            for (int i = 0; i < coinAmount; i++)
            {
                Instantiate(Coin, transform.position, quaternion.identity);
            }

        }

    }

    void Indicators()
    {
        fatigueIndicator.SetActive(false);
        poisonIndicator.SetActive(false);
        freezeIndicator.SetActive(false);
        coroziveIndicator.SetActive(false);
    }

    int ApplyLuckPower(int dropChanse)
    {
        switch (PassiveUpgrades.passiveLvls[1])
        {
            case 0:

                break;
            case 1:
                dropChanse += 5;
                break;
            case 2:
                dropChanse += 10;
                break;
            case 3:
                dropChanse += 13;
                break;
            case 4:
                dropChanse += 16;
                break;
            case 5:
                dropChanse += 20;
                break;
            case 6:
                dropChanse += 24;
                break;
            case 7:
                dropChanse += 30;
                break;
            case 8:
                dropChanse += 40;
                break;
            case 9:
                dropChanse += 55;
                break;

        }

        return dropChanse;
    }

    IEnumerator GetFatigued()
    {
        float fatigueDuration = 0;

        switch (KnifeUpgrades.knifeLvls[0])
        {
            case 0:
                fatigueDuration = 0;
                break;
            case 1:
                fatigueDuration = 3;
                break;
            case 2:
                fatigueDuration = 5;
                break;
            case 3:
                fatigueDuration = 7;
                break;
            case 4:
                fatigueDuration = 9;
                break;
            case 5:
                fatigueDuration = 11;
                break;
            case 6:
                fatigueDuration = 13;
                break;
            case 7:
                fatigueDuration = 15;
                break;
            case 8:
                fatigueDuration = 17;
                break;
            case 9:
                fatigueDuration = 20;
                break;

        }

        isFatigued = true;
        Fatigue();
        yield return new WaitForSeconds(fatigueDuration);
        isFatigued = false;
        ReturnFromFatigue();

    }


    void Fatigue()
    {
        if (isFatigued)
        {
            attackDamage = (float)(attackDamage - (0.25 * attackDamage));
            fatigueIndicator.SetActive(true);
        }
    }

    void ReturnFromFatigue()
    {
        if (!isFatigued)
        {

            attackDamage = (attackDamage * 100) / 75;
            fatigueIndicator.SetActive(false);
        }
    }

    void ManageCollisions()
    {
        Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), GameObject.Find("Enemy(Clone)").GetComponent<Collider2D>());
        Physics2D.IgnoreLayerCollision(9, 13, true);
        Physics2D.IgnoreLayerCollision(9, 15, true);
    }


    IEnumerator GetPoisoned()
    {

        float poisonDuration = 0;
        switch (KnifeUpgrades.knifeLvls[1])
        {
            case 0:
                poisonDuration = 0;
                poisonDamage = 0;
                break;
            case 1:
                poisonDuration = 3;
                poisonDamage = 1;
                break;
            case 2:
                poisonDuration = 3;
                poisonDamage = 2;
                break;
            case 3:
                poisonDuration = 3;
                poisonDamage = 3;
                break;
            case 4:
                poisonDuration = 4;
                poisonDamage = 4;
                break;
            case 5:
                poisonDuration = 5;
                poisonDamage = 4;
                break;
            case 6:
                poisonDuration = 5;
                poisonDamage = 5;
                break;
            case 7:
                poisonDuration = 5;
                poisonDamage = 6;
                break;
            case 8:
                poisonDuration = 5;
                poisonDamage = 8;
                break;
            case 9:
                poisonDuration = 5;
                poisonDamage = 10;
                break;
        }

        isPoisoned = true;
        poisonIndicator.SetActive(true);
        yield return new WaitForSeconds(1);
        for (int i = 0; i < poisonDuration; i++)
        {
            TakeDamage(poisonDamage);
            yield return new WaitForSeconds(1);
        }
        poisonIndicator.SetActive(false);
        isPoisoned = false;
    }

    IEnumerator GetFroze()
    {
        int frozeDuration = 0;
        switch (KnifeUpgrades.knifeLvls[2])
        {
            case 0:
                frozeDuration = 0;
                break;
            case 1:
                frozeDuration = 1;
                break;
            case 2:
                frozeDuration = 2;
                break;
            case 3:
                frozeDuration = 4;
                break;
            case 4:
                frozeDuration = 6;
                break;
            case 5:
                frozeDuration = 8;
                break;
            case 6:
                frozeDuration = 10;
                break;
            case 7:
                frozeDuration = 12;
                break;
            case 8:
                frozeDuration = 16;
                break;
            case 9:
                frozeDuration = 20;
                break;
        }

        isFrozed = true;
        body.velocity = Vector2.zero;
        freezeIndicator.SetActive(true);
        anim.enabled = false;
        yield return new WaitForSeconds(frozeDuration);
        anim.enabled = true;
        isFrozed = false;
        freezeIndicator.SetActive(false);

    }

    IEnumerator GetFeather()
    {


        float featherDuration = 0;
        float amount = 0f;
        switch (KnifeUpgrades.knifeLvls[3])
        {
            case 0:
                featherDuration = 0f;
                amount = 0f;
                break;
            case 1:
                featherDuration = 2f;
                amount = 2f;
                break;
            case 2:
                featherDuration = 3f;
                amount = 3f;
                break;
            case 3:
                featherDuration = 4f;
                amount = 5f;
                break;
            case 4:
                featherDuration = 5f;
                amount = 7f;
                break;
            case 5:
                featherDuration = 7f;
                amount = 9f;
                break;
            case 6:
                featherDuration = 9f;
                amount = 11f;
                break;
            case 7:
                featherDuration = 12f;
                amount = 13f;
                break;
            case 8:
                featherDuration = 15f;
                amount = 15f;
                break;
            case 9:
                featherDuration = 20f;
                amount = 17f;
                break;

        }


        target.GetComponent<PlayerScript>().moveSpeed += amount;
        PlayerScript.featherBoosted = true;

        yield return new WaitForSeconds(featherDuration);

        target.GetComponent<PlayerScript>().moveSpeed -= amount;
        PlayerScript.featherBoosted = false;

    }

    void vampiric()
    {
        float healAmount = 0f;

        switch (KnifeUpgrades.knifeLvls[4])
        {

            case 0:
                healAmount = 0;
                break;
            case 1:
                healAmount = 1f;
                break;
            case 2:
                healAmount = 3f;
                break;
            case 3:
                healAmount = 5f;
                break;
            case 4:
                healAmount = 7f;
                break;
            case 5:
                healAmount = 10f;
                break;
            case 6:
                healAmount = 13f;
                break;
            case 7:
                healAmount = 15f;
                break;
            case 8:
                healAmount = 17f;
                break;
            case 9:
                healAmount = 20f;
                break;
        }

        target.GetComponent<PlayerScript>().getHealed(healAmount);
    }


    public IEnumerator GetFrozeMelee()
    {
        Physics2D.IgnoreLayerCollision(9, 9, true);
        int frozeDuration = 0;
        switch (MeleeUpgrades.meleeLvls[2])
        {
            case 0:
                frozeDuration = 0;
                break;
            case 1:
                frozeDuration = 1;
                break;
            case 2:
                frozeDuration = 2;
                break;
            case 3:
                frozeDuration = 4;
                break;
            case 4:
                frozeDuration = 6;
                break;
            case 5:
                frozeDuration = 8;
                break;
            case 6:
                frozeDuration = 10;
                break;
            case 7:
                frozeDuration = 12;
                break;
            case 8:
                frozeDuration = 16;
                break;
            case 9:
                frozeDuration = 20;
                break;
        }

        isFrozed = true;
        body.velocity = Vector2.zero;
        freezeIndicator.SetActive(true);
        anim.enabled = false;

        yield return new WaitForSeconds(frozeDuration);

        anim.enabled = true;
        isFrozed = false;
        freezeIndicator.SetActive(false);

    }

    IEnumerator GetFeatherMelee()
    {


        float featherDuration = 0;
        float amount = 0f;
        switch (MeleeUpgrades.meleeLvls[3])
        {
            case 0:
                featherDuration = 0f;
                amount = 0f;
                break;
            case 1:
                featherDuration = 2f;
                amount = 2f;
                break;
            case 2:
                featherDuration = 3f;
                amount = 3f;
                break;
            case 3:
                featherDuration = 4f;
                amount = 5f;
                break;
            case 4:
                featherDuration = 5f;
                amount = 7f;
                break;
            case 5:
                featherDuration = 7f;
                amount = 9f;
                break;
            case 6:
                featherDuration = 9f;
                amount = 11f;
                break;
            case 7:
                featherDuration = 12f;
                amount = 13f;
                break;
            case 8:
                featherDuration = 15f;
                amount = 15f;
                break;
            case 9:
                featherDuration = 20f;
                amount = 17f;
                break;

        }


        target.GetComponent<PlayerScript>().moveSpeed += amount;
        PlayerScript.featherBoosted = true;

        yield return new WaitForSeconds(featherDuration);

        target.GetComponent<PlayerScript>().moveSpeed -= amount;
        PlayerScript.featherBoosted = false;

    }


    public IEnumerator GetCorozive()
    {
        float duration = 0f;

        switch (MeleeUpgrades.meleeLvls[4])
        {
            case 0:
                duration = 2f;
                coroziveAmplificator = 0.02f;
                break;
            case 1:
                duration = 2f;
                coroziveAmplificator = 0.05f;
                break;
            case 2:
                duration = 2f;
                coroziveAmplificator = 0.07f;
                break;
            case 3:
                duration = 2f;
                coroziveAmplificator = 0.09f;
                break;
            case 4:
                duration = 2f;
                coroziveAmplificator = 0.11f;
                break;
            case 5:
                duration = 2f;
                coroziveAmplificator = 0.13f;
                break;
            case 6:
                duration = 2f;
                coroziveAmplificator = 0f;
                break;
            case 7:
                duration = 3f;
                coroziveAmplificator = 0.15f;
                break;
            case 8:
                duration = 3f;
                coroziveAmplificator = 0.17f;
                break;
            case 9:
                duration = 3f;
                coroziveAmplificator = 0.20f;
                break;


        }


        isCorozive = true;
        coroziveIndicator.SetActive(true);
        yield return new WaitForSeconds(duration);
        coroziveIndicator.SetActive(false);
        coroziveAmplificator = 1f;
        
    }
}
