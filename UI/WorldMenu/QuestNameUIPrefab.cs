using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class QuestNameUIPrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI Details = null;
    [SerializeField] private Image questFocus = null;

    private GameObject QuestRef;
    private int iterStorage;
    private WorldMenuController WorldMenu;
    private Color32 originalColor;
    private Color32 hoverColor = new Color32(195, 195, 195, 255);
    private bool questActive = false;

    public void Setup(GameObject quest, int iter)
    {
        QuestRef = quest;
        QuestTemplate questScript = quest.GetComponent<QuestTemplate>();
        iterStorage = iter;
        Details.text = questScript.QuestName;
        WorldMenu = GameObject.Find("WorldMenu").GetComponent<WorldMenuController>();
        originalColor = Details.color;
    }

    public void CheckFocus(GameObject temp)
    {
        if(temp == QuestRef)
        {
            questFocus.enabled = true;
            Details.color = STARTUP_DECLARATIONS.goldColor;
            questActive = true; 
        }
        else
        {
            questFocus.enabled = false;
            Details.color = originalColor;
            questActive = false;
        }
    }

    private void DetailButtonPressed()
    {
        WorldMenu.GetDetails(iterStorage);
    }

    public void ActiveButtonPressed()
    {
        WorldMenu.QuestSetFocus(iterStorage);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DetailButtonPressed();
        if (questActive) 
        {
            Details.color = new Color32(220, 170, 0, 255);
            return; 
        }

        Details.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (questActive) 
        {
            Details.color = STARTUP_DECLARATIONS.goldColor;
            return; 
        }

        Details.color = originalColor;
    }
}
