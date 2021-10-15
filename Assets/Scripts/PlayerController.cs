using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Camera cam;

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
        cam = GetComponentInChildren<Camera>();
    }
    

    private void FixedUpdate()
    {
        //Move
        player.Move(movementInput);
    }

    private void Update()
    {
        //Look
        player.Look(lookInput);

        //Dash
        if (dashInput)
            player.StartDash();


        //Shoot
        if (shootInput)
            player.Shoot();



        cam.transform.rotation = Quaternion.Euler(Vector3.zero);
        

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
        if (PlayerManager.inLobby)
            PlayerManager.instance.OnPlayerJoined(playerInput);

    }

    public void Leave()
    {
        if (PlayerManager.inLobby)
            PlayerManager.instance.OnPlayerLeft(playerInput);
    }

    public void OnJoin(InputAction.CallbackContext ctx) => Join();
    public void OnLeave(InputAction.CallbackContext ctx) => Leave();


    //Get Inputs
    
    public void OnMove(InputAction.CallbackContext ctx) => movementInput = ctx.ReadValue<Vector2>();

    public void OnLook(InputAction.CallbackContext ctx) => lookInput = ctx.ReadValue<Vector2>();

    public void OnDash(InputAction.CallbackContext ctx) => dashInput = ctx.action.triggered;

    public void OnShoot(InputAction.CallbackContext ctx) => shootInput = ctx.action.triggered;

    public void OnSpawnt(InputAction.CallbackContext ctx) => spawnInput = ctx.action.triggered;

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
