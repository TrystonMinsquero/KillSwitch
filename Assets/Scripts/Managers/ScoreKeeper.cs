using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public static ScoreKeeper instance;
    public static Score[] scores;

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
        scores = new Score[PlayerManager.playerCount];
        ResetScores();
    }

    public static Score GetScore(int playerIndex)
    {
        foreach (Score score in scores)
            if (playerIndex == score.playerIndex)
                return score;

        Debug.LogWarning("player index not in scores!");
        return new Score();
    }

    public static void ReigisterDeath(int killer, int victim)
    {
        if (victim < 0)
            return;
        for(int i = 0; i < scores.Length; i++)
        {
            if (scores[i].playerIndex == killer)
                scores[i].playerKills += 1;
            if (scores[i].playerIndex == victim)
                scores[i].timesDied += 1;

        }
    }

    public static void ResetScores()
    {
        int j = 0;
        for(uint i = 0; i < PlayerManager.players.Length; i++)
        {
            if(PlayerManager.players[i] != null)
            {
                scores[j] = new Score();
                scores[j].playerIndex = i;
                scores[j].playerKills = 0;
                scores[j].timesDied = 0;
                scores[j].npcKills = 0;
                scores[j].takeOvers = 0;
            }
            j++;
        }
            
    }


}

public struct Score
{
    public uint playerIndex;
    public uint playerKills;
    public uint timesDied;
    public uint npcKills;
    public uint takeOvers;


}