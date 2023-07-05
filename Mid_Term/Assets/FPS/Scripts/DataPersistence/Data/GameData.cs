using FPS;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]

public class GameData
{
    
    public int playerHp;
    public int playerShield;
    public GameData() 
    {
        playerHp = 100;
        playerShield = 100;
    }
}
