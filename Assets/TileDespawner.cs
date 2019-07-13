using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDespawner : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D coll)
    {
        Debug.Log("HELLO!?");
        GameObject go = coll.gameObject;

        if(go.GetComponent<BackgroundTile>() || go.GetComponent<MiddlegroundTile>() || go.GetComponent<ForegroundTile>())
        {
            Debug.Log("MORE HELLO!?");
            Destroy(go.gameObject);
        }
    }
}
