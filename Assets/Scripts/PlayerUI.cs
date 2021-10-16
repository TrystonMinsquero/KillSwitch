using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUI : MonoBehaviour
{
    Controls controls;
    [HideInInspector]
    public PlayerInput playerInput;
    public Behaviour[] behaviours;
    public GameObject healthBarPrefab;
    public Transform healthBarStart;
    public HealthBar healthBar;
    bool debugging;

    private void Awake()
    {
        controls = new Controls();
        playerInput = GetComponent<PlayerInput>();
        DontDestroyOnLoad(this);

    }

    public void InstantiateHealthBar()
    {
        healthBar = Instantiate(healthBarPrefab).GetComponentInChildren<HealthBar>();
        if(healthBarStart != null)
            healthBar.offset = healthBarStart.position - transform.position;
        healthBar.SetPosition(transform.position);
    }

    public void Start()
    {
        if (healthBar == null)
            InstantiateHealthBar();
    }

    private void Update()
    {
        if (healthBar != null)
        {
            healthBar.SetPosition(transform.position);
            healthBar.SetHealth(GetComponent<Player>().GetCurrentHealth());
        }
        else
            InstantiateHealthBar();
    }

    public void Enable()
    {
        foreach (Behaviour component in behaviours)
            component.enabled = true;
        if (healthBar == null)
            InstantiateHealthBar();
        healthBar.Enable();
        Player player = GetComponent<Player>();
        player.AssignComponents();
        player.sr.enabled = true;
        player.weaponHandler.weaponSR.enabled = true;
        player.weaponHandler.flashSR.enabled = true;
        GetComponent<PlayerController>().EnableControls(true);
    }
    public void Disable()
    {
        Player player = GetComponent<Player>();
        player.AssignComponents();
        player.sr.enabled = false;
        player.weaponHandler.weaponSR.enabled = false;
        player.weaponHandler.flashSR.enabled = false;
        GetComponent<PlayerController>().EnableControls(false);
        foreach (Behaviour component in behaviours)
            component.enabled = false;
        if (healthBar == null)
            InstantiateHealthBar();
        healthBar.Disable();
    }

    public void Debug(bool debugging)
    {
        this.debugging = debugging;
    }


    //UI Actions
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
    public void OnToggleDebug(InputAction.CallbackContext ctx) => DebugController.OnToggleDebug(ctx.action.triggered, playerInput);
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
