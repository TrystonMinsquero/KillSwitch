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
    public bool peircing = false;
    public bool explodes = false;

    [Header("Stats")]
    public float fireDelay;
    public float projectileSpeed;
    public float range;
    public int damage;

    private float nextFireTime;


    public bool Shoot(Player player, Vector2 direction)
    {
        if (Time.time < nextFireTime)
            return false;
        nextFireTime = Time.time + fireDelay;

        Vector3 shootPosition = player.transform.position;

        switch (weaponType)
        {
            case WeaponType.STRAIGHT:
                CreateProjectile(shootPosition).Set(player, this, direction);
                SFXManager.Play("Gun");
                break;

            case WeaponType.SHOTGUN:

                float angle = Mathf.Atan2(direction.y, direction.x);
                // angle from player to x-axis

                Vector2 rightBulletDirection = new Vector2(direction.x - (0.05f) * Mathf.Sin(angle),
                    direction.y + (0.05f) * Mathf.Cos(angle));
                Vector2 leftBulletDirection = new Vector2(direction.x + (0.05f) * Mathf.Sin(angle),
                    direction.y - (0.05f) * Mathf.Cos(angle));
                // vectors for direction of each shotgun bullet/projectile


                CreateProjectile(shootPosition).Set(player, this, rightBulletDirection);
                CreateProjectile(shootPosition).Set(player, this, direction);
                CreateProjectile(shootPosition).Set(player, this, leftBulletDirection);

                SFXManager.Play("Shotgun");

                break;

            case WeaponType.RPG:

                CreateProjectile(shootPosition).Set(player, this, direction);

                SFXManager.Play("Gun");
                break;

            case WeaponType.LONG:
                CreateProjectile(shootPosition).Set(player, this, direction);
                SFXManager.Play("Sniper");
                break;
        }

        return true;
    }

    public void Reset()
    {
        nextFireTime = 0;
    }

    private Projectile CreateProjectile(Vector3 position)
    {
        return Instantiate(projectilePrefab, position, Quaternion.identity).GetComponent<Projectile>();
    }

}

public enum WeaponType
{
    STRAIGHT,
    SHOTGUN,
    RPG,
    LONG
}