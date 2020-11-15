using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    
    public Text waveText;
    //public Text enemyInfo;
    public Transform spawnPoint;


    #region Power Ups
    //===Power ups===//
    public Transform powerUpLocation;
    public GameObject hpPills;
    public GameObject powerPotion;
    public GameObject speedPotion;
    public GameObject attackSpeedBuff;
    public GameObject critPills;
    public bool powerSpawner;
    #endregion




    public enum SpawnState { SPAWNING, WAITING, COUNTING };

    [System.Serializable]
    public class Wave
    {
        
        public string name;
        public Transform enemy;
        public int count;
        public float rate;
        public float enemyDamage;
        public float enemyHp;
        public float difficuly;
        public float enemySpeed;
    }
    public Wave[] waves;
    private int nextWave = 0;
    public float timeBetweenWaves = 5f;
    public float waveCountDown = 0f;
    private SpawnState state = SpawnState.COUNTING;
    

    private float searchCountDown = 1f;

    // Start is called before the first frame update
    void Start()
    {
        waveText.text = "PREPARING UwU";
        waveCountDown = timeBetweenWaves;

        hpPills.layer= 12;
        powerPotion.layer = 12;
        speedPotion.layer = 12;
        attackSpeedBuff.layer = 12;
        critPills.layer = 12;
        Physics2D.IgnoreLayerCollision(12, 9, true);
        

    }

    // Update is called once per frame
    void Update()
    {

        if (state == SpawnState.WAITING)
        {
            //Check if enemies are still alive so it doesn't go to the next wave
            if (!EnemyIsAlive())
            {
                //begin a New round               
                WaveCompleted();
                return;
            }
            else
            {
                return;
            }
        }

        if(waveCountDown <= 0)
        {
            //===[if it is not already spawning]===//
            if(state != SpawnState.SPAWNING)
            {
                //Start spawning waves 
                StartCoroutine(SpawnWave(waves[nextWave]));
            }
        }
        else
        {
            waveCountDown -= Time.deltaTime;
        }

    }

    void WaveCompleted()
    {
        TeleportNpc.spawnNpc = true;
        if (powerSpawner) { 
            //===Spawning power ups===//
            int randomizer = Random.Range(1, 6);
            if( isEqual(randomizer,1))
            {
                Instantiate(hpPills, new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, 0f), Quaternion.identity);
            }
            else if (isEqual(randomizer, 2))
            {
                Instantiate(powerPotion, new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, 0f), Quaternion.identity);
            }
            else if (isEqual(randomizer, 3))
            {
                Instantiate(speedPotion, new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, 0f), Quaternion.identity);
            }
            else if (isEqual(randomizer, 4))
            {
                Instantiate(attackSpeedBuff, new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, 0f), Quaternion.identity);
            }
            else if (isEqual(randomizer,5))
            {
                Instantiate(critPills, new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, 0f), Quaternion.identity);
            }
            
        }       

        state = SpawnState.COUNTING;
        waveCountDown = timeBetweenWaves;

        if(nextWave +1 > waves.Length - 1)
        {
            //Here add dificulty scales//
            nextWave = 0;
           // Debug.Log("Completed all waves. Looping...");
        }
        else
        {
            nextWave++;
        }
            
    }

    bool EnemyIsAlive()
    {
        searchCountDown -= Time.deltaTime;
        if(searchCountDown <= 0f)
        {
            searchCountDown = 1f;
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
            
        }
        return true;
    }

    IEnumerator SpawnWave(Wave _wave)
    {                      

        waveText.text = _wave.name;
        state = SpawnState.SPAWNING;

        //Spawn
        for(int i = 0;i< _wave.count; i++)
        {
            SpawnEnemy(_wave);
            yield return new WaitForSeconds(1f / _wave.rate);
        }

        state = SpawnState.WAITING;
        yield break;
    }

    void SpawnEnemy(Wave _wave)
    {
        Enemy enemyScript;
        enemyScript = _wave.enemy.GetComponent<Enemy>();
        enemyScript.attackDamage =(int)(_wave.difficuly * _wave.enemyDamage);
        enemyScript.maxHealth =(int) (_wave.difficuly * _wave.enemyHp);
        enemyScript.speed = (int)(_wave.difficuly * _wave.enemySpeed);

        //Spawn enemy
        //Debug.Log("Spawning Enemy: " + _enemy.name);
        Instantiate(_wave.enemy, new Vector2(transform.position.x,transform.position.y-6), transform.rotation);
        
    }

    bool isEqual(float a, float b)
    {
        if (a >= b - Mathf.Epsilon && a <= b + Mathf.Epsilon)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
