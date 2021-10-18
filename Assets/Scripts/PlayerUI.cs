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
    private SpriteRenderer sr;
    private Animator anim;
    private AnimatorOverrideController aoc;
    bool debugging;

    private void Awake()
    {
        controls = new Controls();
        DontDestroyOnLoad(this);

    }

    public void AssignComponents()
    {
        playerInput = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
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
        MultipleTargetCamera.AddTarget(transform);
        foreach (Behaviour component in behaviours)
            component.enabled = true;
        if (healthBar == null)
            InstantiateHealthBar();
        healthBar.Enable();
        Player player = GetComponent<Player>();
        player.AssignComponents();
        player.SetWeaponActive(true);
        sr.enabled = true;
        GetComponent<PlayerController>().EnableControls(true);
    }
    public void Disable()
    {
        MultipleTargetCamera.RemoveTarget(transform);
        Player player = GetComponent<Player>();
        player.AssignComponents();
        player.SetWeaponActive(false);
        sr.enabled = false;
        GetComponent<PlayerController>().EnableControls(false);
        foreach (Behaviour component in behaviours)
            component.enabled = false;
        if (healthBar == null)
            InstantiateHealthBar();
        healthBar.Disable();
    }

    public void SetAnimations(string stateName)
    {
        anim.Play(stateName);
    }

    public void SwitchVisuals(NPC_Controller npcc)
    {
        //sr.sprite = npcc.npc.image;
        aoc = npcc.npc.aoc;
        anim.runtimeAnimatorController = aoc;

    }

    public void SwitchVisuals(PlayerUI player)
    {
        sr.sprite = player.sr.sprite;
        aoc = player.aoc;
        anim.runtimeAnimatorController = aoc;
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
