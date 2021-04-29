using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FlagBasedObjective : QuestObjective
{
    [SerializeField] GameObject[] ProperFlags = new GameObject[1];
    [SerializeField] bool[] ProperFlags_polarity = new bool[1];

    private void Awake()
    {
        Assert.IsTrue(ProperFlags.Length == NumberOfTasks);
        Assert.IsTrue(ProperFlags_polarity.Length == NumberOfTasks);
    }

    protected override void FlagCheckProperCompletion()
    {
        for(int i = 0; i < NumberOfTasks; ++i)
        {
            if (questTemplate.Return_Zone_Flags().CheckFlag(ProperFlags[i].name) == ProperFlags_polarity[i])
            {
                TaskCompletion_Self(i);
            }
        }
    }
}
