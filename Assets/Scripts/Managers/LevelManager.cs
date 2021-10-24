using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public static Player[] players;

    [Header("Level Attributes")]
    public float _gameTimeInSec = 5 * 60;
    public int _maxPopulation = 18;
    public int _minPopulation = 6;
    public float _spawnDelay = 4;
    public float saftyCheckRange = 4f;

    [Header("UI Draggables")]
    public Text timeText;
    public Leaderboard leaderboard;

    public static float gameTime;
    private static int maxPopulation;
    private static int minPopulation;
    private static float spawnDelay;

    private bool needToSpawn = true;
    private static float canSpawnNPCTime;
    private static float gameTimeEnd;
    private static Queue<Player> playersToSpawn;

    private void Awake()
    {
        if (instance)
            Destroy(gameObject);
        else
            instance = this;

        if(_gameTimeInSec > 0)
            gameTime = _gameTimeInSec;
        SetGameTime(gameTime);

        maxPopulation = _maxPopulation;
        minPopulation = _minPopulation;
        spawnDelay = _spawnDelay;
        canSpawnNPCTime = Time.time;

        playersToSpawn = new Queue<Player>();

    }

    void Start()
    {
        PlayerManager.OnSceneChange(false);
        if (!PlayerManager.startedInLobby)
            PlayerManager.SetJoinable(true);
        ScoreKeeper.OnSceneChange();
        MusicManager.StartMusic(false);

        //Adds all the players to the spawn queue
        foreach (PlayerInput playerInput in PlayerManager.players)
            if(playerInput)
                QueuePlayerToSpawn(playerInput.GetComponent<Player>());
    }

    void Update()
    {
        //During Game Time
        if (Time.time < gameTimeEnd)
        {
            timeText.text = "Time Left: " + FormatTime(gameTimeEnd - Time.time);
            //Attempt to spawn in players if needed
            if (playersToSpawn.Count > 0 && needToSpawn)
            {
                if (TrySpawnPlayer(playersToSpawn.Peek()))
                    playersToSpawn.Dequeue();
            }
            //Will spawn NPCs rapdily if below min population, otherwise will delay spawning
            if (NPCManager.GetNPCPopulation() < minPopulation)
                TrySpawnNPC();
            else if (NPCManager.GetNPCPopulation() < maxPopulation && Time.time > canSpawnNPCTime)
                TrySpawnNPC();
        }
        //After Game Time
        else
        {
            //Stop spawning NPCs and Players
            timeText.text = "Last Stand!";
            timeText.color = Color.red;
            if (playersToSpawn.Count + 1 >= PlayerManager.playerCount)
            {
                EndLevel();
            }
        }

    }

    public static void SetGameTime(float time)
    {
        gameTime = time;
        gameTimeEnd = Time.time + time;
    }

    public static void QueuePlayerToSpawn(Player player)
    {
        playersToSpawn.Enqueue(player);
    }

    //Attempts to spawn an NPC at all the least populated patrol paths, return true if spawned
    private bool TrySpawnNPC()
    {
        //Go through all least populated patrol paths randomly
        List<PatrolPath> patrolPaths = NPCManager.LeastPopulatedPatrolPaths().ToList();
        while(patrolPaths.Count > 0)
        {
            //Randomly Selects Patrol Path to look through
            int pathIndex = Random.Range(0, patrolPaths.Count);
            PatrolPath patrolPath = patrolPaths[pathIndex];

            //Randomly selects a patrol point in the path and trys to spawn there, continues
            //for the rest of patrol points
            List<Transform> patrolPoints = patrolPath.patrolpoints.ToList<Transform>();
            while (patrolPoints.Count > 0)
            {
                int pointIndex = Random.Range(0, patrolPoints.Count);
                Transform patrolPoint = patrolPoints[pointIndex];
                if (SafeToSpawn(patrolPoint.position, saftyCheckRange))
                {
                    NPCManager.SpawnNPC(patrolPath, pointIndex);
                    canSpawnNPCTime = Time.time + spawnDelay;
                    return true;
                }
                patrolPoints.RemoveAt(pointIndex);
            }
            patrolPaths.RemoveAt(pathIndex);

        }
        return false;
    }

    //Attempts to spawn player on a random npc that is patrolling along the most populated patrol paths
    //returns true if spawned
    private bool TrySpawnPlayer(Player player)
    {
        //Go through all most populated patrol paths randomly
        List<PatrolPath> patrolPaths = NPCManager.MostPopulatedPatrolPaths().ToList();
        while (patrolPaths.Count > 0)
        {
            //Randomly selects Patrol Path to look through
            int pathIndex = Random.Range(0, patrolPaths.Count);
            PatrolPath patrolPath = patrolPaths[pathIndex];

            //Randomly selects an NPC in the path and trys to spawn there, will repeat until
            //the player spawned or ran out of NPCs on path
            List<NPC_Controller> NPCsOnPath = NPCManager.GetNPCsOnPath(patrolPath);
            while (NPCsOnPath.Count > 0)
            {
                int npcIndex = Random.Range(0, NPCsOnPath.Count);
                NPC_Controller npc = NPCsOnPath[npcIndex];
                if (SafeToSpawn(npc.transform.position, saftyCheckRange))
                {
                    NPCManager.SpawnPlayerFromNPC(player, npc);
                    return true;
                }
                NPCsOnPath.RemoveAt(npcIndex);
            }
            patrolPaths.RemoveAt(pathIndex);
        }
        return false;
    }

    //Checks to see if there is a player or projectile in a certain range of a position
    private static bool SafeToSpawn(Vector2 position, float range)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, range);
        foreach (Collider2D collider in colliders)
            if (collider.CompareTag("Player") && collider.GetComponent<Player>().enabled || collider.CompareTag("Projectile"))
                return false;
        return true;

    }
    
    private void EndLevel()
    {
        timeText.text = "It's Over!";
        PlayerManager.ResetAll();
        leaderboard.Display();
    }

    private string FormatTime(float time)
    {
        string deci = ".";
        string sec = "";
        string min = "";
        if ((int)((time - (int)time) * 100) < 10)
            deci += "0";
        deci += +(int)((time - (int)time) * 100);
        if (time < 60)
        {
            if (time < 10)
                sec += "0";
            sec += (int)(time);
            return "" + sec + deci;
        }
        else
        {
            if (time % 60 < 10)
                sec += "0";
            if ((int)(time % 60) == 0)
                sec += "0";
            sec += (int)(time % 60);

            min = "0" + (int)(time) / 60;
            return min + ":" + sec;
        }


    }
}
