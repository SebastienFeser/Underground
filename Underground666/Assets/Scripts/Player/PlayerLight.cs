using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLight : MonoBehaviour
{
    [SerializeField] Light torch;

    private void Update()
    {
        if (Input.GetButtonDown("Torch"))
        {
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
