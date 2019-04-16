using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllMapsPickups : MonoBehaviour
{
    List<Vector2> pickupPosition = new List<Vector2>();
    GameObject pickup1;
    GameObject pickup2;
    GameObject pickup3;
    GameObject pickup4;
    bool instantiatePickup1 = false;
    bool instantiatePickup2 = false;
    bool instantiatePickup3 = false;
    bool instantiatePickup4 = false;
    bool victory = false;
    [SerializeField] GameObject pickupGameObject;

    public List<Vector2> PickupPosition
    {
        get { return pickupPosition; }
        set { pickupPosition = value; }
    }
    


    private void Update()
    {
        if (instantiatePickup1 == true)
        {
            pickup1 = Instantiate(pickupGameObject);
            pickup1.transform.position = new Vector3(pickupPosition[0].x, 0, pickupPosition[0].y);
            instantiatePickup2 = true;
            instantiatePickup1 = false;
        }
        
        if (instantiatePickup2)
        {
            if (pickup1 == null)
            {
                pickup2 = Instantiate(pickupGameObject);
                pickup1.transform.position = new Vector3(pickupPosition[1].x, 0, pickupPosition[1].y);
                instantiatePickup2 = false;
                instantiatePickup3 = true;
            }
        }

        if (instantiatePickup3)
        {
            if (pickup2 == null)
            {
                pickup3 = Instantiate(pickupGameObject);
                pickup1.transform.position = new Vector3(pickupPosition[2].x, 0, pickupPosition[2].y);
                instantiatePickup3 = false;
                instantiatePickup4 = true;
            }
        }

        if (instantiatePickup4)
        {
            if (pickup3 == null)
            {
                pickup4 = Instantiate(pickupGameObject);
                pickup1.transform.position = new Vector3(pickupPosition[3].x, 0, pickupPosition[3].y);
                instantiatePickup4 = false;
            }
        }

        if (victory)
        {
            if (pickup4 == null)
            {
                Debug.Log("GG EZ");
            }
        }



    }




}
