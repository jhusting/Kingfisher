using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    public PlayerController playerController;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Fish fishHit = col.gameObject.GetComponent<Fish>();

        if (fishHit)
        {
            fishHit.transform.SetParent(transform);

            //Set the collision to disabled to prevent it from knocking other fish around
            col.enabled = false;

            playerController.AddCurrentValue(fishHit.value);
        }
    }
}
