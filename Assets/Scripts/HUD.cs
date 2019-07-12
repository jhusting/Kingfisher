using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Text currentValueText;
    public Text currentDistanceText;

    private PlayerController playerController;
    private World world;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        world = FindObjectOfType<World>();
    }

    // Update is called once per frame
    void Update()
    {
        //Update the current value text
        currentValueText.text = "Current value: $" + playerController.currentValue.ToString();
        currentDistanceText.text = world.distanceTravelled.ToString() + "m";
    }
}
