using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    [SerializeField] public GameObject[] roofs = null;
    [SerializeField] public GameObject lights = null;
    [SerializeField] private GameObject[] interiorObjectsToDisableWhenOutside = null;
    [SerializeField] private GameObject[] interiorObjectsToDisableWhenInside = null;
    [SerializeField] private List<AudioClip> AmbientAudio = null;

    [SerializeField] private bool lightsAsChildren = false;
    [SerializeField] protected bool roofShadowsOnly = false;
    public bool insideBuilding = false;
    public int currentFloor = 0;
    [SerializeField] private float camDistance = 0f;
    
    private NonDiegeticController AudioControl;
    private CameraEnvironmentController controller;

    protected bool lateStart = true;

    private void Start()
    {
        AudioControl = GameObject.Find("Non Diegetic Audio").GetComponent<NonDiegeticController>();
        controller = FindObjectOfType<CameraEnvironmentController>();

        foreach (GameObject roof in roofs)
        {
            if (roof && roof.activeSelf == false)
            {
                roof.SetActive(true);
            }
        }
    }

    private void LateUpdate()
    {
        if (lateStart) {
            lateStart = false;
            if (insideBuilding)
            {
                insideBuilding = false;
                EnterCollider();
            }
            else
            {
                if (lights) { ControlLights(false); }
                if (roofs.Length > 0) { ControlRoof(true); }
                if (interiorObjectsToDisableWhenOutside.Length > 0) { EnableDisableInterior(false, interiorObjectsToDisableWhenOutside); }
                if (interiorObjectsToDisableWhenInside.Length > 0) { EnableDisableInterior(true, interiorObjectsToDisableWhenInside); }
            }
        }
    }

    public virtual void EnterCollider()
    {
        if (insideBuilding == false)
        {
            if (roofs.Length > 0) { ControlRoof(false); }
            if (lights) { ControlLights(true); }
            if (interiorObjectsToDisableWhenOutside.Length > 0) { EnableDisableInterior(true, interiorObjectsToDisableWhenOutside); }
            if (interiorObjectsToDisableWhenInside.Length > 0) { EnableDisableInterior(true, interiorObjectsToDisableWhenInside); }
            StartCoroutine(controller.EnterBuilding(!roofShadowsOnly, camDistance));
            insideBuilding = true;
            if (AmbientAudio.Count > 0)
            {
                AudioControl.ChangeAudioSpecific(AmbientAudio);
            }
        }
    }

    public virtual void ExitCollider()
    {
        if (insideBuilding == true)
        {
            if (roofs.Length > 0) { ControlRoof(true); }
            if (lights) { ControlLights(false); }
            if (interiorObjectsToDisableWhenOutside.Length > 0) { EnableDisableInterior(false, interiorObjectsToDisableWhenOutside); }
            if (interiorObjectsToDisableWhenInside.Length > 0) { EnableDisableInterior(true, interiorObjectsToDisableWhenInside); }
            StartCoroutine(controller.ExitBuilding(!roofShadowsOnly));
            insideBuilding = false;
            if (AmbientAudio.Count > 0)
            {
                AudioControl.ChangeAudioGeneral();
            }
        }
    }

    private void ControlLights(bool turnOnLights)
    {
        if (!lightsAsChildren)
        {
            lights.SetActive(turnOnLights);
        }
        else
        {
            foreach (Transform child in lights.transform)
            {
                if (child.GetComponentInChildren<Light>())
                {
                    child.GetComponentInChildren<Light>().enabled = turnOnLights;
                }
            }
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

    public void IncreaseFloor(int floorNum)
    {
        if (currentFloor >= floorNum)
        {
            return;
        }

        currentFloor++;

        if (!roofShadowsOnly)
        {
            roofs[currentFloor - 1].SetActive(true);
        }
        else
        {
            GameObject floorRoofs = roofs[currentFloor - 1];
            foreach (MeshRenderer child in floorRoofs.GetComponentsInChildren<MeshRenderer>())
            {
                child.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
        }
    }

    public void DecreaseFloor(int floorNum)
    {
        if (currentFloor < floorNum)
        {
            return;
        }

        currentFloor--;

        if (!roofShadowsOnly)
        {
            roofs[currentFloor].SetActive(false);
        }
        else
        {
            foreach (MeshRenderer child in roofs[currentFloor].GetComponentsInChildren<MeshRenderer>())
            {
                child.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
        }
    }

    private void EnableDisableInterior(bool enable, GameObject[] objects)
    {
        foreach (GameObject interior in objects)
        {
            interior.SetActive(enable);
        }
    }
}
