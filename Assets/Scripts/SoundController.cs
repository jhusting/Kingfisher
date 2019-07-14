using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    AudioSource source;

    [SerializeField]
    AudioClip[] clips;

    [SerializeField]
    AudioClip[] loops;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        //source.PlayOneShot(clips[0], 1f);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Play(string s)
    {
        s = s.ToLower();

        switch(s)
        {
            case "breathin":
                source.PlayOneShot(clips[0], 1f);
                break;
            case "breathout":
                source.PlayOneShot(clips[1], 1f);
                break;
            case "splash":
                source.PlayOneShot(clips[2], 1f);
                break;
            case "bubble":
                source.PlayOneShot(clips[3], 1f);
                break;
            case "speargun":
                source.PlayOneShot(clips[4], 1f);
                break;
            case "jump":
                source.PlayOneShot(clips[5], 1f);
                break;
            default:
                Debug.Log("Input error, I don't know what to play");
                break;
        }
    }

    public void Loop(string s)
    {
        s = s.ToLower();

        switch (s)
        {
            case "over":
                source.clip = loops[0];
                source.Play();
                break;
            case "under1":
                source.clip = loops[1];
                source.Play();
                break;
            case "under2":
                source.clip = loops[2];
                source.Play();
                break;
            default:
                Debug.Log("Input error, I don't know what to play");
                break;
        }
    }
}
