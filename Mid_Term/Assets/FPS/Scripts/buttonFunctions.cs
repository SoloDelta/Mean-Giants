using FPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
   public void Resume()
    {
        GameManager.instance.UnpausedState();
    }

    public void RespawnPlayer()
    {   
        GameManager.instance.UnpausedState();
        GameManager.instance.playerScript.SpawnPlayer();
    }

    public void Restart()
    {
        GameManager.instance.UnpausedState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit(); 
    }
}
