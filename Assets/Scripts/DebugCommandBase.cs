using System;
using UnityEngine;


//Base class for debug commands
[Serializable]
public class DebugCommandBase
{
    public string id;
    public string description;
    public string format;

    public DebugCommandBase(string id, string description, string format)
    {
        this.id = id;
        this.description = description;
        this.format = format;
    }
}

//Debug command with no arguments
[System.Serializable]
public class DebugCommand : DebugCommandBase
{
    private Action command;

    public DebugCommand(string id, string description, string format, Action command) : base (id, description, format)
    {
        this.command = command;
    }

    public void Invoke()
    {
        Debug.Log("Invoking: " + id);
        command.Invoke();
    }
}

//Debug command with one argument
[Serializable]
public class DebugCommand<T> : DebugCommandBase
{
    private Action<T> command;

    public DebugCommand(string id, string description, string format, Action<T> command) : base(id, description, format)
    {
        this.command = command;
    }

    public void Invoke(T value)
    {
        Debug.Log("Invoking: " + id);
        command.Invoke(value);
    }
}