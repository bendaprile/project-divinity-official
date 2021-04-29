using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GenericTalkObjective : QuestObjective
{
    [SerializeField] private GameObject[] ExactDiaLine = new GameObject[1];

    [SerializeField] List<Transform> NewStarts;

    [SerializeField] List<Transform> EnableDiaOptionWithPostObjCleanup;



    public override void initialize()
    {
        for (int i = 0; i < NewStarts.Count; ++i)
        {
            NewStarts[i].GetComponentInParent<DiaRoot>().ModifyStarting(NewStarts[i]);
        }

        for (int i = 0; i < EnableDiaOptionWithPostObjCleanup.Count; ++i)
        {
            EnableDiaOptionWithPostObjCleanup[i].gameObject.SetActive(true);
        }

        base.initialize();

        for (int i = 0; i < ExactDiaLine.Length; ++i)
        {
            ExactDiaLine[i].GetComponent<DiaMaster>().mark_dia_for_quest(gameObject, i);
        }
    }


    protected override void AdditionalObjectiveCompletionLogic()
    {
        for (int i = 0; i < EnableDiaOptionWithPostObjCleanup.Count; ++i)
        {
            EnableDiaOptionWithPostObjCleanup[i].gameObject.SetActive(false);
        }
    }

    protected override void FailLogic()
    {
        for (int i = 0; i < EnableDiaOptionWithPostObjCleanup.Count; ++i)
        {
            EnableDiaOptionWithPostObjCleanup[i].gameObject.SetActive(false);
        }
        base.FailLogic();
    }
}
