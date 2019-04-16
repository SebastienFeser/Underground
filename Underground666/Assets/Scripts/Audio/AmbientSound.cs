using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSound : MonoBehaviour
{
    [SerializeField] AudioSource ambientSource;
    void Start()
    {
        ambientSource.Play();
    }

    
}
