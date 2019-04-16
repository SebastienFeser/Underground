using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPickup : MonoBehaviour
{
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip clip;

    private void Start()
    {
        source.clip = clip;
        source.Play();
    }
    private void Update()
    {
        if (!source.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
