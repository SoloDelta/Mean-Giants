using FPS;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]

public class GameData
{
    PlayerController player;
    
    public int playerHp;
    public int playerShield;
    public Vector3 playerPosition;
    public GameData() 
    {
        playerHp = player.health;
        playerShield = player.shield;
        playerPosition = player.transform.position;
    }
}
