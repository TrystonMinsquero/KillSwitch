using UnityEngine;

public class DashAttack : IState
{
    private readonly BotController bot;
    private readonly Player player;

    public DashAttack(BotController bot, Player player)
    {
        this.bot = bot;
        this.player = player;
    }

    public void OnEnter()
    {
        Debug.Log("Dashing to: " + bot.target);

    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        if(bot.target != null)
            player.lookDirection = (bot.target.position - player.transform.position).normalized;
        player.StartDash();
    }
}
