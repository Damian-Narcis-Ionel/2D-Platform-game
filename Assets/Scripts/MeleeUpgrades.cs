using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class MeleeUpgrades : MonoBehaviour
{



    [System.Serializable]
    public class ShopItems
    {
        public int id;
        public string name;
        public int lvl;
        public int cost;
        public TextMeshProUGUI costText;
        public Image lvlDisplay;
        public Button addButton;
        public bool isMaxed = false;
    }
    public static int[] meleeLvls = { 0, 0, 0, 0, 0 };
    public ShopItems[] items;
    public Sprite[] lvlNumbers;

    // Start is called before the first frame update
    void Start()
    {
        Initialization();

    }

    // Update is called once per frame
    void Update()
    {
        ColorManagement();
        if (Input.GetKeyDown(KeyCode.L))
        {
            PlayerScript.playerCoins += 50;
        }
    }

    void Initialization()
    {

        //Da-i lvl din vectorul de lvl si pune-i imaginea corespunzatoare.
        for (int i = 0; i < items.Length; i++)
        {
            if (meleeLvls[i] >= 9)
            {
                items[i].isMaxed = true;
            }

            items[i].lvl = meleeLvls[items[i].id - 1];           
            items[i].lvlDisplay.sprite = lvlNumbers[items[i].lvl];


        }

        //Stabileste-i pretul la care ar trebui sa fie si seteaza textul.
        for (int i = 0; i < items.Length; i++)
        {
            items[i].cost = items[i].cost * (int)Math.Pow(2, meleeLvls[items[i].id - 1]);
            items[i].costText.text = items[i].cost.ToString();

        }
    }

    void ColorManagement()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (!items[i].isMaxed)
            {
                if (PlayerScript.playerCoins < items[i].cost || items[i].lvl >= 9)
                {
                    items[i].lvlDisplay.color = Color.red;
                    items[i].addButton.image.color = Color.red;
                }
                else
                {
                    items[i].lvlDisplay.color = Color.green;
                    items[i].addButton.image.color = Color.green;
                }
            }
            else
            {
                items[i].lvlDisplay.color = Color.black;
                items[i].addButton.image.color = Color.black;
            }

        }
    }


    public void addLevel(int id)
    {
        //In functie se primeste id-ul corespunzator puterii.
        //Identific id-ul si lucrez in itemul respectiv.
        for (int i = 0; i < items.Length; i++)
        {

            //Am gasit puterea corespunzatoare butonului
            if (Mathf.Abs(id - items[i].id) <= Mathf.Epsilon)
            {
                //Daca e deja maxat, nu il poate apasa
                if (items[i].isMaxed)
                {
                    items[i].addButton.enabled = false;
                }
                //Pot sa cumpar doar daca am destui bani
                if (PlayerScript.playerCoins >= items[i].cost)
                {
                    //plateste
                    PlayerScript.playerCoins -= items[i].cost;
                    //Maresc pret, cresc lvl, schimb imagine, schimb text
                    items[i].cost *= 2;
                    items[i].lvl++;
                    meleeLvls[id - 1]++;
                    items[i].lvlDisplay.sprite = lvlNumbers[items[i].lvl];
                    items[i].costText.text = items[i].cost.ToString();

                    //Daca e la lvl 9 este maxat
                    if (Mathf.Abs(items[i].lvl - 9) <= Mathf.Epsilon)
                    {
                        items[i].isMaxed = true;
                    }
                }


            }
        }

    }
}
