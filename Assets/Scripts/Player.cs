using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerUI), typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Attributes")]
    public float deathTime_MAX;
    public float dashDamage;
    public float movementSpeedInit;
    public float dashDistance;
    public float dashChargeTime;
    public float dashSpeed;

    [Header("Draggables")]
    public GameObject deathEffect;

    //Components
    private Rigidbody2D rb;
    [HideInInspector]
    private WeaponHandler weaponHandler;

    //trackers
    private Vector2 lookDirection;
    private float movementSpeed;

    //Times
    private float timeRemaing;
    private float deathTime;

    //Statuses
    private bool godMode;
    private bool charging;
    private bool charged;
    private bool dashing;
    private int playerWhoHitMeLastIndex = -1;

    private void Start()
    {
        AssignComponents();
        deathTime = Time.time + deathTime_MAX;
        movementSpeed = movementSpeedInit;
        weaponHandler.Set();
    }

    public void AssignComponents()
    {
        GetComponent<PlayerUI>().AssignComponents();
        rb = GetComponent<Rigidbody2D>();
        weaponHandler = GetComponentInChildren<WeaponHandler>();
    }

    private void Update()
    {
        if(Time.time > deathTime && !godMode) 
            Die();
        timeRemaing =  !godMode ? deathTime - Time.time : deathTime_MAX;

        if (charged)
            StartCoroutine(Dash());

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
        else if (charging)
        {

            if (input.sqrMagnitude > .1f)
            {
                SetRotation(input);
            }
        }
    }

    public void Look(Vector2 input)
    {
        if (!dashing)
        {
            Vector2 movementDirection = rb.velocity.normalized;

            //Look
            if (input.sqrMagnitude > .1f)
                SetRotation(input);
            else if (movementDirection.sqrMagnitude > 0 && !charging && !charged)
                SetRotation(movementDirection);
        }
    }

    //Sets the rotation from a nomalized vector2 (like the unit circle)
    private void SetRotation(Vector2 lookDir)
    {
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(lookDir.y, lookDir.x) + 90);
        lookDirection = lookDir;
    }

    //Sets the direction from quaterion
    private void SetRotation(Quaternion rotation)
    {
        Vector3 eulerRotation = rotation.eulerAngles;
        eulerRotation.z += eulerRotation.z + 180 > 360 ? -180 : 180;
        transform.rotation = Quaternion.Euler(eulerRotation);
        float zRadian = (eulerRotation.z - 90) * Mathf.Deg2Rad;
        lookDirection = new Vector2(Mathf.Cos(zRadian), Mathf.Sin(zRadian));
    }

    public void Shoot()
    {
        if(!charging && !dashing)
        {
            //Will change position later
            weaponHandler.Shoot(GetComponent<PlayerInput>(), lookDirection.normalized);
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

    public void SetGodMode(bool enable)
    {
        godMode = enable;
    }

    public void TakeOver(NPC_Controller npcc)
    {
        NPC npc = npcc.npc;
        weaponHandler.SwitchWeapons(npcc.weaponHandler);
        GetComponent<PlayerUI>().SwitchVisuals(npcc);
        EndDash();
        transform.position = npcc.transform.position;
        SetRotation(npcc.transform.rotation);
        deathTime = Time.time + deathTime_MAX;
        npcc.Die();

    }

    public void TakeOver(Player player)
    {
        if (Time.time < player.deathTime - dashDamage)
        {
            MarkWhoHitLast(player.GetComponent<PlayerInput>());
            player.TakeDamage(dashDamage);
            return;
        }
        Debug.Log(name + " Takes Over " + player.name);
        transform.position = player.transform.position;
        SetRotation(player.lookDirection);
        weaponHandler.SwitchWeapons(player.weaponHandler);
        GetComponent<PlayerUI>().SwitchVisuals(player.GetComponent<PlayerUI>());
        EndDash();
        deathTime = Time.time + deathTime_MAX;
        player.Die();

    }

    public Weapon GetWeapon()
    {
        return weaponHandler.weapon;
    }

    public float GetCurrentHealth()
    {
        return timeRemaing / deathTime_MAX;
    }

    public void SetWeaponActive(bool enable)
    {
        if (enable)
            weaponHandler.EnableVisuals();
        else
            weaponHandler.DisableVisuals();
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

        GetComponent<PlayerUI>().SetAnimations(stateName);
        weaponHandler.SetAnimations(stateName);
    }

    public Vector2 GetVelocity() { return rb.velocity; }

    public void TakeDamage(float damage)
    {
        if (godMode)
            return;
        SFXManager.Play("Hit");
        if (damage <= 0)
            Die();
        else
            deathTime -= damage;
    }

    public void Die()
    {
        Debug.Log(name + " dies");
        ScoreKeeper.ReigisterDeath(playerWhoHitMeLastIndex, PlayerManager.GetIndex(GetComponent<PlayerInput>()));
        playerWhoHitMeLastIndex = -1;
        MultipleTargetCamera.instance.targets.Remove(transform);
        GetComponent<PlayerUI>().Disable();
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

}
