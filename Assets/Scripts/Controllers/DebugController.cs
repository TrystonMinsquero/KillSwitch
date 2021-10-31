using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugController : MonoBehaviour
{

    public static DebugController instance;
    public static bool debugMode = false; //if the debug console is visable
    public static bool debugJoin = false; //allows players to join mid game
    private static List<DebugCommandBase> commandList;

    public DebugCommandBase[] _commandList;

    [Header("UI Draggables")]
    public Text _console;

    //UI non-draggables
    static Canvas debugCanvas;
    static InputField input;
    static Text console;
    
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
        commandList = new List<DebugCommandBase>();
        for(int i = 0; i < _commandList.Length; i++)
        {
            switch (_commandList[i].id.ToLower())
            {
                case "help":
                    commandList.Add(new DebugCommand(_commandList[i].id, _commandList[i].description, _commandList[i].format,
                        () =>
                        {
                            ShowHelp();
                        }));
                    break;
                case "kill_all_npcs":
                    commandList.Add(new DebugCommand(_commandList[i].id, _commandList[i].description, _commandList[i].format,
                        () =>
                        {
                            if (NPCManager.instance != null)
                                NPCManager.KillAll();
                        }));
                    break;
                case "kill_all_players":
                    commandList.Add(new DebugCommand(_commandList[i].id, _commandList[i].description, _commandList[i].format,
                        () =>
                        {
                            if (PlayerManager.instance != null)
                                PlayerManager.KillAll();
                        }));
                    break;
                case "set_game_time":
                    commandList.Add(new DebugCommand<int>(_commandList[i].id, _commandList[i].description, _commandList[i].format,
                        (x) =>
                        {
                            LevelManager.SetGameTime(x);
                        }));
                    break;
                case "allow_join":
                    commandList.Add(new DebugCommand<bool>(_commandList[i].id, _commandList[i].description, _commandList[i].format,
                        (x) =>
                        {
                            PlayerManager.SetJoinable(x);
                        }));
                    break;
                case "godmode":
                    commandList.Add(new DebugCommand<bool>(_commandList[i].id, _commandList[i].description, _commandList[i].format,
                        (x) =>
                        {
                            foreach (UnityEngine.InputSystem.PlayerInput player in PlayerManager.players)
                                if (player != null && player.GetComponent<Player>() != null)
                                    player.GetComponent<Player>().SetGodMode(x);
                        }));
                    break;

            }
        }
        {
            /*
            DebugCommand HELP = new DebugCommand("help", "show the list of available commands", "help", () =>
            {
                ShowHelp();
            });

            DebugCommand KILL_ALL_NPCS = new DebugCommand("kill_all_npcs", "Kill all npcs from level", "kill_all_npcs", () =>
            {
                if (NPCManager.instance != null)
                    NPCManager.KillAll();
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
            */
        } // Old method of instatiating debug commands
    }

    public static void OnToggleDebug(bool triggered, UnityEngine.InputSystem.PlayerInput player = null)
    {
        if (PlayerManager.inLobby)
            return;
        if (triggered)
            debugMode = !debugMode;

        if (debugMode)
        {
            input.ActivateInputField();
            input.Select();
            input.enabled = true;
        }
        else
        {
            input.DeactivateInputField();
            input.enabled = false;
            input.text = "";
        }

        if (player != null)
        {
            player.GetComponent<PlayerUI>().Debug(debugMode);
            player.GetComponent<PlayerController>().Debug(debugMode);
        }
        debugCanvas.enabled = debugMode;
    }

    private void OnDestroy()
    {
        input.DeactivateInputField();
        input.enabled = false;
        input.text = "";
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
        //parse through commands
        while(i < commandList.Count && !commandExecuted)
        {
            DebugCommandBase command = commandList[i];
            if (input.Contains(command.id))
            {
                //Checks for no arguemnt commands
                if (command as DebugCommand != null)
                {
                    WriteToConsole("Invoked: " + command.id);
                    (command as DebugCommand).Invoke();
                    commandExecuted = true;
                }
                //Checks for 1 int argument command
                else if (command as DebugCommand<int> != null)
                {
                    if (properties.Length != 2)
                    {
                        i++;
                        continue;
                    }
                    WriteToConsole("Invoked: " + command.id + " " + int.Parse(properties[1]));
                    (command as DebugCommand<int>).Invoke(int.Parse(properties[1]));
                    commandExecuted = true;
                }
                //Checks for 1 bool argument command
                else if (command as DebugCommand<bool> != null)
                {
                    if (properties.Length != 2)
                    {
                        i++;
                        continue;
                    }
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
