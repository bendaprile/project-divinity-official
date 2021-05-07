using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class QuestObjective : MonoBehaviour
{
    [SerializeField] protected GameObject ObjectiveDest;
    [SerializeField] protected GameObject ObjectiveFailDest;
    [SerializeField] protected World_Setup World_Setup_Start = null; //Use this to help setup events


    [Space(35)]
    [SerializeField] protected int NumberOfTasks = 1;
    [SerializeField] private bool[] FakeTask = new bool[1] { false };
    [SerializeField] protected string[] TaskDescription = new string[1] { "" };


    [Space(35)]
    [SerializeField] protected List<GameObject> Any_Flag_Fail = new List<GameObject>();
    [SerializeField] protected bool Fail_On_Player_Death = false;
    [SerializeField] protected GameObject Set_Flag_On_Fail = null;
    [Space(35)]

    ///LOCATION
    [SerializeField] protected bool LocationHint = true;
    [SerializeField] protected List<Transform> ExactLocation = new List<Transform>();
    [SerializeField] protected List<float> Radius = new List<float>();
    ///LOCATION
    [Space(35)]


    private int NumberOfTasks_Real;
    private int TaskCompleteCount = 0;
    protected bool[] TaskCompleted;
    protected QuestTemplate questTemplate;

    public virtual void initialize()
    {
        TaskCompleted = new bool[NumberOfTasks];
        Assert.IsTrue(NumberOfTasks == TaskCompleted.Length, "Number of Tasks not equal to Number of tasks completed for " + gameObject.name);
        Assert.IsTrue(NumberOfTasks == FakeTask.Length, "Number of tasks is not equal to number of fake tasks for " + gameObject.name);
        Assert.IsTrue(NumberOfTasks == TaskDescription.Length, "Number of tasks is not equal to number of task descriptions for " + gameObject.name);
        if (LocationHint)
        {
            Assert.IsTrue(ExactLocation.Count == NumberOfTasks, "Number of locations is not equal to number of tasks for " + gameObject.name);
        }

        for (int i = 0; i < NumberOfTasks; ++i)
        {
            NumberOfTasks_Real += FakeTask[i] ? 0 : 1;
        }

        if (World_Setup_Start)
        {
            World_Setup_Start.Setup();
        }

        questTemplate = GetComponentInParent<QuestTemplate>();
        FlagCheckObj();
    }

    public virtual List<(bool, string)> ReturnTasks()
    {
        List<(bool, string)> taskList = new List<(bool, string)>();
        for (int i = 0; i < NumberOfTasks; ++i)
        {
            taskList.Add((TaskCompleted[i], TaskDescription[i]));
        }
        return taskList;
    }

    public virtual ObjectiveType ReturnType()
    {
        return ObjectiveType.NOTHING; 
    }

    public virtual (bool, List<(Vector2, float)>) ReturnLocs()
    {
        if (LocationHint)
        {
            return (false, ReturnLocsHelper());
        }
        else
        {
            return (false, new List<(Vector2, float)>());
        }
    }

    protected virtual void FailLogic()
    {
        Debug.Log("Fail");
        if (Set_Flag_On_Fail)
        {
            questTemplate.Return_Zone_Flags().SetFlag(Set_Flag_On_Fail);
        }

        questTemplate.ObjectiveFinished(ObjectiveFailDest);
    }

    protected virtual void AdditionalTaskCompletionLogic(int num)
    {

    }

    protected virtual void AdditionalObjectiveCompletionLogic()
    {

    }

    protected virtual void FlagCheckProperCompletion()
    {

    }

    //FIXED FUNCTIONS
    public void TaskCompletion(GameObject ObjectiveRef, int num)
    {
        if (ObjectiveRef == gameObject)
        {
            TaskCompletion_Self(num);
        }
    }

    public void TaskCompletion_Self(int num)
    {
        if (!TaskCompleted[num])
        {
            TaskCompleted[num] = true;
            if (!FakeTask[num])
            {
                AdditionalTaskCompletionLogic(num);
                TaskCompleteCount += 1;
            }
        }

        if (TaskCompleteCount == NumberOfTasks_Real)
        {
            AdditionalObjectiveCompletionLogic();
            questTemplate.ObjectiveFinished(ObjectiveDest);
        }
    }

    public void DeathCheckObj()
    {
        if (Fail_On_Player_Death)
        {
            FailLogic();
        }
    }

    public void FlagCheckObj()
    {
        foreach(GameObject Flag in Any_Flag_Fail)
        {
            if (questTemplate.Return_Zone_Flags().CheckFlag(Flag.name))
            {
                FailLogic();
                break;
            }
        }


        FlagCheckProperCompletion();
    }

    protected List<(Vector2, float)> ReturnLocsHelper()
    {
        List<(Vector2, float)> TempList = new List<(Vector2, float)>();

        for (int i = 0; i < ExactLocation.Count; i++)
        {
            float Rad = (Radius.Count > i) ? Radius[i] : 0;
            if (!TaskCompleted[i])
            {
                Vector2 tempExact = new Vector2(ExactLocation[i].position.x, ExactLocation[i].position.z);
                TempList.Add((tempExact, Rad));
            }
        }
        return TempList;
    }
    ////////////////////
}
