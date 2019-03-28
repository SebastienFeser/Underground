using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float sprintSpeedMultiplier;
    private float sprint;

    Rigidbody playerRigidbody;

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();   
    }


    private void Update()
    {
        float getHorizontal = Input.GetAxisRaw("Horizontal");
        float getVertical = Input.GetAxisRaw("Vertical");

        if (Input.GetButton("Sprint"))
        {
            sprint = sprintSpeedMultiplier;
        }
        else
        {
            sprint = 1;
        }
        

        playerRigidbody.velocity = transform.forward * movementSpeed * sprint * getVertical + transform.right * movementSpeed * getHorizontal;
    }

    
}
