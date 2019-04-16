using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpScare : MonoBehaviour
{
    [SerializeField] AudioSource jumpScareSource;
    [SerializeField] AudioClip jumpScareClip;

    private void Start()
    {
        jumpScareSource.clip = jumpScareClip;
        jumpScareSource.Play();
    }

    private void Update()
    {
        if (Time.timeSinceLevelLoad > 5f)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
