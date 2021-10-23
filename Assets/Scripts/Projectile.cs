using UnityEngine;
using UnityEngine.InputSystem;

public class Projectile : MonoBehaviour
{
    [Header("Explosion")]
    public GameObject explosionPrefab;
    public float explosionRadius = 1.5f;

    //references
    private PlayerInput player;
    private Rigidbody2D rb;

    //bullet info
    private Weapon weapon;
    private Vector2 direction;
    private Vector3 startPos;


    public void Set(PlayerInput player, Weapon weapon, Vector2 direction)
    {
        this.player = player;
        this.weapon = weapon;
        this.direction = direction;

        rb = GetComponent<Rigidbody2D>();

        rb.velocity = direction * weapon.projectileSpeed;
        startPos = transform.position;
    }

    private void Update()
    {

        //Destroy if too far
        Vector2 distance = startPos - transform.position;
        if (distance.magnitude >= weapon.range)
        {
            if (weapon.weaponType == WeaponType.RPG)
                Explode(explosionRadius);
            else
                Delete();
        }
        
        //Ensure velocity is constant
        rb.velocity = direction * weapon.projectileSpeed;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //Check assumptions
        if (collision.CompareTag("Player") && collision.GetComponent<PlayerInput>() == player)
            return;
        else if (weapon.explodes)
            Explode(explosionRadius);
        else if (collision.CompareTag("Projectile"))
            return;


        switch (collision.tag)
        {
            case "Player":
                DamagePlayer(collision.GetComponent<Player>());
                break;

            case "NPC":
                collision.GetComponent<NPC_Controller>().Die();
                break;

            case "Wall":
                break;
        }

        if (weapon.peircing)
            PassThroughWall();
        else
            Delete();

    }

    private void DamagePlayer(Player otherPlayer)
    {
        otherPlayer.MarkWhoHitLast(player);
        otherPlayer.TakeDamage(weapon.damage);
    }

    private void PassThroughWall()
    {

        weapon.damage /= 2;   // wallbang -> damage only half
        transform.localScale /= 2; //shrink by 2
    }

    private void Delete()
    {
        Destroy(this.gameObject);
    }

    private void Explode(float explosionRadius)
    {
        if (explosionPrefab != null)
        {
            SFXManager.Play("Explosion");
            Instantiate(explosionPrefab, transform.position, Quaternion.identity).transform.localScale = Vector3.one * explosionRadius;
        }
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D collision in objectsInRange)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                DamagePlayer(collision.GetComponent<Player>());
            }
            else if (collision.gameObject.CompareTag("NPC"))
            {
                collision.GetComponent<NPC_Controller>().Die();
                
            }
        }
        Delete();
    }

    

    private void AreaDamageEnemies(Vector3 location, float radius, float damage)
    {
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(location, radius);

        foreach (Collider2D collision in objectsInRange)
        {

            if (collision.gameObject.CompareTag("Player"))
            {
                if (collision.GetComponent<Player>() != player)
                    collision.GetComponent<Player>().TakeDamage(damage);
            }
            else if (collision.gameObject.CompareTag("NPC"))
            {
                Destroy(collision.gameObject);
                Delete();
            }

            else
            {
                Delete();
            }
        }
    }

}
