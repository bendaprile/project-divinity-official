using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractiveThing : MonoBehaviour
{
    [SerializeField] protected bool LOCK_UNTIL_QUEST = false;

    [SerializeField] protected List<Transform> QuestIndicators = new List<Transform>();
    protected GameObject player;
    protected bool isCursorOverhead;
    protected PlayerInRadius PIR;
    protected GameObject Text;
    protected UIController UIControl;
    protected TextMeshPro overHead_t_mesh;
    protected bool fixedName = false;

    protected bool MiscDisplayUsed;


    protected GameObject QuestRef;
    protected int QuestInt;



    protected virtual void Awake()
    {
        if (!MiscDisplayUsed)
        {
            Text = transform.Find("Text").gameObject;
            overHead_t_mesh = GetComponentInChildren<TextMeshPro>();
        }

        foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
        {
            if (sprite.gameObject.name == "QuestIndicator")
            {
                QuestIndicators.Add(sprite.transform);
            }
        }

        PIR = GetComponentInChildren<PlayerInRadius>();
        player = GameObject.Find("Player");
        UIControl = GameObject.Find("UI").GetComponent<UIController>();
    }

    public void SetQuest(GameObject QuestRef_in, int i)
    {
        LOCK_UNTIL_QUEST = false;
        QuestInt = i;
        QuestRef = QuestRef_in;
    }

    public void ForceQuestEnd()
    {
        QuestRef = null;
    }


    public virtual void CursorOverObject()
    {
        isCursorOverhead = true;
    }

    public virtual void CursorLeftObject()
    {
        isCursorOverhead = false;
    }

    protected virtual void ActivateLogic()
    {

    }

    protected void QuestActiveLogic()
    {
        if (QuestRef)
        {
            GameObject.Find("QuestsHolder").GetComponent<QuestsHolder>().CheckTaskCompletion(QuestRef, QuestInt);
            QuestRef = null;
        }
    }

    protected virtual void Update()
    {
        if (LOCK_UNTIL_QUEST)
        {
            return;
        }

        if ((isCursorOverhead && PIR.isTrue) || fixedName)
        {
            if (Text)
            {
                Text.SetActive(true);
                rotate(Text.transform);
            };
            ActivateLogic();
        }
        else
        {
            if (Text) { Text.SetActive(false); };
        }

        if (QuestIndicators.Count > 0) //There might be dia without an indicator like using a computer
        {
            if (HasTask() && !fixedName)
            {
                foreach (Transform questIndicator in QuestIndicators)
                {
                    questIndicator.gameObject.SetActive(true);
                    questIndicator.GetComponent<SpriteRenderer>().color = Color.white;
                    rotate(questIndicator);
                }
            }
            else if (StartsQuest() && !fixedName)
            {
                foreach (Transform questIndicator in QuestIndicators)
                {
                    questIndicator.gameObject.SetActive(true);
                    questIndicator.GetComponent<SpriteRenderer>().color = STARTUP_DECLARATIONS.goldColor;
                    rotate(questIndicator);
                }
            }
            else
            {
                foreach (Transform questIndicator in QuestIndicators)
                {
                    questIndicator.gameObject.SetActive(false);
                }
            }
        }
    }

    protected virtual bool HasTask()
    {
        return QuestRef != null;
    }

    protected virtual bool StartsQuest()
    {
        return false;
    }

    protected virtual void rotate(Transform RotateObj)
    {
        RotateObj.transform.eulerAngles = new Vector3(RotateObj.transform.eulerAngles.x,  Camera.main.transform.rotation.eulerAngles.y, RotateObj.transform.eulerAngles.z);
    }
}
