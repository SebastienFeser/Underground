using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstructionScript : MonoBehaviour
{
    
    void Update()
    {
       if(Input.GetButtonDown("Torch"))
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}
