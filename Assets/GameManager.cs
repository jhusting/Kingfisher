using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum GameFinishedStatus
{
    Continue,
    Restart,
    Loss
}

public class GameFinishedEvent : UnityEvent<GameFinishedStatus>{}

public class GameManager : MonoBehaviour
{
    public GameManager gameManager { get; private set;}

    public PlayerController playerController { get; private set; }

    public Canvas gameHUD;
    public Canvas menuHUD;
    public GameObject gameOverScreen;
    public GameObject gameLostScreen;

    public Vector3 cameraInitialPos;

    public GameFinishedEvent gameFinished;

    public Text gameLostText;

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

        gameFinished = new GameFinishedEvent();
        
    }
    

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

        FindObjectOfType<Spear>().FishCaughtEvent.AddListener(OnFishCaught);
        cameraInitialPos = Camera.main.transform.position;

        playerController.runFailed.AddListener(OnRunFailed);
    }

    void OnRunFailed(RunFailedStatus status)
    {

        switch (status)
        {
            case RunFailedStatus.HeldBreath:
                gameLostText.text = "You ran out of breath! Don't forget to breath!";
                break;
            case RunFailedStatus.NoOxygen:
                gameLostText.text = "Oh no! You ran out of oxygen!";
                break;

        }


        gameLostScreen.SetActive(true);
        playerController.FinishRun();

    }

    public void QuitGame()
    {
        Application.Quit();
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

        Camera.main.transform.position = cameraInitialPos;


        gameLostScreen.SetActive(false);
        
    }

    public void OnFishCaught(GameObject fish)
    {
        if (fish.GetComponent<Fish>().fishTag == "king")
        {
            //Player has won
            gameOverScreen.gameObject.SetActive(true);
            playerController.FinishRun();
            FindObjectOfType<FishSpawner>().RemoveKingFish();
        }
    }


    public void OnPlayerContinue()
    {
        gameFinished.Invoke(GameFinishedStatus.Continue);
        gameOverScreen.SetActive(false);
        BackToMenu();
    }



    public void OnPlayerRestart()
    {
        gameOverScreen.SetActive(false);
        gameFinished.Invoke(GameFinishedStatus.Restart);
        
    }

    public void OpenURL(string s)
    {
        Application.OpenURL(s);
    }
}
