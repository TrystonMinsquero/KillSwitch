using UnityEngine;

public class WeaponAttack : IState
{
    private readonly BotController bot;
    private readonly Player player;

    public WeaponAttack(BotController bot, Player player)
    {
        this.bot = bot;
        this.player = player;
    }

    public void OnEnter()
    {
        Debug.Log("Shooting: " + bot.target);
    }

    public void OnExit()
    {
        Debug.Log("Stop shooting");
    }

    public void Tick()
    {
        player.lookDirection = (bot.target.position - player.transform.position).normalized;
        player.Shoot();
    }
}
