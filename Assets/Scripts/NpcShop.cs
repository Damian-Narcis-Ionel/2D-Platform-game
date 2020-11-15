using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NpcShop : MonoBehaviour
{
    public GameObject popUp;
    public TextMeshPro popUpText;
    public Canvas shop;
    // Start is called before the first frame update
    void Start()
    {
        popUp.SetActive(false);
        shop.gameObject.SetActive(false);   
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        popUp.SetActive(true);
        if (Input.GetKey(KeyCode.F))
        {
            shop.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        popUp.SetActive(false);
        shop.gameObject.SetActive(false);
        
    }
}
