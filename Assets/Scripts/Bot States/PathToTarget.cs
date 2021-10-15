using UnityEngine;
using Pathfinding;

public class PathToTarget : IState
{
    private readonly BotController bot;

    public PathToTarget(BotController bot)
    {
        this.bot = bot;
    }

    public void OnEnter()
    {
        Debug.Log("Going to: " + bot.target);
        bot.Seek(true);
    }

    public void OnExit()
    {
        bot.Seek(false);
    }

    public void Tick()
    {
        
    }
}
