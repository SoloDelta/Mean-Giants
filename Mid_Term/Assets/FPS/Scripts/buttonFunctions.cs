using FPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
   public void Resume()
    {
        GameManager.instance.stateUnpaused();
    }

    public void Restart()
    {
        GameManager.instance.stateUnpaused();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit(); 
    }
}
