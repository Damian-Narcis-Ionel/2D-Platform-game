using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{

    public Canvas pauseMenu;
    public GameObject player;
    public Canvas infoMenu;


    public void Start()
    {

        pauseMenu.enabled = false;
    }

    public void Play()
    {
        SceneManager.LoadScene("Level1");
    }


    public void Quit()
    {
        Application.Quit();
    }

    public void Resume()
    {
        player.GetComponent<PlayerScript>().enabled = true;
        Time.timeScale = 1;
        pauseMenu.enabled = false;
        
    }

    public void Restart()
    {
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        PlayerScript.currentHp = 100;
        Time.timeScale = 1;
        
    }

    public void InfoButton()
    {
        pauseMenu.enabled = false;
        infoMenu.enabled = true;
        
    } 
}
