using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    //static variables
    public static LobbyManager instance;
    public static bool canJoin; //true if in lobby menu (not controls, etc.)

    [Header("UI Draggables")]
    public Canvas lobby;
    public Canvas howToPlay;
    public Button startButton;
    public Button howToPlayButton;
    public Button gotItButton;
    public JoinBox[] joinBoxes;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);


        UpdateJoinBoxes();
        PlayerManager.OnSceneChange(true);
        PlayerManager.startedInLobby = true;

        canJoin = true;
        howToPlayButton.Select();
        MusicManager.StartMusic(true);
    }

    private void Update()
    {
        UpdateJoinBoxes();
    }

    public void UpdateJoinBoxes()
    {
        for(int i = 0; i < joinBoxes.Length; i++)
        {
            if (PlayerManager.players[i] != null && !joinBoxes[i].hasPlayer)
                joinBoxes[i].AddPlayer(PlayerManager.players[i]);
            else if (PlayerManager.players[i] == null && joinBoxes[i].hasPlayer)
                joinBoxes[i].RemovePlayer(PlayerManager.players[i]);
        }
    }

    public void SwitchToHTP()
    {
        canJoin = false;
        howToPlay.enabled = true;
        lobby.enabled = false;
        gotItButton.Select();
    }
    public void SwitchToLobby()
    {
        startButton.Select();
        canJoin = true;
        howToPlay.enabled = false;
        lobby.enabled = true;
    }

    public void StartGame()
    {
        if(PlayerManager.playerCount > 0)
            GameManager.PlayNewLevel();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
