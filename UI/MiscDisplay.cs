using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiscDisplay : MonoBehaviour
{
    private TextMeshProUGUI mainText;
    private TextMeshProUGUI secondaryText;
    private Color32 defaultMainColor;
    private Color32 defaultSecondaryColor;

    private float counter; 
    void Start()
    {
        mainText = transform.Find("MainText").GetComponent<TextMeshProUGUI>();
        secondaryText = transform.Find("SecondaryText").GetComponent<TextMeshProUGUI>();
        defaultMainColor = mainText.color;
        defaultSecondaryColor = secondaryText.color;
    }

    public void enableDisplay(string mtext, string stext, SkillCheckStatus skillCheck = SkillCheckStatus.NoCheck)
    {
        mainText.text = mtext;
        secondaryText.text = stext;

        switch (skillCheck)
        {
            case SkillCheckStatus.NoCheck:
                mainText.color = defaultMainColor;
                secondaryText.color = defaultSecondaryColor;
                break;
            case SkillCheckStatus.Failure:
                mainText.color = STARTUP_DECLARATIONS.checkFailColor;
                secondaryText.color = STARTUP_DECLARATIONS.checkFailColor;
                break;
            case SkillCheckStatus.Success:
                mainText.color = STARTUP_DECLARATIONS.checkSuccessColor;
                secondaryText.color = STARTUP_DECLARATIONS.checkSuccessColor;
                break;
        }

        gameObject.SetActive(true);
        counter = .1f;
    }

    // Update is called once per frame
    void Update()
    {
        if(counter > 0)
        {
            counter -= Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
