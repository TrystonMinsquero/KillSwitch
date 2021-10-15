using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public static Player[] players;

    public float _gameTimeInSec = 5 * 60;
    public int _maxPopulation;
    public int _minPopulation;
    public float _spawnDelay;
    public Text timeText;
    public Leaderboard leaderboard;


    public static float gameTime;
    private static int maxPopulation;
    private static int minPopulation;
    private static float spawnDelay;

    public GameObject[] patrolPathsObj;

    private static bool needToSpawn;
    private static float canSpawnPlayerTime;
    private static float gameTimeEnd;
    private static Queue<Player> playersToSpawn;
    private static PatrolPath[] patrolPaths;
    public static Dictionary<PatrolPath, int> patrolPathNPCCount;


    private void Awake()
    {
        if (instance)
        {
            Destroy(instance.gameObject);
            instance = this;
        }
        else
            instance = this;

        gameTime = _gameTimeInSec;
        maxPopulation = _maxPopulation;
        minPopulation = _minPopulation;
        spawnDelay = _spawnDelay;
        canSpawnPlayerTime = Time.time;
        gameTimeEnd = Time.time + gameTime;
        needToSpawn = true;

        playersToSpawn = new Queue<Player>();
        patrolPathNPCCount = new Dictionary<PatrolPath, int>();

}

    // Start is called before the first frame update
    void Start()
    {
        //Gather Patrol Points
        patrolPaths = new PatrolPath[patrolPathsObj.Length];
        for(int i = 0; i < patrolPathsObj.Length; i++)
        {
            patrolPaths[i] = PatrolPath.GeneratePatrolPath(patrolPathsObj[i]);
        }

        foreach (PatrolPath patrolPath in patrolPaths)
            patrolPathNPCCount[patrolPath] = 0;

        foreach (PatrolPath patrolPath in patrolPaths)
            SpawnNPC(patrolPath);
        PlayerManager.OnSceneChange(false);
        ScoreKeeper.OnSceneChange();
        MusicManager.StartMusic(false);

        foreach (PlayerInput playerInput in PlayerManager.players)
            if(playerInput)
                playersToSpawn.Enqueue(playerInput.GetComponent<Player>());
        
    }

    
    //returns all paths in order of number of npc's on path
    private static PatrolPath[] smallestNPCCountPaths()
    {
        PatrolPath[] patrolPathsSorted = new PatrolPath[patrolPathNPCCount.Keys.Count];
        List<KeyValuePair<PatrolPath, int>> myList = patrolPathNPCCount.ToList();
        myList.Sort(
            delegate (KeyValuePair<PatrolPath, int> pair1,
            KeyValuePair<PatrolPath, int> pair2)
            {
                return pair1.Value.CompareTo(pair2.Value);
            }
        );
         
        int i = 0;
        foreach(KeyValuePair<PatrolPath, int> pair in myList)
        {
            patrolPathsSorted[i] = pair.Key;
            //Debug.Log("patrolPathsSorted[" + i + "] = " + pair.Key);
            i++;
        }
        
        return patrolPathsSorted;
    }
      

    public static bool SpawnNPC()
    {
        /*
        foreach (PatrolPath patrolPath in patrolPathNPCCount.Keys)
            Debug.Log(patrolPath + " = " + patrolPathNPCCount[patrolPath]);
        */
        foreach (PatrolPath patrolPath in smallestNPCCountPaths())
        {
            int i = 0;
            foreach (Transform patrolPoint in patrolPath.patrolpoints)
            {
                if (SafeToSpawn(patrolPoint.position, 4))
                {
                    NPCManager.GenerateNPC(patrolPath, i);
                    Debug.Log("Spawned NPC");
                    canSpawnPlayerTime = Time.time + spawnDelay;
                    return true;
                }
                i++;
            }
        }
        return false;
    }


    public static bool SpawnNPC(PatrolPath patrolPath, int index = -1)
    {
        if(index < 0)
        {
            int i = 0;
            foreach (Transform patrolPoint in patrolPath.patrolpoints)
            {
                if (SafeToSpawn(patrolPoint.position, 4))
                {
                    NPCManager.GenerateNPC(patrolPath, i);
                    Debug.Log("Spawned NPC");
                    canSpawnPlayerTime = Time.time + spawnDelay;
                    return true;
                }
                i++;
            }
        }
        else if (SafeToSpawn(patrolPath.patrolpoints[index].position, 4))
        {
            NPCManager.GenerateNPC(patrolPath, index);
            Debug.Log("Spawned NPC");
            canSpawnPlayerTime = Time.time + spawnDelay;
            return true;
        }
        return false;
    }

    public static bool SpawnPlayer(Player player)
    {
        foreach (NPC_Controller npc in NPCManager.NPC_List)
            if (SafeToSpawn(npc.transform.position, 4))
            {
                NPCManager.SpawnPlayerFromNPC(player.GetComponent<PlayerUI>(), npc);
                return true;
            }
        return false;
    }

    public static bool SafeToSpawn(Vector2 position, float range)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, range);
        foreach (Collider2D collider in colliders)
            if (collider.CompareTag("Player") && collider.GetComponent<Player>().enabled || collider.CompareTag("Projectile"))
                return false;
        return true;

    }
    
    public static void QueuePlayerToSpawn(Player player)
    {
        playersToSpawn.Enqueue(player);
    }

    public void EndGame()
    {
        timeText.text = "It's Over!";
        foreach (PlayerInput player in PlayerManager.players)
            if(player != null)
                player.GetComponent<PlayerUI>().Disable();
        leaderboard.Display();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Time.time < gameTimeEnd)
        {
            timeText.text = "Time Left: " + formatTime(gameTimeEnd - Time.time);
            if (playersToSpawn.Count > 0)
            {
                if (SpawnPlayer(playersToSpawn.Peek()))
                    playersToSpawn.Dequeue();
            }

            if (NPCManager.NPC_List.Count < minPopulation)
                SpawnNPC();
            else if (NPCManager.NPC_List.Count < maxPopulation && Time.time > canSpawnPlayerTime)
                SpawnNPC();
        }
        else
        {
            timeText.text = "Last Stand!";
            timeText.color = Color.red;
            if (playersToSpawn.Count + 1 >= PlayerManager.playerCount)
            {
                EndGame();
            }
        }



    }

    public string formatTime(float time)
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
