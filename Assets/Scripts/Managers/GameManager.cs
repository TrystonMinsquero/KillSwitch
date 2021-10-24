using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Min(0)]
    public int _levelStartIndex = 1;
    [Min(1)]
    public int _levelCount = 2;

    public static int levelStartIndex;
    public static int levelCount;
    private static List<int> levelsPlayed = new List<int>();

    public static GameMode gameMode = GameMode.FreeForAll;
    public static bool timedRounds = false;

    //modifers
    public static bool deception; //players more difficult to tell apart
    public static bool starvation; //npcs don't respawn
    public static bool fleeting; //health doesn't come back after takeover

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;

        levelStartIndex = _levelStartIndex;
        levelCount = _levelCount;
        DontDestroyOnLoad(this.gameObject);
    }

    public static void PlayNewLevel()
    {
        int sceneIndex = GetNotRecentRandomSceneIndex();
        Debug.Log(sceneIndex);
        levelsPlayed.Add(sceneIndex);
        SceneManager.LoadScene(sceneIndex);
    }

    private static int GetNotRecentRandomSceneIndex()
    {
        if (levelsPlayed.Count >= levelCount)
            levelsPlayed.Clear();
        List<int> indexes = new List<int>();
        for (int i = levelStartIndex; i < levelCount + levelStartIndex; i++)
            indexes.Add(i);
        while (indexes.Count > 0)
        {
            int randomIndex = indexes[Random.Range(0, indexes.Count)];
            if (!levelsPlayed.Contains(randomIndex))
            {
                return randomIndex;
            }
            else
                indexes.RemoveAt(randomIndex);
        }
        return -1;
    }

}

public enum GameMode
{
    FreeForAll,
    TeamDeathMatch,
    KingOfTheHill,
    Juggernaut,
    Party
}
