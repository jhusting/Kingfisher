using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField]
    private int speed;

    PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: Get the vertical speed from the player controller

        float verticalSpeed = 1f;
        transform.position += Vector3.up * Input.GetAxis("Vertical") * verticalSpeed * Time.deltaTime;


    }
}
