using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager instance;

    //Lists
    public string[] patrolPathPopulationsDisplay; //to see in editor only
    private PatrolPath[] patrolPaths;
    private List<NPC_Controller> NPC_List;
    private Dictionary<PatrolPath, List<NPC_Controller>> patrolPathPopulations;

    [Header("Draggables")]
    public GameObject NPCPrefab;
    public GameObject patrolPathsObj;

    [Header("Scriptable Objects")]
    public NPC[] NPCTemplates;
    public Weapon[] WeaponTemplates;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    void Start()
    {

        //Reset/Set Lists
        patrolPaths = GatherPatrolPoints(patrolPathsObj);
        NPC_List = new List<NPC_Controller>();

        patrolPathPopulations = new Dictionary<PatrolPath, List<NPC_Controller>>();
        foreach (PatrolPath patrolPath in patrolPaths)
            patrolPathPopulations[patrolPath] = new List<NPC_Controller>();

        //set population display (for debugging)
        patrolPathPopulationsDisplay = new string[patrolPaths.Length];

        //Added already exisiting NPCs (shouldn't exist)
        NPC_Controller[] alreadyExistingNPCs = GetComponentsInChildren<NPC_Controller>();
        foreach(NPC_Controller npc in alreadyExistingNPCs)
        {
            NPC_List.Add(npc);
        }
    }

    //Useless, but for debugging
    void Update()
    {
        int i = 0;
        foreach (PatrolPath patrolPath in patrolPaths)
        {
            patrolPathPopulationsDisplay[i] = "Patrol Path " + (i + 1) + " Pop = " + patrolPathPopulations[patrolPath].Count;
            i++;
        }

    }

    //returns all patrolPaths with the smallest populations
    public static PatrolPath[] LeastPopulatedPatrolPaths()
    {
        List<PatrolPath> leastPopulatedPaths = new List<PatrolPath>();

        int smallestNPCPopulation = int.MaxValue;;
        foreach (List<NPC_Controller> npcList in instance.patrolPathPopulations.Values)
            if (npcList.Count < smallestNPCPopulation)
                smallestNPCPopulation = npcList.Count;

        foreach (PatrolPath patrolPath in instance.patrolPathPopulations.Keys)
            if (instance.patrolPathPopulations[patrolPath].Count <= smallestNPCPopulation)
                leastPopulatedPaths.Add(patrolPath);

        return leastPopulatedPaths.ToArray();
    }

    //returns all patrolPaths with the largest populations
    public static PatrolPath[] MostPopulatedPatrolPaths()
    {
        List<PatrolPath> mostPopulatedPaths = new List<PatrolPath>();

        int largestNPCPopulation = int.MinValue; ;
        foreach (List<NPC_Controller> npcList in instance.patrolPathPopulations.Values)
            if (npcList.Count > largestNPCPopulation)
                largestNPCPopulation = npcList.Count;

        foreach (PatrolPath patrolPath in instance.patrolPathPopulations.Keys)
            if (instance.patrolPathPopulations[patrolPath].Count >= largestNPCPopulation)
                mostPopulatedPaths.Add(patrolPath);

        return mostPopulatedPaths.ToArray();
    }

    //Spawns in an NPC on a patrol path and returns that npc 
    //will randomly pick a point along patrol path unless specfied
    public static NPC_Controller SpawnNPC(PatrolPath patrolPath, int patrolPointIndex = -1)
    {
        patrolPointIndex = patrolPointIndex < 0 ? Random.Range(0, patrolPath.patrolpoints.Length) : patrolPointIndex;
        return GenerateNPC(patrolPath, patrolPointIndex);
    }

    //Kills an NPC and replaces it with a player
    public static void SpawnPlayerFromNPC(Player player, NPC_Controller npc)
    {
        Debug.Log("Spawning " + player.name);
        player.GetComponent<PlayerUI>().Enable();
        player.TakeOver(npc);
    }

    //Kills all NPCs
    public static void KillALL()
    {
        for (int i = instance.NPC_List.Count - 1; i >= 0; i--)
            KillNPC(instance.NPC_List[i]);
    }

    //Registers an NPC death for the manager
    public static void KillNPC(NPC_Controller npc)
    {
        instance.NPC_List.Remove(npc);
        instance.patrolPathPopulations[npc.patrolPath].Remove(npc);
        Destroy(npc.gameObject);
    }

    public static List<NPC_Controller> GetNPCsOnPath(PatrolPath patrolPath)
    {
        return instance.patrolPathPopulations[patrolPath];
    }

    public static int GetNPCPopulation()
    {
        return instance.NPC_List.Count;
    }

    public static NPC_Controller GetRandomNPC()
    {
        if (instance.NPC_List.Count <= 0)
            return null;
        else
            return instance.NPC_List[Random.Range(0, instance.NPC_List.Count)];
    }

    //Parses through all the patrol paths to 
    private PatrolPath[] GatherPatrolPoints(GameObject _patrolPathsObj)
    {
        //Check for if _patrolPathsObj is null
        _patrolPathsObj = _patrolPathsObj == null ? GameObject.Find("Patrol Paths") : _patrolPathsObj;
        if (patrolPathsObj == null)
            Debug.LogWarning("Patrol Paths Object not found in Scene!");

        //Finding Patrol Path Objects
        List<GameObject> patrolPathObjs = new List<GameObject>();
        foreach (Transform patrolPath in _patrolPathsObj.GetComponentsInChildren<Transform>())
            if (patrolPath.CompareTag("Patrol Path"))
                patrolPathObjs.Add(patrolPath.gameObject);

        //Assigning Patrol Points to PatrolPath from objects
        PatrolPath[] _patrolPaths = new PatrolPath[patrolPathObjs.Count];
        for (int i = 0; i < patrolPathObjs.Count; i++)
            _patrolPaths[i] = PatrolPath.GeneratePatrolPath(patrolPathObjs[i]);

        return _patrolPaths;
    }

    private static NPC_Controller GenerateNPC(PatrolPath patrolPath, int index, NPC npc = null, Weapon weapon = null)
    {
        NPC_Controller newNPC = Instantiate(instance.NPCPrefab, patrolPath.patrolpoints[index].position, Quaternion.identity, instance.transform).GetComponent<NPC_Controller>();

        newNPC.AssignComponents();
        newNPC.AssignPatrolPath(patrolPath);
        //Assign NPC
        if (npc != null)
            newNPC.npc = npc;
        else
            newNPC.npc = instance.NPCTemplates[Random.Range(0, instance.NPCTemplates.Length)];

        //Assign Weapon
        if (weapon != null && npc.hasWeapon)
            newNPC.weaponHandler.weapon = weapon;
        else if (newNPC.npc.hasWeapon)
            newNPC.weaponHandler.weapon = instance.WeaponTemplates[Random.Range(0, instance.WeaponTemplates.Length)];
        else
            newNPC.weaponHandler.weapon = null;

        newNPC.AssignComponents();
        newNPC.SwitchVisuals();
        instance.NPC_List.Add(newNPC);
        instance.patrolPathPopulations[patrolPath].Add(newNPC);
        return newNPC;
    }


}
