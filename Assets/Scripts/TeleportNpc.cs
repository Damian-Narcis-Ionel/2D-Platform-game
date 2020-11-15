using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TeleportNpc : MonoBehaviour
{
    public SpriteRenderer popUp;
    public TextMeshPro PopupText;
    public static bool spawnNpc = false;
    
    // Start is called before the first frame update
    void Start()
    {
        popUp.gameObject.SetActive(false);
        PopupText.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        popUp.gameObject.SetActive(true);
        PopupText.gameObject.SetActive(true);
        Scene activeScene = SceneManager.GetActiveScene();

        if (Input.GetKey(KeyCode.F))
        {
            if (activeScene.name.Equals("Level1"))
            {
                SceneManager.LoadScene("UpgradeTown");
            }
            if (activeScene.name.Equals("UpgradeTown"))
            {
                SceneManager.LoadScene("Level1");
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        popUp.gameObject.SetActive(false);
        PopupText.gameObject.SetActive(false);
    }


    
}
