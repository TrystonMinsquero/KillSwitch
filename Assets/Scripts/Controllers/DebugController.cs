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

    public Text _console;

    static Canvas debugCanvas;
    static InputField input;
    static Text console;
    

    public static List<DebugCommandBase> commandList;


    public void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
            instance = this;


        debugCanvas = GetComponentInChildren<Canvas>();
        input = GetComponentInChildren<InputField>();
        console = _console;

        DefineCommands();
    }
    private void Start()
    {
    }

    public static void WriteToConsole(string text)
    {
        if (console.text != "")
            console.text += "\n";
        console.text += text;
    }

    private void ShowHelp()
    {
        foreach (DebugCommandBase command in commandList)
            WriteToConsole(command.format + ": " + command.description);
    }

    private void DefineCommands()
    {
        DebugCommand HELP = new DebugCommand("help", "show the list of available commands", "help", () =>
        {
            ShowHelp();
        });

        DebugCommand KILL_ALL_NPCS = new DebugCommand("kill_all_npcs", "Kill all npcs from level", "kill_all_npcs", () =>
        {
            if (NPCManager.instance != null)
                NPCManager.KillALL();
        });
        DebugCommand KILL_ALL_PLAYERS = new DebugCommand("kill_all_players", "Kill all players from level", "kill_all_players", () =>
        {
            if (PlayerManager.instance != null)
                PlayerManager.KillAll();
        });

        DebugCommand<int> SET_GAME_TIME = new DebugCommand<int>("set_game_time", "Set the time remaing on the game", "set_game_time <time_in_seconds>", (x) =>
        {
            LevelManager.SetGameTime(x);
        });

        DebugCommand<bool> JOIN_ENABLE = new DebugCommand<bool>("allow_join", "allow players to join mid game", "allow_join <enabled>", (x) =>
        {
            PlayerManager.SetJoinable(x);
        });

        DebugCommand<bool> GOD_MODE = new DebugCommand<bool>("godmode", "disable death for player 1", "godmode <enabled>", (x) =>
       {
           PlayerManager.players[0].GetComponent<Player>().SetGodMode(x);

       });

        commandList = new List<DebugCommandBase>
        {
            HELP,
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
            input.Select();
        }
        else
        {
            input.DeactivateInputField();
            input.text = "";
        }

        if (player != null)
        {
            player.GetComponent<PlayerUI>().Debug(debugMode);
            player.GetComponent<PlayerController>().Debug(debugMode);
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


        int i = 0;
        bool commandExecuted = false;
        while(i < commandList.Count && !commandExecuted)
        {
            DebugCommandBase command = commandList[i];
            if (input.Contains(command.id))
            {
                if (command as DebugCommand != null)
                {
                    WriteToConsole("Invoked: " + command.id);
                    (command as DebugCommand).Invoke();
                    commandExecuted = true;
                }
                else if (command as DebugCommand<int> != null)
                {
                    WriteToConsole("Invoked: " + command.id + " " + int.Parse(properties[1]));
                    (command as DebugCommand<int>).Invoke(int.Parse(properties[1]));
                    commandExecuted = true;
                }
                else if (command as DebugCommand<bool> != null)
                {
                    int value;
                    bool boolVal;
                    if (int.TryParse(properties[1], out value))
                        boolVal = value != 0;
                    else if (!bool.TryParse(properties[1], out boolVal))
                        boolVal = false;
                    WriteToConsole("Invoked: " + command.id + " " + boolVal);
                    (command as DebugCommand<bool>).Invoke(boolVal);
                    commandExecuted = true;
                }
            }
            i++;
        }
        if (!commandExecuted)
            WriteToConsole("Invalid Command, use help to see the a list of commands");

    }
}
