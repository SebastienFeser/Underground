﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameObject.FindGameObjectWithTag("AllPickups").GetComponent<AllMapsPickups>()
            Destroy(gameObject);
        }
    }
}
