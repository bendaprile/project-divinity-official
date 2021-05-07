using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitCollider : MonoBehaviour
{

    [SerializeField] private BuildingController buildingController = null;
    [SerializeField] private bool decreaseFloor = false;
    [SerializeField] private int floorNum = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!decreaseFloor)
        {
            if (other.tag == "Player" && other is CapsuleCollider)
            {
                buildingController.ExitCollider();
            }
        }
        else
        {
            if (other.tag == "Player" && other is CapsuleCollider)
            {
                buildingController.DecreaseFloor(floorNum);
            }
        }
    }
}
