using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FlagBasedObjective : QuestObjective
{
    [SerializeField] GameObject[] ProperFlags = new GameObject[1];
    [SerializeField] bool[] ProperFlags_polarity = new bool[1];

    [SerializeField] GameObject[] AlternateFlags = new GameObject[0];
    [SerializeField] bool[] AlternateFlags_polarity = new bool[0];

    private void Awake()
    {
        Assert.IsTrue(ProperFlags.Length == NumberOfTasks);
        Assert.IsTrue(ProperFlags_polarity.Length == NumberOfTasks);

        Assert.IsTrue(AlternateFlags.Length == NumberOfTasks || AlternateFlags.Length == 0);
        Assert.IsTrue(AlternateFlags_polarity.Length == NumberOfTasks || AlternateFlags_polarity.Length == 0);
    }

    protected override void FlagCheckProperCompletion()
    {
        for(int i = 0; i < NumberOfTasks; ++i)
        {
            if (questTemplate.Return_Zone_Flags().CheckFlag(ProperFlags[i].name) == ProperFlags_polarity[i])
            {
                Debug.Log(ProperFlags[i].name);
                TaskCompletion_Self(i);
            }
            else if(AlternateFlags.Length != 0 && questTemplate.Return_Zone_Flags().CheckFlag(AlternateFlags[i].name) == AlternateFlags_polarity[i])
            {
                Debug.Log(AlternateFlags[i].name);
                TaskCompletion_Self(i);
            }
        }
    }
}
