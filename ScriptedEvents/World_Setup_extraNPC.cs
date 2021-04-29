using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class World_Setup_extraNPC : World_Setup
{

    [SerializeField] private List<bool> npcOverheadPri_add = new List<bool>() { false };
    [SerializeField] [TextArea(3, 10)]  private List<string> OverheadPri = new List<string>() { "" };


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Assert.IsTrue(npcs.Count == npcOverheadPri_add.Count);
        Assert.IsTrue(npcs.Count == OverheadPri.Count);
    }

    // Update is called once per frame
    public override void Setup()
    {
        base.Setup();
        for (int i = 0; i < npcs.Count; ++i)
        {
            if (npcOverheadPri_add[i])
            {
                npcs[i].GetComponentInChildren<DiaOverhead>().add_Priority_once_line(OverheadPri[i]);
            }
        }
    }
}
