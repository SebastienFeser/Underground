using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPath : MonoBehaviour
{

    [SerializeField] WaypointMoving enemyWaypointMoving;

    private void FixedUpdate()
    {
        enemyWaypointMoving.ForFixedUpdate();
    }
}
