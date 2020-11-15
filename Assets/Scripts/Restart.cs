using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public GameObject player;
    public Canvas pauseMenu;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.enabled = false;   
    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Here we bring the time back
            if(Time.timeScale < 1)
            {
                Time.timeScale = 1;
                player.GetComponent<PlayerScript>().enabled = true;
                pauseMenu.enabled = false;
            }
            //Here we freeze the time
            else
            {
                Time.timeScale = 0;
                player.GetComponent<PlayerScript>().enabled = false;
                pauseMenu.enabled = true;
                
                
            }
            

        }
    }
}
