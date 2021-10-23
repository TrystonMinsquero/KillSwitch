using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JoinBox : MonoBehaviour
{
    [Header("Data")]
    public int slot;
    public bool hasPlayer;

    [Header("UI Draggables")]
    public Canvas joined;
    public Canvas empty;

    public void AddPlayer(PlayerInput player)
    {
        empty.enabled = false;
        joined.enabled = true;
        hasPlayer = true;
        
    }

    public void RemovePlayer(PlayerInput player)
    {
        joined.enabled = false;
        empty.enabled = true;
        hasPlayer = false;
    }
    
}
