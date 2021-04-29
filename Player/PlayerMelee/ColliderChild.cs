using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderChild : MonoBehaviour
{
    public List<Collider> TriggerList = new List<Collider>();

    [SerializeField] private string Tag_Name = "BasicEnemy";

    void OnTriggerEnter(Collider other)
    {
        if (!TriggerList.Contains(other))
        {
            if (other.gameObject.tag == Tag_Name)
            {
                TriggerList.Add(other);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (TriggerList.Contains(other))
        {
            if (other.gameObject.tag == Tag_Name)
            {
                TriggerList.Remove(other);
            }
        }
    }
}
