using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AI_Master : MonoBehaviour
{
    Queue<GameObject> HighPriority = new Queue<GameObject>();
    Dictionary<GameObject, bool> in_HighPriority = new Dictionary<GameObject, bool>();

    Queue<GameObject> LowPriority = new Queue<GameObject>();
    Dictionary<GameObject, bool> in_LowPriority = new Dictionary<GameObject, bool>();

    [SerializeField] int LowPriority_per_frame = 5;
    [SerializeField] float HighPriority_per_frame = 10f;

    void Update()
    {
        for(int i = 0; i < LowPriority_per_frame; ++i)
        {
            if(LowPriority.Count == 0)
            {
                break;
            }
            GameObject temp = LowPriority.Dequeue();
            if (temp && temp.activeInHierarchy)
            {
                temp.GetComponent<EnemyTemplateMaster>().AI_Master_Interface();
            }
        }

        for (int i = 0; i < HighPriority_per_frame; ++i)
        {
            if (HighPriority.Count == 0)
            {
                break;
            }
            GameObject temp = HighPriority.Dequeue();
            if (temp && temp.activeInHierarchy)
            {
                temp.GetComponent<EnemyTemplateMaster>().AI_Master_Interface();
            }
        }
    }

    public void Request(GameObject Object, bool HiPri) 
    {
        if (HiPri)
        {
            if (!in_HighPriority.ContainsKey(Object))
            {
                HighPriority.Enqueue(Object);
            }
        }
        else
        {
            if (!in_LowPriority.ContainsKey(Object))
            {
                LowPriority.Enqueue(Object);
            }
        }
    }
}
