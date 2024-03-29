﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // The refence to the spear
    public GameObject spear;
    public GameObject gun;
    public LineRenderer rope;
    public GameObject spearEnd;
    public ParticleSystem bubblePrefab;
    public ParticleSystem shotBubblesPrefab;

    public bool underWater = false;

    public float moveSpeed = 1f;

    public float spearMaxDistance = 5f;
    public float spearStrength = 0f;
    

    private Camera cam;
    [SerializeField]
    private float minSize = 4.5f;
    [SerializeField]
    private float maxSize = 5.5f;
    [SerializeField]
    private float targetSize = 5f;

    [SerializeField]
    private float baseMoveSpeed = 1f;
    [SerializeField]
    private float burstMovementModifier = 2f;

    [SerializeField]
    private float maxOxygen = 10f;
    [SerializeField]
    private float oxygenBurnRate = 1f;
    private float currOxygen = 0f;

    [SerializeField]
    private AnimationCurve burstCurve;
    [SerializeField]
    private AnimationCurve camZoomCurve;
    [SerializeField]
    private AnimationCurve camShakeCurve;

    private bool spearShot = false;
    public bool spearReturned = true;
    private bool reeling = false;

    Coroutine swimUpRoutine;

    [SerializeField]
    private AnimationCurve diveCurveX;
    [SerializeField]
    private AnimationCurve diveCurveY;[SerializeField]
    public AnimationCurve diveRotationZ;
    public AnimationCurve swimUpRotationZ;

    public RunFailedEvent runFailed { get; private set; }

    //Amount of cash the player has available
    public int cash { get; private set; }
    //Amount of cash the player has gotten on the current attempt
    public int currentValue { get; private set; }

    PlayerCharacter playerCharacter;

    private SoundController soundController;
    private Animator animator;


    public static PlayerController playerController { get; private set; }

    private void Awake()
    {
        //Singleton
        if(playerController != null)
        {
            Destroy(gameObject);
        }else
        {
            playerController = this;
        }

        runFailed = new RunFailedEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
        currOxygen = maxOxygen;
        cam = Camera.main;

        soundController = GetComponent<SoundController>();

        spear.GetComponent<Spear>().FishCaughtEvent.AddListener(OnFishCaught);
        playerCharacter = FindObjectOfType<PlayerCharacter>();
        animator = playerCharacter.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Dev cheats
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddCash(10000);
        }

        animator.speed = Mathf.Clamp(moveSpeed * 2f, 0f, 1.2f);

        if (underWater)
        {
            RotateToMouse(gun);

            if (spearReturned && !spearShot)
                RotateToMouse(spear);
            else
            {
                Vector3[] pos = new Vector3[2];
                pos[0] = Vector3.zero;
                pos[1] = spearEnd.transform.position - rope.transform.position;
                rope.SetPositions(pos);
            }

            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, 0.8f * Time.deltaTime);

            if (currOxygen <= 0f)
                runFailed.Invoke(RunFailedStatus.NoOxygen);
        }

        ReadInput();
    }

    void RotateToMouse(GameObject g)
    {
        Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - g.transform.position;
        diff.Normalize();

        // Gets the angle from the mouse position
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        g.transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
        //currWind.Size = (currWind.StartPos - mouseLocation).magnitude + .4f;
    }

    public void StartBreathing()
    {
        underWater = true;
        currOxygen = maxOxygen;
        StopCoroutine("CamZoom");
        StartCoroutine("CamZoom");
    }

    public void StopBreathing()
    {
        underWater = false;
        StopAllCoroutines();

        spear.transform.localPosition = Vector3.zero;

        Rigidbody2D spearRB = spear.GetComponent<Rigidbody2D>();
        spearRB.gravityScale = 0f;
        spearRB.velocity = Vector3.zero;
        spearReturned = true;

        soundController.Loop("over");

        targetSize = maxSize;
        spearShot = false;
        spearReturned = true;
        moveSpeed = baseMoveSpeed;
    }

    void ReadInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(DiveIn(2));
        }
        if (underWater)
        {
            if (Input.GetKeyDown("space"))
            {
                StartCoroutine("InhaleBurst");
                StopCoroutine("SpeedBurst");
                StartCoroutine("SpeedBurst");

                StopCoroutine("CamZoom");
                StartCoroutine("CamZoom");

                soundController.Play("breathin");
            }

            if (Input.GetKeyUp("space"))
            {
                StopCoroutine("SpeedBurst");
                StartCoroutine("SpeedBurst");

                StopCoroutine("CamZoom");
                StartCoroutine("CamZoom");

                StartCoroutine("ExhaleParticles");
                soundController.Play("breathout");
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (!reeling && spearReturned)
                {
                    StartCoroutine("SpearCharge");
                }
                else
                {
                    StartCoroutine("SpearReturn");
                    reeling = true;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (spearReturned && !reeling)
                {
                    Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - spear.transform.position;
                    Rigidbody2D spearRB = spear.GetComponent<Rigidbody2D>();

                    spearRB.AddForce((spearMaxDistance * 400f) * spearStrength * direction.normalized);
                    spearRB.gravityScale = 0.2f;

                    // Spawn shot bubble particles
                    Vector3 partPos = gun.transform.position + gun.transform.rotation * (new Vector3(1.4f, .16f, 0f));
                    World w = FindObjectOfType<World>();
                    ParticleSystem ps = Instantiate(shotBubblesPrefab, partPos, gun.transform.rotation, w.GetNewestTile().transform) as ParticleSystem;

                    //rope.gameObject.SetActive(true);
                    soundController.Play("speargun");
                    spearStrength = 0f;
                    //spearShot = true;
                    spearReturned = false;
                }
                else 
                {
                    reeling = false;
                }
            }
        }
    }

    public float GetOxygenPercent()
    {
        return Mathf.Clamp(currOxygen / maxOxygen, 0f, 1f);
    }

    IEnumerator ExhaleParticles()
    {
        Vector3 partPos = gun.transform.position;
        partPos.x -= .27f;
        partPos.y += .137f;

        World w = FindObjectOfType<World>();
        int numPart = Mathf.FloorToInt(Random.Range(2f, 5f));

        for (int i = 0; i < numPart; ++i)
        {
            ParticleSystem ps = Instantiate(bubblePrefab, partPos, Quaternion.Euler(new Vector3(-90f, 0f, 0f)), w.GetNewestTile().transform) as ParticleSystem;
            yield return new WaitForSeconds(0.04f);
        }
    }

    IEnumerator InhaleBurst()
    {
        float time = 0f;
        while (time < 1f)
        {
            currOxygen = Mathf.Clamp(currOxygen - oxygenBurnRate * burstCurve.Evaluate(time), 0f, maxOxygen);
            time += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator SpeedBurst()
    {
        float time = 0f;

        while (time < 2.2f)
        {
            moveSpeed = baseMoveSpeed + baseMoveSpeed * burstMovementModifier * (burstCurve.Evaluate(time / 2.2f));
            time += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator SpearCharge()
    {
        float time = 0f;

        while(Input.GetMouseButton(0) && (time < 0.7f || spearStrength >= 1f))
        {
            spearStrength = Mathf.Clamp(time / 0.7f, 0f, 1f);
            time += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator SpearReturn()
    {
        Vector3 currPos = spear.transform.localPosition;
        float reelSpeed = spearMaxDistance;

        while(Vector3.Distance(currPos, Vector3.zero) > reelSpeed*Time.deltaTime)
        {
            Vector3 moveDirectionNormalized = (-1f * currPos).normalized;
            currPos = currPos + moveDirectionNormalized * reelSpeed * Time.deltaTime;
            spear.transform.localPosition = currPos;

            yield return null;
        }

        spear.transform.localPosition = Vector3.zero;

        Rigidbody2D spearRB = spear.GetComponent<Rigidbody2D>();
        spearRB.gravityScale = 0f;
        spearRB.velocity = Vector3.zero;
        spearReturned = true;
        spearShot = false;
        //rope.gameObject.SetActive(false);
    }

    IEnumerator CamZoom()
    {
        float zoomTime = 7f, time = 0f, startingX = cam.transform.position.x;

        while (time < zoomTime)
        {
            Vector3 pos = cam.transform.position;
            targetSize = Mathf.Lerp(maxSize, minSize, camZoomCurve.Evaluate(time / zoomTime));

            time += Time.deltaTime;

            pos.x = startingX + 0.1f * camZoomCurve.Evaluate(time / zoomTime) * camShakeCurve.Evaluate((time % 0.3f) / 0.3f);
            cam.transform.position = pos;

            yield return null;
        }

        soundController.Play("breathin");
        runFailed.Invoke(RunFailedStatus.HeldBreath);
    }

    public IEnumerator DiveIn(float diveTime)
    {
        if (!playerCharacter)
            playerCharacter = FindObjectOfType<PlayerCharacter>();

        soundController.Play("jump");

        float currentTime = 0;
        bool playedSplashSound = false;
        while(currentTime <= diveTime)
        {
            float x = diveCurveX.Evaluate(currentTime / diveTime);
            float y = diveCurveY.Evaluate(currentTime / diveTime);

            if(!playedSplashSound && currentTime / diveTime > 0.6f)
            {
                soundController.Play("splash");
                soundController.Loop("under2");
                playedSplashSound = true;
            }

            //Move player according to the curve
            playerCharacter.transform.position = new Vector3(x, y, 0);
            float rotationZ = diveRotationZ.Evaluate(currentTime / diveTime);
            playerCharacter.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotationZ));

            Vector3 camStart = new Vector3(-26.7f, 7.85f, -10f);
            Vector3 camEnd = new Vector3(0, -1.76f, -10f);

            Camera.main.orthographicSize = 9.06f - (9.06f - 7) * (1 - currentTime / diveTime);
            Vector3 camLocation = camEnd - (camEnd - camStart) * (1 - currentTime / diveTime);
            Camera.main.transform.position = camLocation;

            currentTime += Time.deltaTime;

            yield return null;
        }

        StartBreathing();
        spear.GetComponent<Collider2D>().enabled = true;
        if(swimUpRoutine != null)
        {
            StopCoroutine(swimUpRoutine);
        }

        yield return null;
    }

    public void OnFishCaught(GameObject fish)
    {
        currentValue += fish.GetComponent<Fish>().value;  
    }

    public void FinishRun()
    {
        AddCash(currentValue);
        currentValue = 0;

        var spearedFish = spear.GetComponentsInChildren<Fish>();
        for(int i = spearedFish.Length - 1; i >= 0; i--)
        {
            Destroy(spearedFish[i].gameObject);
        }

        currOxygen = maxOxygen;

        spear.GetComponent<Collider2D>().enabled = false;
        StopBreathing();
        swimUpRoutine = StartCoroutine(SwimUp());

        World.world.ResetRun();
    }
    
    IEnumerator SwimUp()
    {

        float swimUpDuration = 2f;
        float currentTime = 0;

        float targetY = 5f;
        //The coroutine is stopped when the player starts a new run

        float startY = playerCharacter.transform.position.y;

        while (currentTime < swimUpDuration)
        {
            float y = targetY - (targetY - startY) * (1 - currentTime / swimUpDuration);
            Vector3 pos = playerCharacter.transform.position;
            pos.y = y;
            playerCharacter.transform.position = pos;

            float rotationZ = swimUpRotationZ.Evaluate(currentTime / swimUpDuration);

            playerCharacter.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotationZ));


            currentTime += Time.deltaTime;
            yield return null;
        }

        //playerCharacter.gameObject.SetActive(false);
    }

    public void AddBaseMoveSpeed(float amount)
    {
        baseMoveSpeed += amount;
    }

    public void AddCash(int amount)
    {
        cash += amount;
    }

    public void AddMaxOxygen(float amount)
    {
        maxOxygen += amount;
    }
}
