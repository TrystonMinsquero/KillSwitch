using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerManager : MonoBehaviour
{ 
    public static PlayerManager instance;
    public static PlayerInputManager playerInputManager;

    public static bool inLobby;
    public static bool startedInLobby;
    
    public static Player[] players = new Player[8];
    public static int playerCount;

    //used to view players in the editor
    public Player[] playersDisplay;

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
        AddPlayer(playerInput.GetComponent<Player>());
        ScoreKeeper.UpdateScores();
    }

    //Event that gets called when player leaves
    public void OnPlayerLeft(PlayerInput playerInput)
    {
        Remove(playerInput.GetComponent<Player>());
        ScoreKeeper.UpdateScores();
    }

    //kills all players (for debugging)
    public static void KillAll()
    {
        if (!inLobby)
            foreach (Player player in players)
                if (player != null)
                    player.Die();
    }
    
    //kills all players (for debugging)
    public static void DisableAll()
    {
        foreach (Player player in players)
            if (player != null)
                player.GetComponent<PlayerUI>().Disable();
    }

    public static void ResetAll()
    {
        foreach (Player player in players)
            if (player != null)
                player.SetHealth();
        DisableAll();
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
    public static int GetIndex(Player _player)
    {
        if (playerCount <= 0 || _player == null)
            return -1;
        for(int i = 0; i < players.Length; i++)
            if (players[i] == _player)
                return i;
        return -1;
    }

    //returns true if player is already connected to player manager, false otherwise
    public static bool Contains(Player _player)
    {
        if (playerCount <= 0)
            return false;
        foreach (Player player in players)
            if (player == _player)
                return true;
        return false;
    }

    //Sets up the PlayerManager depending on either in game or in lobby
    public static void OnSceneChange(bool _inLobby)
    {
        //if going into the game from the lobby
        if(inLobby && !_inLobby)
        {
            ScoreKeeper.ResetScores();
        }
        inLobby = _inLobby;
        if (_inLobby)
        {
            Debug.Log("Setting up for Lobby");
            PlayerInputManager.instance.EnableJoining();
            foreach (Player player in players)
                if(player)
                    player.GetComponent<PlayerUI>().Disable();
        }
        else
        {
            Debug.Log("Setting up for Game");
            PlayerInputManager.instance.DisableJoining();
            foreach (Player player in players)
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
    private static void AddPlayer(Player player)
    {
        
        if (Contains(player))
            return;

        if(NextPlayerSlot() < 0)
        {
            Destroy(player.gameObject);
            return;
        }

        if(DebugController.debugJoin)
            JoinPlayer(player, !inLobby);
        else if (inLobby)
        {
            if(playerCount < players.Length && LobbyManager.canJoin)
                JoinPlayer(player);
            else
            {
                Destroy(player.gameObject);
                return;
            }
        }

    }
    
    //Properly adds a player to the game
    private static void JoinPlayer(Player player, bool inGame = false)
    {
        player.GetComponent<PlayerUI>().Disable();
        if(inGame)
            LevelManager.QueuePlayerToSpawn(player);
        Debug.Log("Player Joined: Player " + (NextPlayerSlot() + 1));
        player.name = "Player " + (NextPlayerSlot() + 1);
        players[NextPlayerSlot()] = player;
        playerCount++;
        player.transform.parent = instance.transform;
    }

    //Properly removes players from the game
    private static void Remove(Player playerInput)
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
