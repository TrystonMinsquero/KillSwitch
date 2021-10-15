using UnityEngine;
using UnityEngine.InputSystem;

public class Projectile : MonoBehaviour
{

    Player player;
    Vector3 startPos;
    float speed;
    float range;
    Vector2 direction;
    int damage;

    Rigidbody2D rb;
    public GameObject explosionPrefab;
    public float explosionRadius = 1.5f;

    public void Set(Player player, Vector3 startPos, float speed, float range, Vector2 direction, int damage = 0)
    {
        this.player = player;
        this.startPos = startPos;
        this.speed = speed;
        this.range = range;
        this.direction = direction;
        this.damage = damage;


        rb = GetComponent<Rigidbody2D>();

        transform.position = startPos;

        rb.velocity = direction * speed;
        rb.velocity += player.GetVelocity();

        //if (player.weapon.weapon.weaponType == WeaponType.GRENADE)
        //    GetComponent<Collider2D>().isTrigger = false;
    }

    private void Update()
    {

        //Destroy if too far
        Vector2 distance = startPos - transform.position;
        if (distance.magnitude >= range)
        {
            if (player.weaponHandler.weapon.weaponType == WeaponType.RPG)
                Explode(explosionRadius);
            else
                Delete();
        }


        rb.velocity = direction * speed;
    }

    //public void OnCollisionEnter2D(Collision2D collision)
    //{
    //    direction *= -1;
    //}



    public void OnTriggerEnter2D(Collider2D collision)
    {
        switch (player.weaponHandler.weapon.weaponType)
        {
            case WeaponType.STRAIGHT:
                if (collision.gameObject.CompareTag("Projectile"))
                    return;
                if (collision.gameObject.CompareTag("Player"))
                {
                    if (collision.GetComponent<Player>() != player)
                    {
                        DamagePlayer(collision.GetComponent<Player>());
                        Delete();
                        
                    }
                }
                else if (collision.gameObject.CompareTag("NPC"))
                {
                    collision.GetComponent<NPC_Controller>().Die();
                    Delete();
                    
                }
                else
                    Delete();
                break;




            case WeaponType.SHOTGUN:
                if (collision.gameObject.CompareTag("Projectile"))
                    return;
                if (collision.gameObject.CompareTag("Player"))
                {
                    if (collision.GetComponent<Player>() != player)
                    {
                        DamagePlayer(collision.GetComponent<Player>());
                        PassThroughWall(); //WallBang
                    }
                    
                }
                else if (collision.gameObject.CompareTag("NPC"))
                {
                    collision.GetComponent<NPC_Controller>().Die();
                    Delete();
                    
                }
                else if (collision.gameObject.CompareTag("Wall"))
                {
                    PassThroughWall(); //WallBang
                }
                else
                {
                    Delete();
                }
                break;



            // Sniper
            case WeaponType.LONG:
                if (collision.gameObject.CompareTag("Projectile"))
                    return;
                if (collision.gameObject.CompareTag("Player"))
                {
                    if (collision.GetComponent<Player>() != player)
                        DamagePlayer(collision.GetComponent<Player>());
                    
                }
                else if (collision.gameObject.CompareTag("NPC"))
                {
                    collision.GetComponent<NPC_Controller>().Die();
                    
                }
                else if (collision.gameObject.CompareTag("Wall"))
                {
                    PassThroughWall(); //WallBang
                }
                else
                {
                    Delete();
                }
                break;

            case WeaponType.RPG:
                if (collision.gameObject.CompareTag("Projectile"))
                    return;
                if (collision.gameObject.CompareTag("Player"))
                {
                    if (collision.GetComponent<Player>() != player)
                        Explode(explosionRadius);
                    
                }
                else
                    Explode(explosionRadius);
                    
                break;

            // RPG
            //case WeaponType.RPG:
            //    if (collision.gameObject.CompareTag("Projectile"))
            //        return;
            //    if (collision.gameObject.CompareTag("Player"))
            //    {
            //        if (collision.GetComponent<Player>() != player)
            //            collision.GetComponent<Player>().TakeDamage(damage);

            //        AreaDamageEnemies(transform.position, 1.5f, damage);
            //    }
            //    else if (collision.gameObject.CompareTag("NPC"))
            //    {
            //        AreaDamageEnemies(transform.position, 1.5f, damage);

            //        Destroy(collision.gameObject);
            //        Delete();

            //        GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
            //        Destroy(expl, 3f);

            //    }
            //    else
            //    {
            //        AreaDamageEnemies(transform.position, 1.5f, damage);
            //        GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
            //        Destroy(expl, 3f);
            //        Delete();         
            //    }

            //    break;




        }
    }

    public void DamagePlayer(Player otherPlayer)
    {
        otherPlayer.MarkWhoHitLast(player.GetComponent<PlayerInput>());
        otherPlayer.TakeDamage(damage);
    }

    public void PassThroughWall()
    {

        damage /= 2;   // wallbang -> damage only half
        transform.localScale /= 2; //shrink by 2
    }

    public void Delete()
    {
        player.weaponHandler.GetProjectiles().Remove(this);
        Destroy(this.gameObject);
    }

    public void Explode(float explosionRadius)
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

    

    public void AreaDamageEnemies(Vector3 location, float radius, float damage)
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
