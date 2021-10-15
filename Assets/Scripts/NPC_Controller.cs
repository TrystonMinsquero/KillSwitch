using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

public class NPC_Controller : MonoBehaviour
{
    public NPC npc;
    public WeaponHandler weaponHandler;
    public float movementSpeed;
    public GameObject patrolPathObj;
    
    [HideInInspector]
    public PatrolPath patrolPath;
    
    private Patrol patrol;
    private AIPath pathfinder;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        AssignComponents();
        if(patrolPathObj != null)
        {
            AssignPatrolPath(PatrolPath.GeneratePatrolPath(patrolPathObj));
        }
    }

    public void AssignComponents()
    {
        patrol = GetComponent<Patrol>();
        pathfinder = GetComponent<AIPath>();
        weaponHandler = GetComponentInChildren<WeaponHandler>();
        anim = GetComponent<Animator>();
        pathfinder.maxSpeed = movementSpeed;
        weaponHandler.Set();
    }
    public void SwitchVisuals()
    {
        anim.runtimeAnimatorController = npc.aoc;
        weaponHandler.SwitchVisuals();
    }
    public void SwitchVisuals(NPC newNPC)
    {
        anim.runtimeAnimatorController = newNPC.aoc;
        weaponHandler.SwitchVisuals(weaponHandler.weapon);
    }

    public void AssignPatrolPath(PatrolPath patrolPath)
    {
        this.patrolPath = patrolPath;
        patrol.targets = new Transform[patrolPath.patrolpoints.Length];
        for (int i = 0; i < patrolPath.patrolpoints.Length; i++)
            patrol.targets[i] = patrolPath.patrolpoints[i];
    }

    private void Update()
    {
        SetAnimations();
    }

    public void Die()
    {
        NPCManager.KillNPC(this);
    }

    public void SetAnimations()
    {
        if (pathfinder.velocity.sqrMagnitude > .1f)
        {
            anim.Play("Walk");
            weaponHandler.SetAnimations("Walk");
        }

        else
        {
            anim.Play("Idle");
            weaponHandler.SetAnimations("Idle");
        }
    }
}
