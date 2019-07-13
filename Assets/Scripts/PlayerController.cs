using System.Collections;
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

    [SerializeField]
    private AnimationCurve diveCurveX;

    [SerializeField]
    private AnimationCurve diveCurveY;

    private RunFailedEvent runFailed;
    //Amount of cash the player has gotten on the current attempt
    public int currentValue { get; private set; }

    private void Awake()
    {
        runFailed = new RunFailedEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
        currOxygen = maxOxygen;
        cam = Camera.main;
        //StartBreathing();

        spear.GetComponent<Spear>().FishCaughtEvent.AddListener(OnFishCaught);
    }

    // Update is called once per frame
    void Update()
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

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, 0.8f*Time.deltaTime);

        if (currOxygen <= 0f)
            runFailed.Invoke(RunFailedStatus.NoOxygen);

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
        StopCoroutine("CamZoom");
        StartCoroutine("CamZoom");
    }

    public void StopBreathing()
    {
        underWater = false;
        StopAllCoroutines();

        Rigidbody2D spearRB = spear.GetComponent<Rigidbody2D>();
        spearRB.gravityScale = 0f;
        spearRB.velocity = Vector3.zero;
        spearReturned = true;

        targetSize = maxSize;
        spearShot = false;
        spearReturned = true;
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
            }

            if (Input.GetKeyUp("space"))
            {
                StopCoroutine("SpeedBurst");
                StartCoroutine("SpeedBurst");

                StopCoroutine("CamZoom");
                StartCoroutine("CamZoom");

                StartCoroutine("ExhaleParticles");
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (!spearShot)
                {
                    StartCoroutine("SpearCharge");
                }
                else
                    StartCoroutine("SpearReturn");
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (!spearShot)
                {
                    Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - spear.transform.position;
                    Rigidbody2D spearRB = spear.GetComponent<Rigidbody2D>();

                    spearRB.AddForce((spearMaxDistance * 400f) * spearStrength * direction.normalized);
                    spearRB.gravityScale = 0.2f;

                    //rope.gameObject.SetActive(true);
                    spearStrength = 0f;
                    spearShot = true;
                    spearReturned = false;
                }
                else
                {
                    spearShot = false;
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
        while (Input.GetKey("space"))
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
        float reelSpeed = 5f;

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

            pos.x = startingX + 0.05f * camZoomCurve.Evaluate(time / zoomTime) * camShakeCurve.Evaluate((time % 0.3f) / 0.3f);
            cam.transform.position = pos;

            yield return null;
        }

        runFailed.Invoke(RunFailedStatus.HeldBreath);
    }

    public IEnumerator DiveIn(float diveTime)
    {
        PlayerCharacter playerCharacter = FindObjectOfType<PlayerCharacter>();
        float currentTime = 0;
        while(currentTime <= diveTime)
        {
            float x = diveCurveX.Evaluate(currentTime / diveTime);
            float y = diveCurveY.Evaluate(currentTime / diveTime);

            //Move player according to the curve
            playerCharacter.transform.position = new Vector3(x, y, 0);

            Vector3 camStart = new Vector3(-18.54f, 12.47f, -10f);
            Vector3 camEnd = new Vector3(-8.87f, 0, -10f);
            Vector3 camLocation = camEnd - (camEnd - camStart) * (1 - currentTime / diveTime);
            Camera.main.transform.position = camLocation;

            currentTime += Time.deltaTime;

            yield return null;
        }

        StartBreathing();
        yield return null;
    }

    public void OnFishCaught(GameObject fish)
    {
        currentValue += fish.GetComponent<Fish>().value;  
    }
}
