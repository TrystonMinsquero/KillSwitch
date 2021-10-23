using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerManager : MonoBehaviour
{ 
    public static PlayerManager instance;
    public static PlayerInputManager playerInputManager;

    public static bool inLobby;
    public static bool startedInLobby;
    
    public static PlayerInput[] players = new PlayerInput[8];
    public static int playerCount;

    //used to view players in the editor
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

    //Event that gets called when input is detected
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        AddPlayer(playerInput);
    }

    //Event that gets called when player leaves
    public void OnPlayerLeft(PlayerInput playerInput)
    {
        Remove(playerInput);
    }

    //kills all players (for debugging)
    public static void KillAll()
    {
        if (!inLobby)
            foreach (PlayerInput player in players)
                if (player != null)
                    player.GetComponent<Player>().Die();
    }

    //Sets "EnableJoining" on the manager to true, allowing new inputs to join
    public static void SetJoinable(bool enabled)
    {
        DebugController.debugJoin = enabled;
        if (enabled)
            PlayerInputManager.instance.EnableJoining();
        else
            PlayerInputManager.instance.DisableJoining();
    }

    //Gets the index of player in the player array, returns -1 if not there
    public static int GetIndex(PlayerInput _player)
    {
        if (playerCount <= 0 || _player == null)
            return -1;
        for(int i = 0; i < players.Length; i++)
            if (players[i] == _player)
                return i;
        return -1;
    }

    //returns true if player is already connected to player manager, false otherwise
    public static bool Contains(PlayerInput _player)
    {
        if (playerCount <= 0)
            return false;
        foreach (PlayerInput player in players)
            if (player == _player)
                return true;
        return false;
    }

    //Sets up the PlayerManager depending on either in game or in lobby
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

    //returns the next empty slot index in the array (so can leave and rejoin),
    //returns -1 if no slots are available
    public static int NextPlayerSlot()
    {
        for (int i = 0; i < players.Length; i++)
            if (players[i] == null)
                return i;
        return -1;
    }

    //Will check coniditons to add a player and add them if conditions hold
    private static void AddPlayer(PlayerInput playerInput)
    {
        
        if (Contains(playerInput))
            return;

        if(NextPlayerSlot() < 0)
        {
            Destroy(playerInput.gameObject);
            return;
        }

        if(DebugController.debugJoin)
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
    
    //Properly adds a player to the game
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

    //Properly removes players from the game
    private static void Remove(PlayerInput playerInput)
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
