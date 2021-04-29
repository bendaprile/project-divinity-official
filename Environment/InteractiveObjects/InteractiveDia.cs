using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

public class InteractiveDia : InteractiveThing
{
    [SerializeField] private Transform DiaTrans = null;
    [SerializeField] private DiaRoot DR = null;

    [SerializeField] private float CombatSize = 0.8f;
    [SerializeField] private float DiaSize = 0.8f;

    private bool in_combat;
    private string Temp_text;
    private float Original_size;

    public virtual void SetText(string str)
    {
        overHead_t_mesh.text = str;
    }

    protected override void Awake()
    {
        Assert.IsFalse(LOCK_UNTIL_QUEST); //DO NOT USE (No reason not to, but not needed... yet..)
        base.Awake();
        in_combat = false;
        Original_size = overHead_t_mesh.fontSize;
    }

    public void CombatText()
    {
        fixedName = true;
        in_combat = true; 
        overHead_t_mesh.fontSize = Original_size * CombatSize;
    }

    public void NormalText()
    {
        fixedName = false;
        in_combat = false;
        overHead_t_mesh.fontSize = Original_size;
    }

    public void ExternalDisplayText(string str)
    {
        StartCoroutine(DisplayDia(str));
    }



    private IEnumerator DisplayDia(string str)
    {
        fixedName = true;
        Temp_text = overHead_t_mesh.text;

        overHead_t_mesh.color = STARTUP_DECLARATIONS.goldColor;
        overHead_t_mesh.fontSize = Original_size * DiaSize;
        SetText(str);

        float timer = 5;
        while(timer > 0)
        {
            timer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        overHead_t_mesh.color = Color.white;
        SetText(Temp_text);
        if (in_combat) //In case the mode switched
        {
            CombatText();
        }
        else
        {
            NormalText();
        }

    }


    protected override void ActivateLogic()
    {
        if (Input.GetKeyDown(KeyCode.F) && !fixedName)
        {
            ForcedActivate();
        }
    }

    public void ForcedActivate()
    {
        QuestActiveLogic();
        Transform StartingTrans = DiaTrans.GetComponent<DiaRoot>().ReturnStarting();
        DiaOverhead diaOver = StartingTrans.GetComponent<DiaOverhead>();
        if (diaOver)
        {
            StartCoroutine(DisplayDia(diaOver.return_line()));
        }
        else
        {
            UIControl.DialogueMenuBool(DiaTrans);
        }
    }

    protected override bool HasTask()
    {
        return DR.dia_quest_objective_count > 0;
    }

    protected override bool StartsQuest()
    {
        return DR.has_quest;
    }
}
