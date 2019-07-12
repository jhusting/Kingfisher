using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameManager gameManager { get; private set;}

    public PlayerController playerController { get; private set; }

    public Canvas gameHUD;
    public Canvas menuHUD;

    void Awake()
    {
        //Ensure this is a singleton
        if(gameManager != null)
        {
            Destroy(gameObject);
        }
        else
        {
            gameManager = this;
        }
    }

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    public void StartDive()
    {

        playerController.StartCoroutine(playerController.DiveIn(2f));
        menuHUD.gameObject.SetActive(false);
        gameHUD.gameObject.SetActive(true);

    }

    public void BackToMenu()
    {
        menuHUD.gameObject.SetActive(true);
        gameHUD.gameObject.SetActive(false);
    }
}
