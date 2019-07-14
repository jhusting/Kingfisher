using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundToggleScript : MonoBehaviour
{
    private Image myImage;
    private SoundController sc;

    [SerializeField]
    private Sprite onSprite;
    [SerializeField]
    private Sprite offSprite;
    // Start is called before the first frame update
    void Start()
    {
        myImage = GetComponent<Image>();

        sc = FindObjectOfType<SoundController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleOnOff(bool state)
    {
        if(state)
        {
            myImage.sprite = onSprite;

            if(sc)
                sc.SetVolume(1f);
        }
        else
        {
            myImage.sprite = offSprite;

            if (sc)
                sc.SetVolume(0f);
        }
    }
}
