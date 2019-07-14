using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatWobble : MonoBehaviour
{
    Vector3 startPos;

    Vector3 startRotEulers;

    float lifeTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.localPosition;
        startRotEulers = transform.localRotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime += Time.deltaTime;
        Vector3 newPos = startPos;

        newPos.y += 0.005f* Mathf.Sin(lifeTime);

        transform.localPosition = newPos;

        Vector3 newRot = startRotEulers;
        newRot.z += 1.5f * Mathf.Sin(lifeTime + 0.3f);

        transform.localRotation = Quaternion.Euler(newRot);
    }
}
