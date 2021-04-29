using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMaster : MonoBehaviour
{
    [SerializeField] private Transform RespawnPos;

    private Collider rollingCollider;
    private PlayerMovement playerMovement;
    private PlayerAnimationUpdater animationUpdater;
    private WeaponController weaponController;
    private AbilitiesController abilitiesController;
    private ConsumableController consumableController;
    private PlayerHealth PH;
    private Energy PE;

    private UIController UIControl;
    private CursorLogic CL;


    public static InputManager inputActions;


    private bool PlayerControl = true;


    private void Awake()
    {
        inputActions = new InputManager();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Core functionality
        playerMovement = GetComponent<PlayerMovement>();
        weaponController = GetComponentInChildren<WeaponController>();
        animationUpdater = GetComponentInChildren<PlayerAnimationUpdater>();
        abilitiesController = GetComponentInChildren<AbilitiesController>();
        consumableController = GetComponentInChildren<ConsumableController>();
        PH = GetComponentInChildren<PlayerHealth>();
        PE = GetComponentInChildren<Energy>();
        CL = FindObjectOfType<CursorLogic>();

        UIControl = GameObject.Find("UI").GetComponent<UIController>();
        rollingCollider = GameObject.Find("RollingCollider").GetComponent<Collider>();
    }


    public void Set_PlayerControl(bool set_in) //Independent of UI_Control
    {
        PlayerControl = set_in;
    }


    private float WeaponLockOut = .5f;
    private float current_WeaponLockOut = .5f;
    private void Update() //THE ORDER OF THESE MATTER, UpdatePlayerState needs to be before handle weapon and handleabilites so that a roll can start
    {
        if (PlayerControl && UIControl.current_UI_mode == UI_Mode.Normal)
        {

            // Player movement
            playerMovement.UpdatePlayerState(); //must be above abilites and weapons

            //Ability Controller
            abilitiesController.HandleAbilities(1f);

            // Item Controller
            if(current_WeaponLockOut > 0)
            {
                current_WeaponLockOut -= Time.deltaTime;
            }
            else
            {
                weaponController.HandleWeapon();
            }

            //Consumable Controller
            consumableController.HandleConsumables();

            // Player animation
            animationUpdater.UpdateAnimation();
        }
        else
        {
            current_WeaponLockOut = WeaponLockOut;

            if (UIControl.current_UI_mode == UI_Mode.PauseMenu)
            {
                playerMovement.PauseAimTowardCamera();
                animationUpdater.UpdateAnimationPauseMenu();
            }
            else if (UIControl.current_UI_mode == UI_Mode.DiaMenu)
            {
                /*
                if (!UIControl.npcTransform)
                {
                    return;
                }
                */
                playerMovement.DialogueMenuAimTowards(UIControl.Centerpoint);
            }
        }
    }

    private void FixedUpdate()
    {
        if (PlayerControl && UIControl.current_UI_mode == UI_Mode.Normal)
        {
            transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            playerMovement.Move();
            CL.GetPosition();
            //Vector2 movementInput = inputActions.Movement.Move.ReadValue<Vector2>();
        }
        else
        {
            transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }

        CL.InteractiveObjectFunction(UIControl.current_UI_mode == UI_Mode.Normal); //When not in Normal mode, stop all interactive objects

        rollingCollider.enabled = playerMovement.GetMoveState() == MoveState.Rolling;
    }

    public void PlayerDeath()
    {
        FindObjectOfType<QuestsHolder>().CheckPlayerDeath();
        transform.position = RespawnPos.position;
        PH.OnDeathFunc();
        PE.OnDeathFunc();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
