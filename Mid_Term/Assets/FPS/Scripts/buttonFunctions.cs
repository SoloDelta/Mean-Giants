using FPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
   public void Resume()
    {
        GameManager.Instance.UnpausedState();
    }

    public void RespawnPlayer()
    {
        GameManager.Instance.UnpausedState();
        GameManager.Instance.playerScript.SpawnPlayer();
    }

    public void Restart()
    {
        GameManager.Instance.UnpausedState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit(); 
    }
}
