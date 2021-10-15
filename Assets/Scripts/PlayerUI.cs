using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUI : MonoBehaviour
{
    Controls controls;
    [HideInInspector]
    public PlayerInput playerInput;
    public Behaviour[] behaviours;
    

    private void Awake()
    {
        controls = new Controls();
        playerInput = GetComponent<PlayerInput>();
        DontDestroyOnLoad(this);

    }

    public void Enable()
    {
        foreach (Behaviour component in behaviours)
            component.enabled = true;

        Player player = GetComponent<Player>();
        player.AssignComponents();
        player.sr.enabled = true;
        player.weaponHandler.weaponSR.enabled = true;
        player.weaponHandler.flashSR.enabled = true;
        GetComponent<PlayerController>().EnableControls(true);
    }
    public void Disable(bool camEnabled = false)
    {
        Player player = GetComponent<Player>();
        player.AssignComponents();
        player.sr.enabled = false;
        player.weaponHandler.weaponSR.enabled = false;
        player.weaponHandler.flashSR.enabled = false;
        GetComponent<PlayerController>().EnableControls(false);
        foreach (Behaviour component in behaviours)
            if (!(camEnabled && component == GetComponent<PlayerInput>().camera))
                component.enabled = false;
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

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
