using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPath : MonoBehaviour
{

    [SerializeField] WaypointMoving playerWaypointMoving;

    private void FixedUpdate()
    {
        playerWaypointMoving.UpdateWaypointDetection();
    }
}
