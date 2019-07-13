using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameObjectEvent : UnityEvent<GameObject> { }


public class Spear : MonoBehaviour
{
    public PlayerController playerController;

    public GameObjectEvent FishCaughtEvent;

    public ParticleSystem bubblePrefab;

    private Rigidbody2D rb;
    private bool spawningBubbles = false;

    void Awake()
    {

        FishCaughtEvent = new GameObjectEvent();
    }

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(!playerController.spearReturned)
        {
            float speed = 2f * playerController.moveSpeed;
            transform.position = transform.position + Vector3.left * speed * Time.deltaTime;
        }

        if (rb.velocity.magnitude > 1f && !spawningBubbles)
        {
            float spawnRate = Random.Range(0.03f, 0.07f);
            //StartCoroutine(SpawnBubbles(spawnRate));
        }
        else if (spawningBubbles && rb.velocity.magnitude <= 1f)
        {
            StopAllCoroutines();
            spawningBubbles = false;
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

    IEnumerator SpawnBubbles(float waitTime)
    {
        World w = FindObjectOfType<World>();
        spawningBubbles = true;

        while (true)
        {
            ParticleSystem ps = Instantiate(bubblePrefab, transform.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f)), w.GetNewestTile().transform) as ParticleSystem;
            yield return new WaitForSeconds(waitTime);
        }
    }
}
