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

    LineRenderer lr;

    public GameObject endOfGun;
    public GameObject spearEnd;

    void Awake()
    {

        FishCaughtEvent = new GameObjectEvent();
    }

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        rb = GetComponent<Rigidbody2D>();

        lr = GetComponent<LineRenderer>();
    }

    private void Update()
    {

        if(playerController.spearReturned){
            lr.enabled = false;
        }else
        {
            lr.enabled = true;
        }
        //lr.SetPosition(2, transform.InverseTransformVector(endOfGun.transform.position));
        lr.SetPosition(0, transform.GetChild(0).transform.position - Vector3.forward);
        lr.SetPosition(1, endOfGun.transform.position - Vector3.forward);

        if(!playerController.spearReturned)
        {
            float speed = 2f * playerController.moveSpeed;
            transform.position = transform.position + Vector3.left * speed * Time.deltaTime;
        }

        if (rb.velocity.magnitude > 1f && !spawningBubbles)
        {
            float spawnRate = Random.Range(0.05f, 0.2f);
            StartCoroutine(SpawnBubbles(spawnRate));
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

            UIController uic = FindObjectOfType<UIController>();

            //Set the collision to disabled to prevent it from knocking other fish around
            col.enabled = false;
            fishHit.SetAlive(false);
            FishCaughtEvent.Invoke(fishHit.gameObject);

            SoundController sc = playerController.GetComponent<SoundController>();

            sc.Play("bubble");
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
