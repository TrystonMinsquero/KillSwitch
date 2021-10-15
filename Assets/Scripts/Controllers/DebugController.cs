using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DebugController : MonoBehaviour
{

    public static DebugController instance;
    public static bool debugMode = false;
    public static bool debugJoin = false;

    static Canvas debugCanvas;
    static InputField input;

    public static List<DebugCommandBase> commandList;


    public void Awake()
    {
        if (instance != null)
            instance = this;
        else
            Destroy(this);

        debugCanvas = GetComponentInChildren<Canvas>();
        input = GetComponentInChildren<InputField>();
        DontDestroyOnLoad(gameObject);
        DefineCommands();
    }

    private void DefineCommands()
    {
        DebugCommand KILL_ALL_NPCS = new DebugCommand("kill_all_npcs", "Kills all npcs from level", "kill_all_npcs", () =>
        {
            if (NPCManager.instance != null)
                NPCManager.KillALL();
        });
        DebugCommand KILL_ALL_PLAYERS = new DebugCommand("kill_all_players", "Kills all players from level", "kill_all_players", () =>
        {
            if (NPCManager.instance != null)
                NPCManager.KillALL();
        });

        DebugCommand<int> SET_GAME_TIME = new DebugCommand<int>("set_game_time", "Sets the time remaing on the game", "set_game_time <time_in_seconds>", (x) =>
        {
            LevelManager.SetGameTime(x);
        });

        DebugCommand<bool> JOIN_ENABLE = new DebugCommand<bool>("join_enable", "allows players to join mid game", "join_enable <enabled>", (x) =>
        {
            debugJoin = x;
            PlayerManager.SetJoinable(x);
        });

        DebugCommand<bool> GOD_MODE = new DebugCommand<bool>("god_mode", "disables death for player 1", "god_mode <enabled>", (x) =>
       {
           PlayerManager.players[0].GetComponent<Player>().SetGodMode(x);

       });

        commandList = new List<DebugCommandBase>
        {
            KILL_ALL_NPCS,
            KILL_ALL_PLAYERS,
            SET_GAME_TIME,
            JOIN_ENABLE,
            GOD_MODE,
        };
    }

    public static void OnToggleDebug(bool triggered, PlayerInput player = null)
    {
        if (PlayerManager.inLobby)
            return;
        if (triggered)
            debugMode = !debugMode;
        if (debugMode)
        {
            input.ActivateInputField();
        }
        else
        {
            HandleInput(input.text);
            input.text = "";
            input.DeactivateInputField();
        }
        debugCanvas.enabled = debugMode;
    }

    public static void OnReturn(bool triggered)
    {
        if (PlayerManager.inLobby)
            return;
        if (triggered && debugMode)
        {
            HandleInput(input.text);
            input.text = "";
            input.ActivateInputField();
        }
    }

    private static void HandleInput(string input)
    {
        string[] properties = input.Split(' ');

        foreach(DebugCommandBase command in commandList)
        {
            if (input.Contains(command.id))
            {
                if (command as DebugCommand != null)
                    (command as DebugCommand).Invoke();
                else if (command as DebugCommand<int> != null)
                {
                    (command as DebugCommand<int>).Invoke(int.Parse(properties[1]));
                }
                else if (command as DebugCommand<bool> != null)
                {
                    int value;
                    bool boolVal;
                    if (int.TryParse(properties[1], out value))
                        boolVal = value != 0;
                    else if (!bool.TryParse(properties[1], out boolVal))
                        boolVal = false;
                    (command as DebugCommand<bool>).Invoke(boolVal);
                }

            }
            else
                Debug.Log("Not a valid command, check help");
            
                
        }
    }
}
