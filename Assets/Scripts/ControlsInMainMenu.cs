using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsInMainMenu : MonoBehaviour
{
    public Canvas controlMenu;


    // Start is called before the first frame update
    void Start()
    {
        controlMenu.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MenuHandle()
    {
        if (controlMenu.enabled)
        {
            controlMenu.enabled = false;

        }
        else
        {
            controlMenu.enabled = true;
        }
    }
}
