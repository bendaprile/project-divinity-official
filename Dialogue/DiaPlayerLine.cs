﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiaPlayerLine : DiaMaster
{
    [SerializeField] [TextArea(3, 10)] private string Line = "NotSet";
    [SerializeField] private Transform Dest = null;

    [SerializeField] public GameObject FlagRef_req;
    [SerializeField] public bool FlagRefVal_req;

    [SerializeField] public int SkillLevelReq = 0; //0 if no check
    [SerializeField] public SkillsEnum SkillCheck = SkillsEnum.Speech;

    [SerializeField] public int CombatSkillLevelReq = 0; //0 if no check
    [SerializeField] public CombatSkillsEnum CombatSkillCheck = CombatSkillsEnum.armor_proficiency;



    /// Set //////////////////////////////////////////
    [SerializeField] private Transform NewStartingLine = null;
    [SerializeField] private List<GameObject> DisableList = null; //If used for Dia can only disable player options
    /// Set //////////////////////////////////////////

    private Zone_Flags ZF;
    private PlayerStats PS;

    public string return_line()
    {
        return Line;
    }

    public bool Check_Accessible(TextMeshProUGUI textRef) //Can use with textRef or without
    {
        PS = GameObject.Find("Player").GetComponent<PlayerStats>();
        ZF = GameObject.Find("Master Object").GetComponentInChildren<Zone_Flags>();

        string viewable_text = "";
        bool Accessible = true;

        if (SkillLevelReq > 0)
        {
            int charskill = PS.ReturnNonCombatSkill(SkillCheck);
            if (SkillLevelReq <= charskill)
            {
                if (textRef)
                {
                    textRef.color = STARTUP_DECLARATIONS.checkSuccessColor;
                }
            }
            else
            {
                if (textRef)
                {
                    textRef.color = STARTUP_DECLARATIONS.checkFailColor;
                }
                Accessible = false;
            }

            viewable_text += "[" + STARTUP_DECLARATIONS.SkillEnumReverse[(int)SkillCheck] + " (" + charskill + "/" + SkillLevelReq + ")] ";
        }

        if (CombatSkillLevelReq > 0)
        {
            int charcskill = PS.ReturnCombatSkill(CombatSkillCheck);
            if (CombatSkillLevelReq <= PS.ReturnCombatSkill(CombatSkillCheck))
            {
                if (textRef)
                {
                    textRef.color = STARTUP_DECLARATIONS.checkSuccessColor;
                }
            }
            else
            {
                if (textRef)
                {
                    textRef.color = STARTUP_DECLARATIONS.checkFailColor;
                }
                Accessible = false;
            }

            viewable_text += "[" + STARTUP_DECLARATIONS.CombatSkillsEnumReverse[(int)CombatSkillCheck] + " (" + charcskill + "/" + CombatSkillLevelReq + ")] ";
        }


        if (FlagRef_req && ZF.CheckFlag(FlagRef_req.name) != FlagRefVal_req) //DO NOT SHOW IF FLAG ISN'T CORRECT
        {
            viewable_text = "Option from another universe";
            if (textRef)
            {
                textRef.color = Color.grey;
            }
            Accessible = false;
        }
        else if(textRef)
        {
            if (textRef.color != STARTUP_DECLARATIONS.checkFailColor)
            {
                viewable_text += return_line();
            }
        }


        if (MakeHostile)
        {
            viewable_text += " [Attack]";
        }
        else if (Dest == null)
        {
            viewable_text += " [Leave Conversation]";
        }





        if (textRef)
        {
            textRef.text = viewable_text;
        }

        return Accessible;
    }

    public Transform return_new_start()
    {
        return NewStartingLine;
    }

    public Transform return_dest(bool real = true) //This means that it is clicked
    {
        if (real)
        {
            foreach (GameObject temp in DisableList)
            {
                temp.SetActive(false);
            }

            if(DisableList.Count > 0)
            {
                GetComponentInParent<DiaRoot>().check_for_quest();
            }
        }

        return Dest;
    }
}
