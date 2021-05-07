using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterCollider : MonoBehaviour
{

    [SerializeField] private BuildingController buildingController = null;
    [SerializeField] private bool increaseFloor = false;
    [SerializeField] private int floorNum = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!increaseFloor)
        {
            if (other.tag == "Player" && other is CapsuleCollider)
            {
                buildingController.EnterCollider();
            }
        }
        else
        {
            if (other.tag == "Player" && other is CapsuleCollider)
            {
                buildingController.IncreaseFloor(floorNum);
            }
        }
    }
}
