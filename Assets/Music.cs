using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        source.Play();
    }

    private void FixedUpdate()
    {
        if (!source.isPlaying) source.Play();
    }
     
}
