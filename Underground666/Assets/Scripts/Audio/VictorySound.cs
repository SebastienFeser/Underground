﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictorySound : MonoBehaviour
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
        if (Time.timeSinceLevelLoad > 5f)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
