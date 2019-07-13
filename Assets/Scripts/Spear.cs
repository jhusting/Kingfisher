﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameObjectEvent : UnityEvent<GameObject> { }


public class Spear : MonoBehaviour
{
    public PlayerController playerController;

    public GameObjectEvent FishCaughtEvent;

    void Awake()
    {

        FishCaughtEvent = new GameObjectEvent();
    }

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

    }

    private void Update()
    {
        if(!playerController.spearReturned)
        {
            float speed = 2f * playerController.moveSpeed;
            transform.position = transform.position + Vector3.left * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Fish fishHit = col.gameObject.GetComponent<Fish>();

        if (fishHit)
        {
            Quaternion rot = fishHit.transform.rotation;
            fishHit.transform.SetParent(transform, true);
            fishHit.transform.rotation = rot;

            //Set the collision to disabled to prevent it from knocking other fish around
            col.enabled = false;

            FishCaughtEvent.Invoke(fishHit.gameObject);
        }
    }
}