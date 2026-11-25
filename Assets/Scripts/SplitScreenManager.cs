using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SplitScreenManager : MonoBehaviour
{
    readonly List<PlayerInput> players = new();

    public void OnPlayerJoined(PlayerInput player)
    {
        players.Add(player);
        UpdateLayout();
    }

    public void OnPlayerLeft(PlayerInput player)
    {
        players.Remove(player);
        UpdateLayout();
    }

    void UpdateLayout()
    {
        Rect[] rects = GetLayout(players.Count);
        for(int i = 0; i < players.Count; i++)
            players[i].camera.rect = rects[i];
    }

    Rect[] GetLayout(int count)
    {
        switch (count)
        {
            case 1: return new Rect[] { new Rect(0, 0, 1, 1) };
            
            case 2:
                return new Rect[]
                {
                    new Rect(0, 0.5f, 1, 0.5f), // Top
                    new Rect(0, 0, 1, 0.5f)     // Bottom
                };
            
            case 3:
                return new Rect[]
                {
                    new Rect(0, 0.5f, 1, 0.5f),     // Top, Full Width
                    new Rect(0, 0, 0.5f, 0.5f),     // Bottom Left
                    new Rect(0.5f, 0, 0.5f, 0.5f)   // Bottom Right
                };
            
            default:
                return new[]
                {
                    new Rect(0, 0.5f, 0.5f, 0.5f),  // Top Left
                    new Rect(0.5f, 0.5f, 0.5f, 0.5f),     // Top Right
                    new Rect(0, 0, 0.5f, 0.5f),  // Bottom Left
                    new Rect(0.5f, 0, 0.5f, 0.5f),  // Bottom Right
                };
        }
    }
}
