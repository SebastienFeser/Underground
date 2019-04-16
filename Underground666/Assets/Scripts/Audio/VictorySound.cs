using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictorySound : MonoBehaviour
{
    private void Update()
    {
        if (Time.timeSinceLevelLoad > 5f)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
