using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Zone_Flags : MonoBehaviour
{
    [SerializeField] private Transform Master_Flags = null;
    protected Dictionary<string, bool> Flag = new Dictionary<string, bool>(); //This int is often used like a bool (0,1)
    protected QuestsHolder QH;


    protected void Start()
    {
        QH = FindObjectOfType<QuestsHolder>();
        Recursive_Flag_Add(Master_Flags);

        /*
        foreach (KeyValuePair<string, bool> kvp in Flag)
        { 
            Debug.Log(kvp.Key);
        }
        */

        for (int i = 5; i < STARTUP_DECLARATIONS.FactionCount; ++i)
        {
            Assert.IsTrue(Flag.ContainsKey(STARTUP_DECLARATIONS.FactionsEnumReverse[i] + "Hostile"), "There needs to be a hostile flag for the new faction: " + STARTUP_DECLARATIONS.FactionsEnumReverse[i]);
        }
    }

    protected void Recursive_Flag_Add(Transform PassThrough)
    {
        if (PassThrough.childCount == 0)
        {
            Flag.Add(PassThrough.name, false);
            return;
        }

        foreach(Transform child in PassThrough) 
        {
            Recursive_Flag_Add(child);
        }
    }

    public bool CheckFlag(string FlagRef)
    {
        return Flag[FlagRef];
    }

    public void SetFlag(GameObject FlagRef) //I am just using the GameObject for its unique number, I dont actually store anything to it
    {
        Debug.Log(FlagRef);
        World_Setup WS = FlagRef.GetComponent<World_Setup>();
        if (WS)
        {
            WS.Setup();
        }

        Flag[FlagRef.name] = true;
        FlagEffects(FlagRef.name);
    }

    protected virtual void FlagEffects(string FlagRef)
    {
        QH.CheckFlags(FlagRef);
    }

    protected virtual void FixedUpdate()
    {
        
    }
}
