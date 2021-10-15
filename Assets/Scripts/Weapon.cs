using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "new Weapon")]
public class Weapon : ScriptableObject
{
    [Header("Visual")]
    public Color color = Color.white;
    public AnimatorOverrideController weaponAoc;

    [Header("Muzzle Flash")]
    public AnimatorOverrideController flashAoc;

    [Header("Projectile Details")]
    public GameObject projectilePrefab;
    public WeaponType weaponType;

    [Header("Stats")]
    public float fireDelay;
    public float projectileSpeed;
    public float range;
    public int damage;

    [HideInInspector]
    public List<Projectile> projectiles;

    private float nextFireTime;


    public bool Shoot(Player player)
    {
        if (Time.time < nextFireTime)
            return false;
        nextFireTime = Time.time + fireDelay;
        Projectile pro = GameObject.Instantiate(projectilePrefab).GetComponent<Projectile>();


        switch (weaponType)
        {
            case WeaponType.STRAIGHT:
                pro.Set(player, player.transform.position, projectileSpeed, range, player.lookDirection.normalized, damage);
                projectiles.Add(pro);

                SFXManager.Play("Gun");

                break;


            case WeaponType.SHOTGUN:

                float angle = Mathf.Atan2(player.lookDirection.y, player.lookDirection.x);
                // angle from player to x-axis

                Vector2 shotgun = new Vector2(player.lookDirection.x - (0.05f) * Mathf.Sin(angle),
                    player.lookDirection.y + (0.05f) * Mathf.Cos(angle));
                Vector2 shotgun2 = new Vector2(player.lookDirection.x + (0.05f) * Mathf.Sin(angle),
                    player.lookDirection.y - (0.05f) * Mathf.Cos(angle));
                // vectors for direction of each shotgun bullet/projectile

                Projectile pro2 = GameObject.Instantiate(projectilePrefab).GetComponent<Projectile>();
                // second bullet created

                Projectile pro3 = GameObject.Instantiate(projectilePrefab).GetComponent<Projectile>();

                pro.Set(player, player.transform.position, projectileSpeed, range, shotgun, damage);
                pro2.Set(player, player.transform.position, projectileSpeed, range, shotgun2, damage);
                pro3.Set(player, player.transform.position, projectileSpeed, range, player.lookDirection.normalized, damage);

                projectiles.Add(pro);
                projectiles.Add(pro2);
                projectiles.Add(pro3);

                SFXManager.Play("Shotgun");

                break;

            case WeaponType.RPG:
                pro.Set(player, player.transform.position, projectileSpeed, range, player.lookDirection.normalized, damage);
                projectiles.Add(pro);

                SFXManager.Play("Gun");
                break;

            case WeaponType.LONG:
                pro.Set(player, player.transform.position, projectileSpeed, range, player.lookDirection.normalized, damage);
                projectiles.Add(pro);

                SFXManager.Play("Sniper");
                break;
        }

        return true;
    }

    public void Reset()
    {
        nextFireTime = 0;
    }


}

public enum WeaponType
{
    STRAIGHT,
    SHOTGUN,
    RPG,
    LONG
}