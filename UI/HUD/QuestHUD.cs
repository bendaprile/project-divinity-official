using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestHUD : MonoBehaviour
{
    [SerializeField] private Transform QuestTaskPrefab;
    private TextMeshProUGUI mainText;
    private Transform secondaryText;
    private Image questMarkerImage;
    private Color32 grayColor = new Color32(200, 200, 200, 255);

    private QuestTemplate FocusedTemp;
    private QuestObjective CurrentObj;

    void Start()
    {
        mainText = transform.Find("MainText").GetComponent<TextMeshProUGUI>();
        secondaryText = transform.Find("SecondaryText");
        questMarkerImage = transform.Find("Icon").GetComponent<Image>();

        DisableDisplay();
    }

    public void QuestDisplay(QuestTemplate FocusedTemp_in)
    {
        DisableDisplay();

        FocusedTemp = FocusedTemp_in;
        CurrentObj = FocusedTemp.returnActiveObjective();
        mainText.text = FocusedTemp.QuestName;
        SetupTaskDetail(CurrentObj);
        questMarkerImage.enabled = true;
    }

    public void DisableDisplay()
    {
        mainText.text = "";
        for (int i = secondaryText.childCount - 1; i >= 0; --i)
        {
            Destroy(secondaryText.GetChild(i).gameObject);
        }
        questMarkerImage.enabled = false;
    }

    private void SetupTaskDetail(QuestObjective tempObj)
    {
        List<(bool, string)> taskList = tempObj.ReturnTasks();
        foreach ((bool, string) task in taskList)
        {
            Transform textPrefab = Instantiate(QuestTaskPrefab, secondaryText);
            if (task.Item1)
            {
                textPrefab.Find("Text").GetComponent<TextMeshProUGUI>().color = grayColor;
                textPrefab.Find("Bullet").GetComponent<Image>().color = grayColor;
            }
            else
            {
                textPrefab.Find("Text").GetComponent<TextMeshProUGUI>().color = Color.white;
                textPrefab.Find("Bullet").GetComponent<Image>().color = Color.white;
            }

            textPrefab.Find("Text").GetComponent<TextMeshProUGUI>().text = task.Item2;
        }
    }

    private void FixedUpdate() //Piece of shit redundent code, but I'm too tired
    {
        if (questMarkerImage.enabled)
        {
            if(CurrentObj != FocusedTemp.returnActiveObjective())
            {
                QuestDisplay(FocusedTemp);
            }


            List<(bool, string)> taskList = (FocusedTemp.returnActiveObjective()).ReturnTasks();
            for(int i = 0; i < taskList.Count; ++i)
            {
                if (taskList[i].Item1)
                {
                    secondaryText.GetChild(i).Find("Text").GetComponent<TextMeshProUGUI>().color = Color.gray;
                    secondaryText.GetChild(i).Find("Bullet").GetComponent<Image>().color = Color.gray;
                }
                else
                {
                    secondaryText.GetChild(i).Find("Text").GetComponent<TextMeshProUGUI>().color = Color.white;
                    secondaryText.GetChild(i).Find("Bullet").GetComponent<Image>().color = Color.white;
                }
            }
        }

    }
}
