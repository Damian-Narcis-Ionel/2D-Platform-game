using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadMenu : MonoBehaviour
{
    public Canvas deadMenu;

    // Start is called before the first frame update
    void Start()
    {
        deadMenu.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region UI FUNCTIONS
    public void OpenDeadMenu()
    {
        deadMenu.enabled = true;
        Time.timeScale = 0;
    }
    public void RestartDead()
    {
        PlayerScript.currentHp = 100;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitDead()
    {
        Application.Quit();
    }

    public void BackToMenuDead()
    {
        SceneManager.LoadScene("Main menu");
    }

    #endregion
}
