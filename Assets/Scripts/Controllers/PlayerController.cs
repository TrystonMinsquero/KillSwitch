using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public Player player;
    private PlayerInput playerInput;
    [HideInInspector]
    Controls controls;
    Vector2 movementInput;
    Vector2 lookInput;
    bool dashInput;
    bool shootInput;
    bool spawnInput;

    private void Awake()
    {
        controls = new Controls();
        AssignComponents();

    }
    public void AssignComponents()
    {
        playerInput = GetComponent<PlayerInput>();
        player = GetComponent<Player>();
    }
    

    private void FixedUpdate()
    {
        if (DebugController.debugMode)
            return;
        //Move
        player.Move(movementInput);
    }

    private void Update()
    {
        if (DebugController.debugMode)
            return;
        //Look
        player.Look(lookInput);

        //Dash
        if (dashInput)
            player.StartDash();


        //Shoot
        if (shootInput)
            player.Shoot();     

    }

    public void EnableControls(bool enabled)
    {
        if (enabled)
        {
            GetComponent<PlayerInput>().actions.FindActionMap("UI").Disable();
            GetComponent<PlayerInput>().actions.FindActionMap("Gameplay").Enable();            
        }
        else
        {
            GetComponent<PlayerInput>().actions.FindActionMap("Gameplay").Disable();
            GetComponent<PlayerInput>().actions.FindActionMap("UI").Enable();
        }
    }

    //UI Actions
    public void Join()
    {
        if (PlayerManager.inLobby || DebugController.debugMode)
            PlayerManager.instance.OnPlayerJoined(playerInput);

    }

    public void Leave()
    {
        if (PlayerManager.inLobby && !DebugController.debugMode)
            PlayerManager.instance.OnPlayerLeft(playerInput);
    }

    public void OnJoin(InputAction.CallbackContext ctx) => Join();
    public void OnLeave(InputAction.CallbackContext ctx) => Leave();


    //Get Inputs
    
    public void OnMove(InputAction.CallbackContext ctx) => movementInput = ctx.ReadValue<Vector2>();

    public void OnLook(InputAction.CallbackContext ctx) => lookInput = ctx.ReadValue<Vector2>();

    public void OnDash(InputAction.CallbackContext ctx) => dashInput = ctx.action.triggered;

    public void OnShoot(InputAction.CallbackContext ctx) => shootInput = ctx.action.triggered;

    public void OnSpawn(InputAction.CallbackContext ctx) => spawnInput = ctx.action.triggered;

    public void OnToggleDebug(InputAction.CallbackContext ctx) => DebugController.OnToggleDebug(ctx.action.triggered);

    public void OnReturn(InputAction.CallbackContext ctx) => DebugController.OnReturn(ctx.action.triggered);

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
