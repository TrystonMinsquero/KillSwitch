using UnityEngine;
using UnityEngine.UI;

public class LeaderboardSlot : MonoBehaviour
{
    public Text playerNum;
    public Text kills;
    public Text deaths;



    public void Fill(Score score)
    {
        playerNum.text = "" + (score.PlayerIndex + 1);
        kills.text = "K: " + score.PlayerKills;
        deaths.text = "D: " + score.NumDeaths;
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

}
