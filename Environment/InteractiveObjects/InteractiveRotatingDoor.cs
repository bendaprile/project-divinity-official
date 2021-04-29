using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveRotatingDoor : InteractiveThing
{
    [SerializeField] protected Material[] materials = null;
    [SerializeField] int LockPickingRequirement = 0;
    [SerializeField] ItemMaster keyToUnlock = null;
    private PlayerStats playerStats;
    private MeshRenderer mesh;
    private MiscDisplay miscDisplay;
    private DoorController doorController;
    private Inventory inventory;

    InteractiveRotatingDoor()
    {
        MiscDisplayUsed = true;
    }

    protected override void Awake()
    {
        base.Awake();
        mesh = GetComponent<MeshRenderer>();
        playerStats = player.GetComponent<PlayerStats>();
        miscDisplay = FindObjectOfType<MiscDisplay>();
        doorController = GetComponent<DoorController>();
        inventory = FindObjectOfType<Inventory>();
    }

    public override void CursorOverObject()
    {
        base.CursorOverObject();
        set_item_material(1);
    }

    public override void CursorLeftObject()
    {
        base.CursorLeftObject();
        set_item_material(0);
    }

    protected virtual void set_item_material(int i)
    {
        mesh.material = materials[i];
    }

    // TODO: Handle Input through InputManager and not direct key references
    protected override void ActivateLogic()
    {
        string openCloseText = doorController.open ? "Close" : "Open";
        if (LockPickingRequirement == 0)
        {
            miscDisplay.enableDisplay("", "(F) " + openCloseText);
            if (Input.GetKeyDown(KeyCode.F))
            {
                QuestActiveLogic();
                doorController.ActivateDoor();
            }
        }
        else if (playerStats.ReturnNonCombatSkill(SkillsEnum.Larceny) >= LockPickingRequirement)
        {
            miscDisplay.enableDisplay("",
                "Larceny Met [" + playerStats.ReturnNonCombatSkill(SkillsEnum.Larceny) + "/" + LockPickingRequirement + "] (F)",
                SkillCheckStatus.Success);
            if (Input.GetKeyDown(KeyCode.F))
            {
                QuestActiveLogic();
                doorController.ActivateDoor();
            }
        }
        else if (keyToUnlock && inventory.ReturnItems_ByName(keyToUnlock).Count > 0)
        {
            miscDisplay.enableDisplay("", "Use Key: (F) " + openCloseText, SkillCheckStatus.Success);
            if (Input.GetKeyDown(KeyCode.F))
            {
                QuestActiveLogic();
                doorController.ActivateDoor();
            }
        }
        else
        {
            miscDisplay.enableDisplay("",
                "Larceny Not Met [" + playerStats.ReturnNonCombatSkill(SkillsEnum.Larceny) + "/" + LockPickingRequirement + "]",
                SkillCheckStatus.Failure);
        }
    }
}
