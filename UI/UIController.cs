using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class UIController : MonoBehaviour
{
    public GameObject Menu;
    public GameObject HUD;
    public GameObject Map;
    public GameObject InteractiveObjectMenu;
    public GameObject LevelUpMenu;
    public GameObject DialogueMenu;
    public CinemachineBrain cinemachineBrain;
    [SerializeField] private Transform EXPUI;

    private Transform Player;


    //External Use
    public UI_Mode current_UI_mode = UI_Mode.Normal;
    public Vector3 Centerpoint;
    //External Use

    private List<Transform> npcTransforms = new List<Transform>();

    private float originalTimeScale;
    private bool mapOpen = false;

    private PlayerStats playerStats;
    private PlayerAnimationUpdater playerAnimationUpdater;
    private CameraStateController camStateController;
    private Transform UIparent;
    private Transform originalParent;

    // Start is called before the first frame update
    void Start()
    {
        Menu.SetActive(false);
        originalTimeScale = Time.timeScale;

        Player = GameObject.Find("Player").transform;
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
        playerAnimationUpdater = playerStats.GetComponentInChildren<PlayerAnimationUpdater>();
        UIparent = transform.Find("UIParent");
        camStateController = FindObjectOfType<CameraStateController>();
        originalParent = EXPUI.parent;
    }


    // Update is called once per frame
    void Update()
    {
        // TODO: Handle Input through InputManager and not direct key references
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (current_UI_mode == UI_Mode.InteractiveMenu)
            {
                InteractiveObjectMenu.GetComponent<InteractiveObjectMenuUI>().DisablePanel();
                current_UI_mode = UI_Mode.Normal;
            }
            else if (current_UI_mode == UI_Mode.PauseMenu)
            {
                Menu.SetActive(false);
                Unpaused();
                UnpauseEXPBar();
            }
            else if(current_UI_mode == UI_Mode.Normal)
            {
                Menu.SetActive(true);
                Paused();
                PauseEXPBar();
            }
        }


        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (current_UI_mode == UI_Mode.Normal)
            {
                EnableDisableMap(!mapOpen);
            }
            else if (current_UI_mode == UI_Mode.PauseMenu)
            {
                Menu.GetComponent<MenuController>().WorldEnable();
            }
        }



        if(current_UI_mode == UI_Mode.DiaMenu)
        {
            Centerpoint = Player.position;

            for (int i = 0; i < npcTransforms.Count; ++i)
            {
                Centerpoint += npcTransforms[i].position;
            }
            Centerpoint /= (npcTransforms.Count + 1);

            for (int i = 0; i < npcTransforms.Count; ++i)
            {
                Vector3 direction = (Centerpoint - npcTransforms[i].position);
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                npcTransforms[i].rotation = Quaternion.RotateTowards(npcTransforms[i].rotation, Quaternion.Euler(new Vector3(0, angle, 0)), 270 * Time.unscaledDeltaTime);
            }
        }
    }

    public void EnableDisableMap(bool enable)
    {
        if (enable)
        {
            Map.SetActive(true);
            Map.GetComponentInChildren<Animator>().Play("In");
            mapOpen = true;
        }
        else if (Map.activeSelf)
        {
            mapOpen = false;
            Map.GetComponentInChildren<Animator>().Play("Out");
            StartCoroutine(TurnOffPanel(Map));
        }
    }

    public void OpenInteractiveMenu(GameObject container)
    {
        Map.SetActive(false);
        playerAnimationUpdater.PlayAnimation("idle");
        current_UI_mode = UI_Mode.InteractiveMenu;
        InteractiveObjectMenu.SetActive(true);
        InteractiveObjectMenu.GetComponent<InteractiveObjectMenuUI>().Refresh(container);
    }

    public void LevelUpMenuBool(bool open)
    {
        if (open)
        {
            Paused();
            PauseEXPBar();
            Map.SetActive(false);
            current_UI_mode = UI_Mode.LevelMenu;
            LevelUpMenu.SetActive(true);
            LevelUpMenu.GetComponent<Animator>().Play("Modal Window In");
        }
        else
        {
            Unpaused();
            UnpauseEXPBar();
            current_UI_mode = UI_Mode.Normal;
            LevelUpMenu.GetComponent<Animator>().Play("Modal Window Out");
            StartCoroutine(TurnOffPanel(LevelUpMenu));
        }
    }

    public void ReturnMapLocation(Transform mapRect)
    {
        StartCoroutine(ReturnMapLocationCoroutine(mapRect));
    }

    private IEnumerator TurnOffPanel(GameObject panel)
    {
        yield return new WaitForSecondsRealtime(0.3f);

        if (!Map || !mapOpen)
        {
            panel.SetActive(false);
        }
    }

    private IEnumerator ReturnMapLocationCoroutine(Transform mapRect)
    {
        yield return new WaitForSecondsRealtime(0.3f);

        if (!mapOpen)
        {
            mapRect.localPosition = new Vector3(960f, 597.5f);
        }
    }

    public void add_dia_npc(Transform npc)
    {
        if (!npcTransforms.Contains(npc))
        {
            npcTransforms.Add(npc);
        }
    }


    Queue<Transform> DiaQueue = new Queue<Transform>();
    public void DialogueMenuBool(Transform DiaData = null) //WARNING Order is hyper sensitive here
    {
        if (DiaData != null)
        {
            HumanoidMaster TempMaster = DiaData.GetComponentInParent<HumanoidMaster>();

            if (TempMaster)
            {
                TempMaster.SetMovementLocked(true);
                npcTransforms.Add(TempMaster.transform);

            }

            if (current_UI_mode == UI_Mode.DiaMenu)
            {
                DiaQueue.Enqueue(DiaData);
                return;
            }


            current_UI_mode = UI_Mode.DiaMenu;
            playerAnimationUpdater.PlayAnimation("idle");

            Map.SetActive(false);
            HUD.GetComponent<CanvasGroup>().alpha = 0;
            DialogueMenu.SetActive(true);
            DialogueMenu.GetComponent<Animator>().Play("Panel In");
            DialogueMenu.GetComponent<DiaParent>().SetupDia(DiaData);

            camStateController.SetCamState(CameraStateController.CameraState.DialogueMenuCam);
        }
        else
        {
            foreach(Transform trans in npcTransforms)
            {
                trans.GetComponent<HumanoidMaster>().SetMovementLocked(false);
            }
            npcTransforms.Clear();

            camStateController.RevertCamToPreviousState();
            current_UI_mode = UI_Mode.Normal;

            if (DiaQueue.Count > 0)
            {
                DialogueMenuBool(DiaQueue.Dequeue());
            }
            else
            {

                DialogueMenu.GetComponent<Animator>().Play("Panel Out");
                StartCoroutine(TurnOffPanel(DialogueMenu));
                HUD.GetComponent<CanvasGroup>().alpha = 1;
            }

        }
    }

    void Paused()
    {
        cinemachineBrain.m_IgnoreTimeScale = true;
        cinemachineBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
        playerAnimationUpdater.SetUpdateMode(AnimatorUpdateMode.UnscaledTime);
        playerAnimationUpdater.PlayAnimation("idle");
        camStateController.SetCamState(CameraStateController.CameraState.PauseMenuCam);
        current_UI_mode = UI_Mode.PauseMenu; 


        playerAnimationUpdater.GetComponentInParent<Rigidbody>().velocity = Vector3.zero;
        playerAnimationUpdater.GetComponentInParent<Rigidbody>().angularVelocity = Vector3.zero;
        Time.timeScale = 0f;
        HUD.GetComponent<CanvasGroup>().alpha = 0;
    }

    void Unpaused()
    {
        cinemachineBrain.m_IgnoreTimeScale = false;
        cinemachineBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.SmartUpdate;
        playerAnimationUpdater.SetUpdateMode(AnimatorUpdateMode.Normal);
        playerAnimationUpdater.PlayAnimation("idle");
        camStateController.RevertCamToPreviousState();
        current_UI_mode = UI_Mode.Normal;

        Time.timeScale = originalTimeScale;
        HUD.GetComponent<CanvasGroup>().alpha = 1;
    }

    private void PauseEXPBar()
    {
        EXPUI.gameObject.SetActive(true);
        EXPUI.SetParent(UIparent);
        EXPUI.transform.position = UIparent.transform.position;
        //EXPUI.transform.localScale = new Vector3(1f, 1f, 1f);
        EXPUI.Find("EXPNum").gameObject.SetActive(false);
    }

    private void UnpauseEXPBar()
    {
        EXPUI.gameObject.SetActive(false);
        EXPUI.SetParent(originalParent);
        EXPUI.transform.position = originalParent.transform.position;
        //EXPUI.transform.localScale = new Vector3(1f, 1f, 1f);
        EXPUI.Find("EXPNum").gameObject.SetActive(true);
    }
}
