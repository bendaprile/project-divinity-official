using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLightController : BuildingController
{

    private void Start()
    {
        if (insideBuilding)
        {
            if (lights) { lights.SetActive(true); }
            if (roofs.Length > 0) { ControlRoof(false); }
        } 
        else
        {
            if (lights) { lights.SetActive(false); }
            if (roofs.Length > 0) { ControlRoof(true); }
        }
        lateStart = false;
    }

    public override void EnterCollider()
    {
        if (insideBuilding == false)
        {
            if (roofs.Length > 0) { ControlRoof(false); }
            if (lights) { lights.SetActive(true); }
            insideBuilding = true;
        }
    }

    public override void ExitCollider()
    {
        if (insideBuilding == true)
        {
            if (roofs.Length > 0) { ControlRoof(true); }
            if (lights) { lights.SetActive(false); }
            insideBuilding = false;
        }
    }

    private void ControlRoof(bool turnOnRoof)
    {
        if (!roofShadowsOnly)
        {
            foreach (GameObject roof in roofs)
            {
                roof.SetActive(turnOnRoof);
            }
        }
        else
        {
            foreach (GameObject roof in roofs)
            {
                foreach (MeshRenderer child in roof.GetComponentsInChildren<MeshRenderer>())
                {
                    child.shadowCastingMode = turnOnRoof ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                }
            }
        }
    }
}
