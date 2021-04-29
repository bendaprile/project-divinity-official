using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class World_Setup : MonoBehaviour
{
    [SerializeField] [TextArea(3, 10)] private string COMMENT = "";
    [Space(35)]

    [SerializeField] protected List<Transform> npcs = new List<Transform>() {};
    [SerializeField] protected List<bool> npcSetActive = new List<bool>() { };
    [SerializeField] protected List<Transform> npcDiaModifyStarting = new List<Transform>() { };

    [Space(35)]

    [SerializeField] protected List<bool> npcControlModeBool = new List<bool>() { };
    [SerializeField] protected List<NPC_Control_Mode> npcControlMode = new List<NPC_Control_Mode>() { };

    [Space(35)]

    [SerializeField] protected List<bool> npcFactionSwapBool = new List<bool>() { };
    [SerializeField] protected List<FactionsEnum> npcFactionSwap = new List<FactionsEnum>() { };

    [Space(50)]

    [SerializeField] protected List<GameObject> ObjectsToEnable = new List<GameObject>();
    [SerializeField] protected List<GameObject> ObjectsToDisable = new List<GameObject>();

    [Space(50)]

    [SerializeField] protected List<AudioClip> Playlist_to_play = new List<AudioClip>();
    [SerializeField] protected bool revert_music_to_general = false;

    [Space(50)]

    [SerializeField] protected List<GameObject> GiveItems = new List<GameObject>();

    [Space(50)]


    [SerializeField] protected PlayerMovementSet SetMovement = PlayerMovementSet.Nothing;




    protected enum PlayerMovementSet { Nothing, Enable, Disable };
    protected NonDiegeticController NDC;
    protected Inventory INV;


    protected virtual void Start()
    {
        NDC = FindObjectOfType<NonDiegeticController>();
        INV = FindObjectOfType<Inventory>();

        Assert.IsTrue(npcs.Count == npcDiaModifyStarting.Count || npcDiaModifyStarting.Count == 0);

        Assert.IsTrue(npcs.Count == npcSetActive.Count || npcSetActive.Count == 0);

        Assert.IsTrue(npcs.Count == npcControlModeBool.Count || npcControlModeBool.Count == 0);
        Assert.IsTrue(npcs.Count == npcControlMode.Count || npcControlMode.Count == 0);

        Assert.IsTrue(npcs.Count == npcFactionSwapBool.Count || npcFactionSwapBool.Count == 0);
        Assert.IsTrue(npcs.Count == npcFactionSwap.Count || npcFactionSwap.Count == 0);

        for(int i = 0; i < ObjectsToEnable.Count; i++)
        {
            Assert.IsFalse(ObjectsToEnable[i].activeSelf);
        }
    }


    public virtual void Setup()
    {
        for(int i = 0; i < npcs.Count; ++i)
        {
            if (npcSetActive.Count > 0 && npcSetActive[i])
            {
                npcs[i].gameObject.SetActive(true);
            }

            if (npcDiaModifyStarting.Count > 0 && npcDiaModifyStarting[i])
            {
                npcs[i].GetComponentInChildren<DiaRoot>().ModifyStarting(npcDiaModifyStarting[i]);
            }

            if (npcControlModeBool.Count > 0 && npcControlModeBool[i])
            {
                npcs[i].GetComponent<HumanoidMaster>().Set_ControlMode(npcControlMode[i]);
            }

            if (npcFactionSwapBool.Count > 0 && npcFactionSwapBool[i])
            {
                npcs[i].GetComponent<HumanoidMaster>().HumanoidSwitchFaction(npcFactionSwap[i]);
            }
        }

        foreach(GameObject obj in ObjectsToEnable)
        {
            obj.SetActive(true);
        }

        foreach (GameObject obj in ObjectsToDisable)
        {
            obj.SetActive(false);
        }



        if (Playlist_to_play.Count != 0)
        {
            NDC.ChangeAudioSpecific(Playlist_to_play);
        }
        else if (revert_music_to_general)
        {
            NDC.ChangeAudioGeneral();
        }


        foreach (GameObject obj in GiveItems)
        {
            INV.AddItem(obj);
        }


        if (SetMovement == PlayerMovementSet.Enable)
        {
            FindObjectOfType<PlayerMaster>().Set_PlayerControl(true);
        }
        else if(SetMovement == PlayerMovementSet.Disable)
        {
            FindObjectOfType<PlayerMaster>().Set_PlayerControl(false);
        }
    }
}
