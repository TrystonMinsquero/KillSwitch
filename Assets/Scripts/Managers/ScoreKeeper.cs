using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public static ScoreKeeper instance;
    private static Score[] scores;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;

        ResetScores(); //will get called by player manager later in case this is built first
        DontDestroyOnLoad(this.gameObject);
    }

    //returns the score of the player if in scores, otherwise creates a new Score with player index
    public static Score GetScore(Player player)
    {
        if (player == null)
            Debug.LogError("Can't get score of no player");

        int index = GetIndex(player);
        if(scores != null && index >= 0 && index < scores.Length)
            return scores[index];

        Debug.LogWarning(player.name + " not in scores!");
        return new Score(PlayerManager.GetIndex(player));
    }

    public static void ReigisterDeath(Player victim, Player killer)
    {

        //victim killed themselves
        scores[GetIndex(victim)].numDeaths += 1;
        if (killer == null || victim == killer)
            return;
        scores[GetIndex(killer)].playerKills += 1;
    }

    public static void RegisterTakeOver(Player killer)
    {
        if (killer != null)
            scores[GetIndex(killer)].takeOvers += 1;

    }

    public static void ResetScores()
    {
        scores = new Score[PlayerManager.playerCount];
        int j = 0;

        ////Simplify previous loop
        //scores.ForEach(x =>
        //{
        //    if (PlayerManager.players[j] != null)
        //    {
        //        x = new Score(j, 0, 0, 0, 0);
        //    }
        //    j++;
        //});

        //has to parse through all of player manager players because they're might be holes in the array
        //i.e. players = {player 1, null, null, player 4} then scores should only have a size of 2
        for (int i = 0; i < PlayerManager.players.Length; i++)
        {
            if (PlayerManager.players[i] != null)
            {
                scores[j] = new Score(i);
                j++;
            }
        }
        //Debug.Log("Scores:");
        //foreach (Score score in scores)
        //    Debug.Log(score.playerIndex);
    }

    //for when a player leaves or joins
    public static void UpdateScores()
    {
        Score[] updatedScores = new Score[PlayerManager.playerCount];
        foreach (Player player in PlayerManager.players)
            if (player != null)
                updatedScores[GetIndex(player)] = GetScore(player);
        scores = updatedScores;
            
    }

    public static void RegisterNPCKill(Player player)
    {
        if(player != null)
            scores[GetIndex(player)].npcKills += 1;
    }

    //gets the theoretical index of the player in scores
    private static int GetIndex(Player player)
    {
        int index = 0;
        foreach (Player _player in PlayerManager.players)
            if (player != null)
            {
                if(player == _player)
                    return index;
                index++;
            }
        return -1;
    }

}

public struct Score
{

    public int playerIndex;
    public int playerKills;
    public int numDeaths;
    public int npcKills;
    public int takeOvers; 

    //Constructor to simplify reseting and Initializing scores for each player.
    public Score(int pIndx, int pKills = 0, int numDeaths = 0, int npcKills = 0, int takeOvers = 0)
    {
        this.playerIndex = pIndx;
        this.playerKills = pKills;
        this.numDeaths = numDeaths;
        this.npcKills = npcKills;
        this.takeOvers = takeOvers;
    }

}