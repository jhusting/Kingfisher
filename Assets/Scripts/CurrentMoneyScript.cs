using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentMoneyScript : MonoBehaviour
{
    Text t;
    private PlayerController pc;
    // Start is called before the first frame update
    void Start()
    {
        pc = FindObjectOfType<PlayerController>();
        t = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(pc && t)
        {
            t.text = pc.cash.ToString();
        }
    }
}
