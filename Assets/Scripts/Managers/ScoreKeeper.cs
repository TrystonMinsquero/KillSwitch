using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public static ScoreKeeper instance;
    public static List<Score> scores;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;

        OnSceneChange();
        DontDestroyOnLoad(this.gameObject);
    }

    public static void OnSceneChange()
    {
        scores = new List<Score>();//Score[PlayerManager.playerCount];
        ResetScores();
    }

    public static Score GetScore(int playerIndex)
    {
        foreach (Score score in scores)
            if (playerIndex == score.PlayerIndex)
                return score;

        Debug.LogWarning("player index not in scores!");
        return new Score(playerIndex);
    }

    public static void ReigisterDeath(int killer, int victim)
    {
        //If NPC then pass victim number as -1;
        scores[killer].PlayerKills += 1;
        if (victim < 0)
            return;        
        scores[victim].NumDeaths += 1;
        //for(int i = 0; i < scores.Count; i++)
        //{
        //    if (scores[i].PlayerIndex == killer)
        //        scores[i].PlayerKills += 1;
        //    if (scores[i].PlayerIndex == victim)
        //        scores[i].NumDeaths += 1;

        //}
    }

    public static void RegisterTakeOver(int killer, int victim)
    {
        scores[killer].TakeOvers += 1;
        //Assign victim to less than 0 for when taking over NPC 
        if (victim < 0)
        {            
            return;
        }
        scores[victim].NumDeaths += 1;

    }

    public static void ResetScores()
    {
        int j = 0;
        //Simplify previous loop
        scores.ForEach(x =>
        {
            if (PlayerManager.players[j] != null)
            {
                x = new Score(j, 0, 0, 0, 0);
            }
            j++;
        });

        //for(uint i = 0; i < PlayerManager.players.Length; i++)
        //{
        //    if(PlayerManager.players[i] != null)
        //    {
        //        scores[j] = new Score();
        //        scores[j].PlayerIndex = i;
        //        scores[j].PlayerKills = 0;
        //        scores[j].NumDeaths = 0;
        //        scores[j].NpcKills = 0;
        //        scores[j].TakeOvers = 0;
        //    }
        //    j++;
        //}
    }

}

public class Score
{
    //overloading constructors;
    public Score(int pIndx)
    {
        //Don't remove this empty constructor.
        PlayerIndex = pIndx;
    }

    //Constructor to simplify reseting and Initializing scores for each player.
    public Score(int pIndx, int pKills, int numDeaths, int npcKills, int takeOvers)
    {
        PlayerIndex = pIndx;
        PlayerKills = pKills;
        NumDeaths = numDeaths;
        NpcKills = npcKills;
        TakeOvers = takeOvers;
    }

    public int PlayerIndex { get; set; }
    public int PlayerKills { get; set; }
    public int NumDeaths { get; set; }
    public int NpcKills { get; set; }
    public int TakeOvers { get; set; }

}