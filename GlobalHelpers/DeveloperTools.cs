using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeveloperTools : MonoBehaviour
{
    Transform playerTrans;
    [SerializeField] TextMeshProUGUI tmText = null;

    Dictionary<string, Vector3> transformLocations = new Dictionary<string, Vector3>();
    
    private void Start()
    {
        playerTrans = GameObject.Find("Player").transform;

        transformLocations.Add("Terry's Farm", new Vector3(2725, 20, -2900));
        transformLocations.Add("Midway City", new Vector3(2140, 20, -2156));
        transformLocations.Add("Wasteland Facility", new Vector3(3151, 20, -2995));
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            UpdateText();
        }

        //Debug.Log(playerTrans.position);
        if (Input.GetKeyDown(KeyCode.F1))
        {
            UpdatePlayerTransform(transformLocations["Terry's Farm"]);
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            UpdatePlayerTransform(transformLocations["Midway City"]);
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            UpdatePlayerTransform(transformLocations["Wasteland Facility"]);
        }
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            UpdatePlayerRunSpeed();
        }
    }

    void UpdatePlayerTransform(Vector3 newTrans)
    {
        playerTrans.position = newTrans;
    }

    void UpdatePlayerRunSpeed()
    {
        PlayerMovement pm = playerTrans.GetComponent<PlayerMovement>();

        if (pm.runSpeedMultiplier < 2.5f)
        {
            pm.runSpeedMultiplier = 5f;
        }
        else
        {
            pm.runSpeedMultiplier = 1.7f;
        }
    }

    void UpdateText()
    {
        tmText.text = "";
        int index = 1;
        foreach (string location in transformLocations.Keys)
        {
            tmText.text = tmText.text + "\n" + "F" + index.ToString() + ": " + location;
            index++;
        }


        string currentRun = "Normal";
        if (playerTrans.GetComponent<PlayerMovement>().runSpeedMultiplier > 2f) { currentRun = "Fast"; }
        tmText.text = tmText.text + "\n" + "F4: Toggle Player Run Speed (Currently " + currentRun + ")";
    }
}
