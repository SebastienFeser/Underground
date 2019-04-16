using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLight : MonoBehaviour
{
    [SerializeField] Light torch;
    [SerializeField] AudioSource lightSource;
    [SerializeField] AudioClip lightClip;

    private void Start()
    {
        lightSource.clip = lightClip;
    }
    private void Update()
    {
        if (Input.GetButtonDown("Torch"))
        {
            lightSource.Play();
            if (torch.enabled)
            {
                torch.enabled = false;
            }
            else
            {
                torch.enabled = true;
            }
        }
    }
}
