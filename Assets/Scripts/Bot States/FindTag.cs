using UnityEngine;

public class FindTag : IState
{
    private readonly BotController bot;
    private string tag;
    private LayerMask layerMask;
    private Vector3 smallestDistance = Vector3.one * int.MaxValue;
    private float searchRadius = 10;


    public FindTag(BotController bot, string tag, string layerName = "Entity")
    {
        this.bot = bot;
        this.tag = tag;
        layerMask = LayerMask.NameToLayer(layerName);
    }


    public void OnEnter()
    {
        Debug.Log("Finding " + tag);
        bot.SetTarget(Search());
        if (bot.target == null)
            Debug.Log("Not found on first search");

    }

    private Transform Search(int radiusMultiplier = 1)
    {
        Collider2D[] collidersHit = Physics2D.OverlapCircleAll(bot.transform.position, searchRadius * radiusMultiplier);
        //Debug.Log(collidersHit.Length);
        Transform tempTarget = null;
        foreach (Collider2D collider in collidersHit)
        {
            if (collider.tag == tag && collider.gameObject != bot.gameObject && DistanceSmaller(collider.transform.position, smallestDistance))
            {
                tempTarget = collider.transform;
                smallestDistance = collider.transform.position;
            }
        }
        return tempTarget;
    }

    private bool DistanceSmaller(Vector3 targetPos, Vector3 compare) 
    { 
        return (targetPos - bot.transform.position).magnitude < (compare - bot.transform.position).magnitude; 
    }

    public void OnExit()
    {
        if (bot.target == null)
            Debug.Log(tag + " not found");
        else
            Debug.Log(tag + " Found: " + bot.target.name);
    }

    public void Tick()
    {
        if(bot.target == null)
            bot.SetTarget(Search(2));
    }



}
