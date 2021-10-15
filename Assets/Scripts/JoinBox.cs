using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JoinBox : MonoBehaviour
{
    public int slot;
    public bool hasPlayer;
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
