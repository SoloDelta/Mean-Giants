using FPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunShots : MonoBehaviour
{
    public AudioSource gSource;
    public AudioClip gClip;
    // Start is called before the first frame update
    void Start()
    {
        AudioInst();
    }
    private void AudioInst()
    {
        gSource = GetComponent<AudioSource>();
        gSource.clip = gClip;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void shootsound()
    {
        gSource.Play();
        


    }
}
