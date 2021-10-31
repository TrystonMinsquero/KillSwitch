using UnityEngine;
using UnityEngine.UI;

public class LeaderboardSlot : MonoBehaviour
{
    public Text playerNum;
    public Text playerKills;
    public Text deaths;
    public Text npcKills;
    public Text takeOvers;



    public void Fill(Score score)
    {
        playerNum.text = "" + (score.playerIndex + 1);
        playerKills.text = "" + score.playerKills;
        deaths.text = "" + score.numDeaths;
        npcKills.text = "" + score.npcKills;
        takeOvers.text = "" + score.takeOvers;
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
