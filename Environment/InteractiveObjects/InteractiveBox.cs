using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using TMPro;

public class InteractiveBox : InteractiveThing
{
    public string interactiveBoxName = "Chest";
    [SerializeField] protected Material[] NormalMats = null;
    [SerializeField] protected Material[] HoverMats = null;
    [SerializeField] protected MeshRenderer[] MeshesForMats = null;

    [SerializeField] int LockPickingRequirement = 0;

    private Transform ItemHolder;
    private PlayerStats playerStats;

    public GameObject ReturnItem(int i)
    {
        return ItemHolder.GetChild(i).gameObject;
    }

    public List<GameObject> ReturnItems()
    {
        List<GameObject> tempItems = new List<GameObject>();
        foreach(Transform item in ItemHolder)
        {
            tempItems.Add(item.gameObject);
        }
        return tempItems;
    }


    protected override void Awake()
    {
        Assert.IsTrue(NormalMats.Length == HoverMats.Length);
        Assert.IsTrue(NormalMats.Length == MeshesForMats.Length);

        base.Awake(); 
        UIControl = GameObject.Find("UI").GetComponent<UIController>();
        playerStats = player.GetComponent<PlayerStats>();
        ItemHolder = transform.Find("ItemHolder");
    }

    public override void CursorOverObject()
    {
        base.CursorOverObject();
        set_item_material(true);
    }

    public override void CursorLeftObject()
    {
        base.CursorLeftObject();
        set_item_material(false);
    }

    protected virtual void set_item_material(bool active)
    {
        Material[] TempMats = active ? HoverMats : NormalMats;

        for(int i = 0; i < TempMats.Length; ++i)
        {
            MeshesForMats[i].material = TempMats[i];
        }
    }

    // TODO: Handle Input through InputManager and not direct key references
    protected override void ActivateLogic()
    {
        if(LockPickingRequirement == 0)
        {
            Text.GetComponent<TextMeshPro>().text = "(F) Open";
            if (Input.GetKeyDown(KeyCode.F))
            {
                QuestActiveLogic();
                UIControl.OpenInteractiveMenu(gameObject);
            }
        }
        else if (playerStats.ReturnNonCombatSkill(SkillsEnum.Larceny) >= LockPickingRequirement)
        {
            Text.GetComponent<TextMeshPro>().text = "Larceny [" + playerStats.ReturnNonCombatSkill(SkillsEnum.Larceny) + "/" + LockPickingRequirement + "] (F)";
            if (Input.GetKeyDown(KeyCode.F))
            {
                QuestActiveLogic();
                UIControl.OpenInteractiveMenu(gameObject);
            }
        }
        else
        {
            Text.GetComponent<TextMeshPro>().text = ("Larceny [" + playerStats.ReturnNonCombatSkill(SkillsEnum.Larceny) + "/" + LockPickingRequirement + "]");
        }
    }
}
