using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParentOnTerrainCollision : MonoBehaviour
{
    [SerializeField] GameObject parent = null;


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Terrain") || other.gameObject.layer == LayerMask.NameToLayer("TerrainNoCam") || other.gameObject.layer == LayerMask.NameToLayer("Obstacles") || other.gameObject.layer == LayerMask.NameToLayer("SmallObstacles"))
        {
            Destroy(parent);
        }
    }
}
