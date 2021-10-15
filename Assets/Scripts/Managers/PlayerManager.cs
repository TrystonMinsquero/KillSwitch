using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{ 
    public static PlayerManager instance;
    public static PlayerInputManager playerInputManager;
    public static bool inLobby;

    public static PlayerInput[] players = new PlayerInput[8];
    public static int playerCount;

    public PlayerInput[] _players = new PlayerInput[8];



    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
            instance = this;


        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        inLobby = LobbyManager.instance != null;
        //Debug.Log("inLobby = " + inLobby);
    }

    public static int GetIndex(PlayerInput _player)
    {
        if (playerCount <= 0 || _player == null)
            return -1;
        for(int i = 0; i < players.Length; i++)
            if (players[i] == _player)
                return i;
        return -1;
    }

    private void Update()
    {
        _players = players;
    }

    public static bool Contains(PlayerInput _player)
    {
        if (playerCount <= 0)
            return false;
        foreach (PlayerInput player in players)
            if (player == _player)
                return true;
        return false;
    }

    public static void OnSceneChange(bool _inLobby)
    {
        inLobby = _inLobby;
        if (_inLobby)
        {
            Debug.Log("Setting up for Lobby");
            PlayerInputManager.instance.EnableJoining();
            foreach (PlayerInput player in players)
                if(player)
                    player.GetComponent<PlayerUI>().Disable();
        }
        else
        {
            Debug.Log("Setting up for Game");
            PlayerInputManager.instance.DisableJoining();
            foreach (PlayerInput player in players)
                if(player)
                    player.GetComponent<PlayerUI>().Enable();
        }
    }

    public static void SetJoinable(bool enabled)
    {
        if (enabled)
            PlayerInputManager.instance.EnableJoining();
        else
            PlayerInputManager.instance.DisableJoining();
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        AddPlayer(playerInput);
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        Remove(playerInput);
    }

    public static int NextPlayerSlot()
    {
        for (int i = 0; i < players.Length; i++)
            if (players[i] == null)
                return i;
        return -1;
    }

    public static void Join(GameObject newPlayer)
    {
        instance.OnPlayerJoined(newPlayer.GetComponent<PlayerInput>());
    }

    public static void AddPlayer(PlayerInput playerInput)
    {
        
        if (Contains(playerInput))
            return;
        Debug.Log("Trying to Join Player: " + playerInput);
        if (DebugController.debugJoin)
        {
            Debug.Log("Player Joined: " + (NextPlayerSlot() + 1));
            playerInput.name = "Player " + (NextPlayerSlot() + 1);
            playerInput.GetComponent<PlayerUI>().Disable();
            if (!inLobby)
                LevelManager.QueuePlayerToSpawn(playerInput.GetComponent<Player>());
            players[NextPlayerSlot()] = playerInput;
        }
        else if (inLobby)
        {
            if (playerCount < 0 || playerCount >= players.Length || !LobbyManager.canJoin)
            {
                Destroy(playerInput.gameObject);
                return;
            }
            playerInput.GetComponent<PlayerUI>().Disable();
            Debug.Log("Player Joined: Player " + (NextPlayerSlot() + 1));
            playerInput.name = "Player " + (NextPlayerSlot() + 1);
            players[NextPlayerSlot()] = playerInput;
            playerCount++;
        }
    }

    public static void KillAll()
    {
        if (!inLobby)
            foreach (PlayerInput player in players)
                if (player != null)
                    player.GetComponent<Player>().Die();
    }

    public static void Remove(PlayerInput playerInput)
    {
            
        if (inLobby && LobbyManager.canJoin)
        {
            for (int i = 0; i < playerCount; i++)
            {
                if (players[i] != null && players[i] == playerInput)
                {
                    players[i] = null;
                    Debug.Log("Player Left: " + playerInput.name);
                    Destroy(playerInput.gameObject);
                    playerCount--;
                    return;
                }

            }
        }
    }
}
