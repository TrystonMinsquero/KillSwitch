using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Player), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    //necessary compononent references
    private Player player;
    private PlayerInput playerInput;
    private Controls controls;

    //input trackers
    Vector2 movementInput;
    Vector2 lookInput;
    bool dashInput;
    bool shootInput;
    bool spawnInput;
    bool isKeyboardAndMouse;

    bool debugging;


    public void SetDebug(bool debugging)
    {
        this.debugging = debugging;
    }

    public void ControlsActive(bool enabled)
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

    public void AssignComponents()
    {
        playerInput = GetComponent<PlayerInput>();
        player = GetComponent<Player>();
    }

    private void Awake()
    {
        controls = new Controls();
        AssignComponents();
    }

    private void FixedUpdate()
    {
        if (debugging)
            return;
        //Move
        player.Move(movementInput);
    }

    private void Update()
    {
        if (debugging)
            return;

        //Look
        if (playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            lookInput = (Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position).normalized;
        }
        player.Look(lookInput);

        //Dash
        if (dashInput)
            player.StartDash();


        //Shoot
        if (shootInput)
            player.Shoot();

    }


    //UI Button Actions

    public void Join()
    {
        if (debugging)
            return;
        if (PlayerManager.inLobby || DebugController.debugJoin)
            PlayerManager.instance.OnPlayerJoined(playerInput);

    }

    public void Leave()
    {
        if (debugging)
            return;
        if (PlayerManager.inLobby && !DebugController.debugMode)
            PlayerManager.instance.OnPlayerLeft(playerInput);
    }

    public void OnJoin(InputAction.CallbackContext ctx) => Join();

    public void OnLeave(InputAction.CallbackContext ctx) => Leave();

    //Gamplay Button Actions

    public void OnMove(InputAction.CallbackContext ctx) => movementInput = ctx.ReadValue<Vector2>();

    public void OnLook(InputAction.CallbackContext ctx) =>  lookInput = ctx.ReadValue<Vector2>(); 

    public void OnDash(InputAction.CallbackContext ctx) => dashInput = ctx.action.triggered;

    public void OnShoot(InputAction.CallbackContext ctx) => shootInput = ctx.action.triggered;

    public void OnSpawn(InputAction.CallbackContext ctx) => spawnInput = ctx.action.triggered;

    public void OnToggleDebug(InputAction.CallbackContext ctx) => DebugController.OnToggleDebug(ctx.action.triggered, player);

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
