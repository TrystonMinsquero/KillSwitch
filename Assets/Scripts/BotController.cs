using Pathfinding;
using UnityEngine;
using System;

public class BotController : MonoBehaviour
{
    StateMachine stateMachine;
    Player player;
    AIPath aiPath;
    AIDestinationSetter pathfinder;
    public CurrentState currentState;

    public Transform target;
    public Rigidbody2D rb;
    private readonly int lowTimeNum = 8;

    void Start()
    {
        player = GetComponent<Player>();
        aiPath = GetComponent<AIPath>();
        pathfinder = GetComponent<AIDestinationSetter>();
        rb = GetComponent<Rigidbody2D>();

        aiPath.maxSpeed = player.movementSpeedInit;
        pathfinder.enabled = false;
        target = null;

        SetUpStateMachine();
    }

    private void SetUpStateMachine()
    {
        stateMachine = new StateMachine();

        var findNPC = new FindTag(this, "NPC");
        var pathToNPC = new PathToTarget(this);
        var dashAttackNPC = new DashAttack(this, player);
        var findPlayer = new FindTag(this, "Player");
        var findPlayerClose = new FindTag(this, "Player");
        var pathToPlayer = new PathToTarget(this);
        var dashAttackPlayer = new DashAttack(this, player);
        var weaponAttackPlayer = new WeaponAttack(this, player);

        stateMachine.AddAnyTransition(findNPC, timeLowOrNoWeapAndNoNPC());
        stateMachine.AddTransition(findNPC, pathToNPC, NPCFound());
        stateMachine.AddTransition(pathToNPC, dashAttackNPC, targetInDashRange());
        stateMachine.AddTransition(dashAttackNPC, pathToNPC, targetNotInDashRange());
        stateMachine.AddTransition(dashAttackNPC, findPlayer, timeNotLowOrHasWeapon());
        stateMachine.AddTransition(findPlayer, pathToPlayer, playerFound());
        stateMachine.AddTransition(pathToPlayer, dashAttackPlayer, targetInDashRange());
        stateMachine.AddTransition(pathToPlayer, weaponAttackPlayer, targetInWeaponRange());
        stateMachine.AddTransition(dashAttackPlayer, pathToPlayer, targetNotInDashRange());
        stateMachine.AddAnyTransition(findPlayerClose, playerInDashRange());
        stateMachine.AddTransition(findPlayerClose, dashAttackPlayer, playerFound());

        Func<bool> timeLowOrNoWeapAndNoNPC() => () => (TimeLow() || !HasWeapon()) && !HaveNPCTarget() && !playerInRange(player.dashDistance);
        Func<bool> timeNotLowOrHasWeapon() => () => !TimeLow() || HasWeapon();
        Func<bool> NPCFound() => () => HaveNPCTarget();
        Func<bool> targetInDashRange() => () => TargetInRange(player.dashDistance);
        Func<bool> targetNotInDashRange() => () => !TargetInRange(player.dashDistance);
        Func<bool> playerFound() => () => HavePlayerTarget();
        Func<bool> targetInWeaponRange() => () => HasWeapon() && TargetInRange(player.weaponHandler.weapon.range);
        Func<bool> playerInDashRange() => () => playerInRange(player.dashDistance);

        stateMachine.SetState(findNPC);
       
        
    }
    private bool TimeLow() { return player.timeRemaing < lowTimeNum; }
    private bool HaveNPCTarget() { return target != null && target.tag == "NPC"; }
    private bool HavePlayerTarget() { return target != null && target.tag == "Player"; }
    private bool TargetInRange(float range) { return (target.position - transform.position).magnitude < range; }
    private bool HasWeapon() { return player.weaponHandler != null; }
    private bool playerInRange(float range)
    {
        Collider2D[] collidersHit = Physics2D.OverlapCircleAll(transform.position, range);
        foreach (Collider2D collider in collidersHit)
            if (collider.tag == "Player" && collider.transform != target && collider != this) {
                Debug.Log("Non-target Player too close!");
                return true;
            }
                
        return false;
    }
    public void Seek(bool seek)
    {
        pathfinder.enabled = seek;
    }

    public void SetTarget(Transform target)
    {
        Debug.Log("Setting Target to " + target);
        this.target = target;
    }

    void FixedUpdate()
    {
        //Check for if target was destroyed
        if (target == null)
            target = null;

        stateMachine.Tick();

        //Look
        if (player.lookDirection.sqrMagnitude > .1f)
        {
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(player.lookDirection.y, player.lookDirection.x));
        }

        if(stateMachine.GetState().GetType() == typeof(FindTag))
            currentState = CurrentState.FIND_TARGET;
        else if(stateMachine.GetState().GetType() == typeof(PathToTarget))
            currentState = CurrentState.PATH_TO_TARGET;
        else if (stateMachine.GetState().GetType() == typeof(DashAttack))
            currentState = CurrentState.DASH_ATTACK;
        else if (stateMachine.GetState().GetType() == typeof(WeaponAttack))
            currentState = CurrentState.WEAPON_ATTACK;

        pathfinder.target = target;
    }


    public void OnDrawGizmosSelected()
    {
        Collider2D[] collidersHit = Physics2D.OverlapCircleAll(transform.position, 10 * 1);
        Gizmos.DrawWireSphere(transform.position, 10);
    }


    public enum CurrentState
    {
        FIND_TARGET,
        PATH_TO_TARGET,
        DASH_ATTACK,
        WEAPON_ATTACK
    }
}
