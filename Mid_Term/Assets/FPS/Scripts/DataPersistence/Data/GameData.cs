using FPS;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
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
        //this.playerHp = player.health;
        //this.playerShield = player.shield;
        //this.playerPosition = player.transform.position;
    }
}
