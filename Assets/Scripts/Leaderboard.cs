using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    public LeaderboardSlot[] slots = new LeaderboardSlot[8];
    public Button doneButton;

    public void ReturnToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void Display()
    {
        doneButton.Select();
        Populate();
        for (int i = 0; i < ScoreKeeper.scores.Length; i++)
            slots[i].Show();
        GetComponent<Canvas>().enabled = true;
    }

    public void Populate()
    {
        int slotCount = 0;
        foreach(PlayerInput player in PlayerManager.players)
        {
            if (PlayerManager.GetIndex(player) >= 0)
            {
                slots[slotCount].Fill(ScoreKeeper.GetScore(PlayerManager.GetIndex(player)));
                slotCount++;
            }
        }
    }
}


