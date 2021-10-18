using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{ 
    public static PlayerManager instance;
    public static PlayerInputManager playerInputManager;

    public static bool inLobby;
    public static bool startedInLobby;
    
    public static PlayerInput[] players = new PlayerInput[8];
    public static int playerCount;

    public PlayerInput[] playersDisplay;



    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;
        playerInputManager = GetComponent<PlayerInputManager>();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        playersDisplay = players;
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
        DebugController.debugJoin = enabled;
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
        if(NextPlayerSlot() < 0)
        {
            Destroy(playerInput.gameObject);
            return;
        }

        if(DebugController.debugJoin )
            JoinPlayer(playerInput, !inLobby);
        else if (inLobby)
        {
            if(playerCount < players.Length && LobbyManager.canJoin)
                JoinPlayer(playerInput);
            else
            {
                Destroy(playerInput.gameObject);
                return;
            }
        }

    }

    private static void JoinPlayer(PlayerInput playerInput, bool inGame = false)
    {
        playerInput.GetComponent<PlayerUI>().Disable();
        if(inGame)
            LevelManager.QueuePlayerToSpawn(playerInput.GetComponent<Player>());
        Debug.Log("Player Joined: Player " + (NextPlayerSlot() + 1));
        playerInput.name = "Player " + (NextPlayerSlot() + 1);
        players[NextPlayerSlot()] = playerInput;
        playerCount++;
        playerInput.transform.parent = instance.transform;
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
