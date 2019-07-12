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

    public float moveSpeed = 1f;

    public float spearMaxDistance = 5f;
    public float spearStrength = 0f;

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

    private bool spearShot = false;
    private bool spearReturned = true;


    //Amount of cash the player has gotten on the current attempt
    public int currentValue { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        currOxygen = maxOxygen;
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

    void ReadInput()
    {
        if(Input.GetKeyDown("space"))
        {
            StartCoroutine("InhaleBurst");
            StartCoroutine("SpeedBurst");
            //currOxygen = Mathf.Clamp(currOxygen - 2f, 0f, maxOxygen);
        }

        if(Input.GetKeyUp("space"))
        {
            StartCoroutine("SpeedBurst");
        }

        if(Input.GetMouseButtonDown(0))
        {
            if (!spearShot)
            {
                StartCoroutine("SpearCharge");
            }
            else
                StartCoroutine("SpearReturn");
        }

        if(Input.GetMouseButtonUp(0))
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

    public float GetOxygenPercent()
    {
        return Mathf.Clamp(currOxygen / maxOxygen, 0f, 1f);
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
        while (time < 0.4f)
        {
            moveSpeed = baseMoveSpeed + burstMovementModifier * burstCurve.Evaluate(time / 0.4f);
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

    public void AddCurrentValue(int value)
    {
        currentValue += value;  
    }
}
