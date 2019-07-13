using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{

    public int value = 1;
    public string fishTag = "";


    public float fishBaseSpeed = 2f;
    public float fishSpeedVariance = 0.5f;

    public float sizeVariance = 0.25f;

    public bool alive = true;

    //Speed of the fish after variance is taken into account. Does not account for world speed
    public float fishFinalSpeed { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        fishFinalSpeed = fishBaseSpeed + Random.Range(-1 * fishSpeedVariance, fishSpeedVariance);

        transform.localScale *= 1 + Random.Range(-1 * sizeVariance, sizeVariance);
    }

    // Update is called once per frame
    void Update()
    {
        if (alive)
        {
            transform.localPosition += Vector3.right * -1 * fishFinalSpeed * Time.deltaTime;
        }
    }
}
