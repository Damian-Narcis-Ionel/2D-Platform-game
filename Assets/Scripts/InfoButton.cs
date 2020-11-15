using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoButton : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public Sprite[] sprites;
    public Image powerImage;
    public GameObject infoObject;

    // Start is called before the first frame update
    void Start()
    {
        infoObject.SetActive(false);
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openInfo(int id)
    {
        infoObject.SetActive(true);
        powerImage.sprite = sprites[id];
        powerImage.color = Color.white;
        switch (id)
        {
            //Dagger
            case 0:
                infoText.text = "Hitting an enemy with the dagger reduces their damage.";
                break;
            case 1:
                infoText.text = "Hitting an enemy with the dagger inflics poison that deals damage over time.";
                break;
            case 2:
                infoText.text = "Hitting an enemy with the dagger makes the target freeze.";
                break;
            case 3:
                infoText.text = "Killing an enemy with the dagger provides a short boost of movement speed.";
                powerImage.color = Color.black;
                break;
            case 4:
                infoText.text = "Killing an enemy with the dagger refills a small amount of health.";
                break;

            //Passives
            case 5:
                infoText.text = "You take reduced damage.";
                break;
            case 6:
                infoText.text = "The chanse of droping coins is increased.";
                break;
            case 7:
                powerImage.color = Color.red;
                infoText.text = "Falling under 35% of you hp makes you enraged, improving all your stats.";
                break;
            case 8:
                infoText.text = "After attacking and not dealing crit damage, you chanse of critical strike is increased until you crit again.";
                break;
                //Melee
            case 9:
                infoText.text = "Critical damage now deals more damage instead of 200%.";
                break;
            case 10:
                infoText.text = "After you critical strike an enemy with the melee attack, your next attack is 100% a crit.";
                break;
            case 11:
                infoText.text = "Hitting an enemy with the melee attack makes the target freeze.";
                
                break;
            case 12:
                infoText.text = "Killing an enemy with the melee attack provides a short boost of movement speed.";
                powerImage.color = Color.black;
                break;
            case 13:
                infoText.text = "Attack an enemy with the melee attack will apply corozive. Corozive will lower their defense.";
                break;
        }

        
    }
}
