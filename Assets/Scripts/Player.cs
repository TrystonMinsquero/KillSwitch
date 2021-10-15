using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float deathTime_MAX;
    public float dashDamage;
    public float movementSpeedInit;
    public float dashDistance;
    public float dashChargeTime;
    public float dashSpeed;
    public HealthBar healthBar;

    public GameObject deathEffect;
    public SpriteRenderer sr;
    private Animator anim;
    private Rigidbody2D rb;
    public WeaponHandler weaponHandler;

    [HideInInspector]
    public Vector2 lookDirection;
    private Vector3 healthBarPos;
    [HideInInspector]
    public float timeRemaing;
    [HideInInspector]
    public float movementSpeed;
    private float deathTime;
    private bool charging;
    private bool charged;
    private bool dashing;
    private int playerWhoHitMeLastIndex = -1;

    private void Start()
    {
        AssignComponents();
        deathTime = Time.time + deathTime_MAX;
        healthBarPos = healthBar.transform.position - transform.position;
        movementSpeed = movementSpeedInit;
        weaponHandler.Set();
    }

    public void AssignComponents()
    {

        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        weaponHandler = GetComponentInChildren<WeaponHandler>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if(Time.time > deathTime) 
            Die();
        timeRemaing = deathTime - Time.time;

        healthBar.SetHealth(timeRemaing / deathTime_MAX);
        healthBar.transform.position = transform.position + healthBarPos;

        if (charged)
        {
            StartCoroutine(Dash());
        }

        rb.angularVelocity = 0;
        SetAnimation();
    }

    public void Move(Vector2 input)
    {
        if (!dashing && !charging )
        {
            rb.velocity = new Vector2(input.x, input.y) * movementSpeed;
            rb.drag = input.sqrMagnitude > 0 ? 0 : 1;
        }
    }

    public void Look(Vector2 input)
    {
        if (!dashing)
        {
            Vector2 movementDirection = rb.velocity.normalized;
            //Look
            if (input.sqrMagnitude > .1f)
            {
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(input.y, input.x) + 90);
                lookDirection = input;
            }
            else if (movementDirection.sqrMagnitude > 0 && !charging && !charged)
            {
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(movementDirection.y, movementDirection.x) + 90);
                lookDirection = movementDirection;
            }

        }
    }

    public void Shoot()
    {
        if(!charging && !dashing)
        {
            weaponHandler.Shoot(this);
        }
    }

    public void StartDash()
    {
        if (dashing || charging && !charged)
            return;
        //Debug.Log("Dash");
        SFXManager.Play("Dash");
        StartCoroutine(ChargeDash(dashChargeTime));

    }

    private IEnumerator ChargeDash(float chargeTime)
    {
        charging = true;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(chargeTime);
        charged = true;
        charging = false;
    }
    private IEnumerator Dash()
    {
        rb.drag = 0;
        charged = false;
        dashing = true;
        Vector3 startPos = transform.position;
        //Debug.Log(lookDirection);
        rb.velocity = lookDirection * dashSpeed; //initial velocity added
        float maxDashTime = (dashDistance / rb.velocity.magnitude) + .3f; //Estimated
        float timeStarted = Time.time;
        while((transform.position - startPos).magnitude < dashDistance && Time.time - timeStarted < maxDashTime)
            yield return null;
        EndDash();
        //Debug.Log("Actual Time: " + (Time.time - timeStarted));
        //Debug.Log("Estimated Time: " + maxDashTime);
    }

    public void MarkWhoHitLast(PlayerInput otherPlayer)
    {
        playerWhoHitMeLastIndex = PlayerManager.GetIndex(otherPlayer);
    }

    private void EndDash()
    {
        rb.velocity = Vector2.zero;
        dashing = false;
        movementSpeed = movementSpeedInit;
    }

    public void TakeOver(NPC_Controller npcc)
    {
        NPC npc = npcc.npc;
        Debug.Log("Take Over " + npc.name);
        SwitchVisuals(npcc);
        EndDash();
        deathTime = Time.time + deathTime_MAX;
        npcc.Die();

    }

    public void TakeOver(Player player)
    {
        Debug.Log("Take Over " + player.name);
        if (Time.time < player.deathTime - dashDamage)
        {
            MarkWhoHitLast(player.GetComponent<PlayerInput>());
            player.TakeDamage(dashDamage);
            return;
        }
        SwitchVisuals(player);
        EndDash();
        deathTime = Time.time + deathTime_MAX;
        player.Die();

    }

    public void SetAnimation()
    {
        string stateName = "";
        if(!dashing && !charging)
        {
            Vector2 movementDirection = rb.velocity.normalized;
            Vector2 relativeDirection = lookDirection + movementDirection;
            if (rb.velocity.magnitude < .2f)
                stateName = "Idle";
            else if ((lookDirection - movementDirection).sqrMagnitude < .6f || (lookDirection + movementDirection).sqrMagnitude < .6f)
            {
                stateName = "Walk";
            }
            else
            {
                if(Mathf.Abs(movementDirection.x) > .1f)
                {

                    if (relativeDirection.x < 0 && relativeDirection.y > 0 || relativeDirection.x > 0 && relativeDirection.y < 0)
                        stateName = "Right";
                    else if (relativeDirection.x > 0 && relativeDirection.y > 0 || relativeDirection.x < 0 && relativeDirection.y < 0)
                        stateName = "Left";
                }
                else if(Mathf.Abs(movementDirection.y) > .1f)
                {

                    if (relativeDirection.x < 0 && relativeDirection.y > 0 || relativeDirection.x > 0 && relativeDirection.y < 0)
                        stateName = "Left";
                    else if (relativeDirection.x > 0 && relativeDirection.y > 0 || relativeDirection.x < 0 && relativeDirection.y < 0)
                        stateName = "Right";
                }
            }
        }
        else if (charging || dashing)
        {
            stateName = "Dash";
        }

        anim.Play(stateName);
        weaponHandler.SetAnimations(stateName);
    }

    public Vector2 GetVelocity() { return rb.velocity; }

    public void TakeDamage(float damage)
    {
        SFXManager.Play("Hit");
        if (damage <= 0)
            Die();
        else
            deathTime -= damage;
    }

    private void Die()
    {
        Debug.Log("Die");
        ScoreKeeper.ReigisterDeath(playerWhoHitMeLastIndex, PlayerManager.GetIndex(GetComponent<PlayerInput>()));
        playerWhoHitMeLastIndex = -1;
        GetComponent<PlayerUI>().Disable(true);
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        Instantiate(deathEffect, transform.position, Quaternion.identity).transform.localScale = transform.localScale;
        StartCoroutine(WaitToRespawn(2.2f));
    }

    private IEnumerator WaitToRespawn(float time)
    {
        yield return new WaitForSeconds(time);
        LevelManager.QueuePlayerToSpawn(this);

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (dashing)
        {
            if (collision.gameObject.CompareTag("Player") && collision.gameObject != this.gameObject)
            {
                TakeOver(collision.gameObject.GetComponent<Player>());
            }
            else if (collision.gameObject.CompareTag("NPC"))
            {
                TakeOver(collision.gameObject.GetComponent<NPC_Controller>());
            }
        }

    }

    public void SwitchVisuals(NPC_Controller npcc)
    {
        //sr.sprite = npcc.npc.image;
        anim.runtimeAnimatorController = npcc.npc.aoc;
        weaponHandler.SwitchWeapons(npcc.weaponHandler);

    }

    public void SwitchVisuals(Player player)
    {
        sr.sprite = player.sr.sprite;
        anim.runtimeAnimatorController = new AnimatorOverrideController(player.anim.runtimeAnimatorController);
        weaponHandler.SwitchWeapons(player.weaponHandler);
    }
}
