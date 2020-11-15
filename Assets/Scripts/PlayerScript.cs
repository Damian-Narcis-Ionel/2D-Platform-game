using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication.ExtendedProtection;
using TMPro;
using Unity.Mathematics;
//using System.IO.Ports;
//using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class PlayerScript : MonoBehaviour
{
    #region Component Accesors
    //===[Components accesors]===
    protected Rigidbody2D rb;
    private Animator anim;
    private Collider2D collider;
    [SerializeField] GameObject GroundCheckerComponent;
    #endregion

    #region Ui Control
    //===[Variables for UI control]===//  
    public HpBar healthBar;
    public Canvas deadMenu;
    public TextMeshProUGUI coinText;
    #endregion

    #region Movement
    //===[Variables for movement]===//
    public float moveSpeed = 5f;
    public float jumpHigh;
    #endregion

    #region Attack Variables
    //===[Variables for Attack]===//
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public float attackDamage = 5;
    public float attackRate = 2f;
    float nextAttackTime = 1.5f;
    #endregion

    #region State Variables
    //===[State variables]===//
    public bool isDamaged;
    public bool isGrounded = true;
    public bool isAttacking = false;
    public bool isDead = false;
    public bool isRangedAttacking = false;
    public bool isDashing = false;
    public float BONUS_GRAV = 12f;
    public static bool isInvulnerable = false;
    public bool nearNPC;
    public bool isEnraged;
    public static bool usedEnrage = false;
    public static bool hpWasInitialized = false;
    public static bool featherBoosted = false;
    #endregion

    #region Damaged Variables
    //===[Variables for getting damaged]===//
    public float maxHp = 100;
    public static float currentHp = 100;
    #endregion

    #region Ranged Attack Variables
    //===[Variables for ranged Attack]===//
    public Transform knifeSpawnPoint;
    public float rangedAttackDamage = 3;
    public float rangedAttackRate = 1f;
    float nextRangedAttackTime = 1.5f;
    public GameObject knifeRight;
    public GameObject knifeLeft;
    #endregion

    #region Dash Variables
    //===[Variables for dash ability]===//
    public float dashSpeed;
    private float dashTime;
    public float startDashTime;
    private int direction;
    private float nextDashTime = 1.5f;
    public float dashRate = 2f;
    private Vector3 startPosition;
    private Vector3 endPosition;
    #endregion

    #region VFX
    //===[Variables for VFX]===//
    public GameObject blood;
    #endregion

    #region Crit
    //===[Variables for Critical damage]===//
    int random = 0;
    public int critChanse = 5;
    public static int critParticleSpawner;
    public static int critScaler = 0;
    public static int playerCurrentCritChanse;
    private bool wasCrit = false;
    public static int notBuffedCrit = 0;
    public static bool hasToHeadAim = false;
    #endregion

    #region
    public SpriteRenderer invincibilitySign;
    #endregion

    #region Static Variables
    public static int playerCoins = 0;
    private static float[] savedStats = new float[5];
    #endregion

    void Start()
    {


        Physics2D.IgnoreLayerCollision(8, 15, true);
        notBuffedCrit = critChanse;
        #region Components
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
        #endregion

        #region Randoms
        dashTime = startDashTime;
        invincibilitySign.enabled = false;
        healthBar.SetMaxHealth((int)maxHp);
        healthBar.SetHealth((int)currentHp);
        //hpWasInitialized = true;
        deadMenu.enabled = false;
        coinText.text = playerCoins.ToString();
        #endregion


        savedStats[0] = attackDamage;
        savedStats[1] = rangedAttackDamage;
        savedStats[2] = moveSpeed;
        savedStats[3] = attackRate;
        savedStats[4] = rangedAttackRate;

        
    }

    void Update()
    {
        playerCurrentCritChanse = critChanse;

        //===Gravity==//
        Vector3 vel = rb.velocity;
        vel.y -= BONUS_GRAV * Time.deltaTime;
        rb.velocity = vel;

        if (!isDead)
        {

            Move();
            Jump();

            if (isGrounded)
            {
                Attack();
                rangedAttack();
            }

        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("hp: " + currentHp);
        }

    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            Dash();
        }
    }

    public void Move()
    {


        //Locking the rotation       
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 0f, 0f);

        //===[Movement]===//
        float hDirection = Input.GetAxisRaw("Horizontal");
        anim.SetFloat("Speed", Mathf.Abs(hDirection));

        //===[Move in game]===//

        transform.Translate((transform.right * Input.GetAxisRaw("Horizontal")).normalized * moveSpeed * Time.deltaTime);




        //===[Rotate left]===//
        if (hDirection < 0)
        {

            rb.transform.localScale = new Vector2(-.5f, .5f);

        }

        ////===[Rotate right]===//
        if (hDirection > 0)
        {

            rb.transform.localScale = new Vector2(.5f, .5f);


        }



    }

    public void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && !isAttacking)
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpHigh), ForceMode2D.Impulse);
        }
    }

    public void Attack()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (Time.time >= nextAttackTime)
            {
                if (isInvulnerable)
                {
                    Physics2D.IgnoreLayerCollision(8, 9, false);
                    isInvulnerable = false;
                    invincibilitySign.enabled = false;
                }
                isAttacking = true;
                anim.SetFloat("Speed", Mathf.Abs(Input.GetAxisRaw("Horizontal")));
                //Attack animation
                anim.SetTrigger("Attack");
                //Detect enemies in range
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
                //Damage enemies

                foreach (Collider2D enemy in hitEnemies)
                {
                    if (crititalStrike())
                    {
                        enemy.GetComponent<Enemy>().TakeDamage(EstablishCritDamage(attackDamage));
                        enemy.GetComponent<Enemy>().StartCoroutine("GetCorozive");
                        enemy.GetComponent<Enemy>().StartCoroutine("GetFrozeMelee");


                        if (MeleeUpgrades.meleeLvls[3] > 0 && !PlayerScript.featherBoosted)
                        {
                            if (enemy.GetComponent<Enemy>().currentHealth - rangedAttackDamage <= 0)
                            {
                                StartCoroutine("GetFeather");
                            }

                        }

                    }
                    else
                    {
                        enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
                        enemy.GetComponent<Enemy>().StartCoroutine("GetFrozeMelee");

                        if (MeleeUpgrades.meleeLvls[3] > 0 && !PlayerScript.featherBoosted)
                        {
                            if (enemy.GetComponent<Enemy>().currentHealth - rangedAttackDamage <= 0)
                            {
                                StartCoroutine("GetFeather");
                            }

                        }
                    }

                }

                nextAttackTime = Time.time + attackRate;
            }

        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.CompareTag("Enemy"))
        {
            takeDamage(5);
            if (currentHp > 0)
            {
                StartCoroutine("GetInvulnerable");
            }
            rb.velocity = Vector2.zero;
        }
        if (other.gameObject.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            playerCoins++;
            UpdatePlayerCoinsText();
        }

    }

    private void OnDrawGizmos()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void isNotDamaged()
    {

        if (isDamaged)
        {
            isDamaged = false;
        }

    }

    void isNotAttacking()
    {
        if (isAttacking)
        {
            isAttacking = false;
        }
        if (isRangedAttacking)
        {
            isRangedAttacking = false;
        }
    }

    public void takeDamage(float damageToTake)
    {

        if (!isDamaged)
        {
            isDamaged = true;

            hpHandle(ApplyShieldPower(damageToTake));
            anim.SetTrigger("Hit");


            if (currentHp <= 0)
            {
                Die();
            }
        }
        Debug.Log("Hp: " + currentHp);
    }

    void Die()
    {

        if (!isDead)
        {

            isDead = true;
            anim.SetBool("isDead", true);
        }
        anim.SetTrigger("Dead");
        collider.enabled = false;


        GetComponent<Rigidbody2D>().isKinematic = true;
        rb.bodyType = RigidbodyType2D.Static;
        Destroy(GroundCheckerComponent);

        deadMenu.enabled = true;
    }

    void rangedAttack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (Time.time >= nextRangedAttackTime)
            {
                if (isInvulnerable)
                {
                    Physics2D.IgnoreLayerCollision(8, 9, false);
                    isInvulnerable = false;
                    invincibilitySign.enabled = false;
                }
                isAttacking = true;
                isRangedAttacking = true;
                //Animation
                anim.SetTrigger("RangeAttack");
                nextRangedAttackTime = Time.time + rangedAttackRate;
            }
        }
    }

    void spawnKnife()
    {
        if (transform.localScale.x > 0f)
        {
            Instantiate(knifeRight, knifeSpawnPoint.position, Quaternion.Euler(0f, 0f, 160f));
        }
        else
        {

            Instantiate(knifeLeft, knifeSpawnPoint.position, Quaternion.Euler(0f, 0f, 198.5f));

        }

    }

    public void hpHandle(float damageToTake)
    {

        currentHp -= damageToTake;
        healthBar.SetHealth((int)currentHp);

        if (currentHp < maxHp * 0.35)
        {
            if (!usedEnrage)
            {
                StartCoroutine("GetEnraged");
                usedEnrage = true;
            }


        }

        // hpText.text = "HEALTH: " + currentHp.ToString();
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
                Physics2D.IgnoreLayerCollision(8, 9, false);
            }
            else
            {

                dashTime -= Time.deltaTime;
                if (Time.time >= nextDashTime)
                {

                    if (direction == 1)
                    {

                        isDashing = true;
                        //anim.SetTrigger("Dash");
                        anim.SetBool("isDashing", true);

                        isInvulnerable = true;
                        Physics2D.IgnoreLayerCollision(8, 9, true);

                        //===========================//

                        //===========================//

                        rb.velocity = Vector2.left * dashSpeed;
                        transform.localScale = new Vector2(-.5f, transform.localScale.y);

                    }
                    else if (direction == 2)
                    {

                        isDashing = true;
                        //anim.SetTrigger("Dash");
                        anim.SetBool("isDashing", true);

                        isInvulnerable = true;
                        Physics2D.IgnoreLayerCollision(8, 9, true);

                        rb.velocity = Vector2.right * dashSpeed;
                        transform.localScale = new Vector2(.5f, transform.localScale.y);


                    }

                    nextDashTime = Time.time + dashRate;
                }


            }
        }

    }

    public void isNotDashing()
    {
        if (isDashing)
        {
            anim.SetBool("isDashing", false);
            isDashing = false;

        }
    }

    public void spawnBlood()
    {

        Instantiate(blood, new Vector3(transform.position.x, transform.position.y + 3f, 1f), quaternion.identity);
    }

    public bool crititalStrike()
    {

        random = Random.Range(1, 101);
        critParticleSpawner = random;

        if (hasToHeadAim)
        {
            wasCrit = true;
            hasToHeadAim = false;
            return true;
        }

        if (!wasCrit)
        {

            critScaler = 5 * PassiveUpgrades.passiveLvls[3];
            critChanse += critScaler;
        }
        else
        {
            critScaler = 0;
            critChanse = notBuffedCrit;
            hasToHeadAim = true;
        }

        if (random <= critChanse)
        {
            wasCrit = true;
            return true;
        }
        else
        {
            wasCrit = false;
            return false;
        }

    }

    float ApplyShieldPower(float damageToTake)
    {

        switch (PassiveUpgrades.passiveLvls[0])
        {
            case 0:
                break;
            case 1:
                damageToTake = damageToTake - (damageToTake * 0.05f);
                break;
            case 2:
                damageToTake = damageToTake - (damageToTake * 0.10f);
                break;
            case 3:
                damageToTake = damageToTake - (damageToTake * 0.15f);
                break;
            case 4:
                damageToTake = damageToTake - (damageToTake * 0.20f);
                break;
            case 5:
                damageToTake = damageToTake - (damageToTake * 0.25f);
                break;
            case 6:
                damageToTake = damageToTake - (damageToTake * 0.30f);
                break;
            case 7:
                damageToTake = damageToTake - (damageToTake * 0.35f);
                break;
            case 8:
                damageToTake = damageToTake - (damageToTake * 0.40f);
                break;
            case 9:
                damageToTake = damageToTake - (damageToTake * 0.43f);
                break;

        }

        return damageToTake;
    }

    IEnumerator GetEnraged()
    {
        float enrageDuration = 0;
        switch (PassiveUpgrades.passiveLvls[2])
        {
            case 0:
                enrageDuration = 0f;
                break;
            case 1:
                enrageDuration = 5f;
                break;
            case 2:
                enrageDuration = 10f;
                break;
            case 3:
                enrageDuration = 13f;
                break;
            case 4:
                enrageDuration = 16f;
                break;
            case 5:
                enrageDuration = 20f;
                break;
            case 6:
                enrageDuration = 30f;
                break;
            case 7:
                enrageDuration = 35f;
                break;
            case 8:
                enrageDuration = 40f;
                break;
            case 9:
                enrageDuration = 60f;
                break;

        }
        isEnraged = true;
        Enraged();
        yield return new WaitForSeconds(enrageDuration);
        ReturnFromEnrage();
        isEnraged = false;
    }

    void Enraged()
    {

        if (isEnraged && !usedEnrage)
        {
            invincibilitySign.color = Color.red;
            attackDamage = attackDamage + (attackDamage * 0.5f);
            rangedAttackDamage = (int)(rangedAttackDamage + (rangedAttackDamage * 0.5f));
            moveSpeed = moveSpeed + (moveSpeed * 0.5f);
            attackRate = attackRate - (attackRate * 0.5f);
            critChanse = (int)(critChanse + (critChanse * 0.5f));
            rangedAttackRate = rangedAttackRate - (rangedAttackRate * 0.5f);
        }
    }

    void ReturnFromEnrage()
    {
        invincibilitySign.color = Color.white;
        attackDamage = savedStats[0];
        rangedAttackDamage = savedStats[1];
        moveSpeed = savedStats[2];
        attackRate = savedStats[3];
        rangedAttackRate = savedStats[4];
    }

    IEnumerator GetInvulnerable()
    {
        Physics2D.IgnoreLayerCollision(8, 9, true);
        invincibilitySign.enabled = true;
        isInvulnerable = true;

        yield return new WaitForSeconds(1.5f);
        Physics2D.IgnoreLayerCollision(8, 9, false);
        invincibilitySign.enabled = false;
        isInvulnerable = false;

    }

    #region UI FUNCTIONS
    public void OpenDeadMenu()
    {
        deadMenu.enabled = true;
        Time.timeScale = 0;
    }

    public void RestartDead()
    {
        currentHp = 100;
        SceneManager.LoadScene(SceneManager.GetActiveScene().ToString());

    }

    public void QuitDead()
    {
        Application.Quit();
    }

    public void BackToMenuDead()
    {
        SceneManager.LoadScene("Main menu");
    }

    public void UpdatePlayerCoinsText()
    {
        coinText.text = playerCoins.ToString();
    }

    #endregion

    public void getHealed(float healToTake)
    {
        hpHandle(-healToTake);
        Debug.Log("Hp: " + currentHp);
    }

    //Strenght
    float EstablishCritDamage(float damage)
    {
        float value = damage;

        switch (MeleeUpgrades.meleeLvls[0])
        {
            case 0:
                return damage*2f;
            case 1:
                return damage * 2.10f;
            case 2:
                return damage * 2.15f;
            case 3:
                return damage * 2.20f;
            case 4:
                return damage * 2.40f;
            case 5:
                return damage * 2.60f;
            case 6:
                return damage * 2.80f;
            case 7:
                return damage * 3f;
            case 8:
                return damage * 3.25f;
            case 9:
                return damage * 3.5f;
                
        }
        return damage * 2;
       
    }

    void headAim()
    {
        //implemented in the crit function code;
    }

    
}
