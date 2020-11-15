using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlsInfo : MonoBehaviour
{
    public Canvas controlMenu;
    public Canvas pauseMenu = null;
    public GameObject player = null;

    // Start is called before the first frame update
    void Start()
    {
        
        controlMenu.enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ControlsMenuClose()
    {
        controlMenu.enabled = false;
        player.GetComponent<PlayerScript>().enabled = true;
        Time.timeScale = 1;
    }

}
