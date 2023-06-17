/**
 * Copyright (c) 2023 - 2023, The Mean Giants, All Rights Reserved.
 *
 * Authors
 *  - 
 */

//-----------------------------------------------------------------
// Using Namespaces
//-----------------------------------------------------------------
using FPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FPS
{
    /**----------------------------------------------------------------
     * @brief Handles button interaction for our UI.
     */
    public class ButtonFunctions : MonoBehaviour
    {
        /**----------------------------------------------------------------
         * @brief
         */
        public void Resume()
        {
            GameManager.instance.UnpausedState();
        }

        /**----------------------------------------------------------------
         * @brief
         */
        public void RespawnPlayer()
        {   
            GameManager.instance.UnpausedState();
            GameManager.instance.playerScript.SpawnPlayer();
        }

        /**----------------------------------------------------------------
         * @brief
         */
        public void Restart()
        {
            GameManager.instance.UnpausedState();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /**----------------------------------------------------------------
         * @brief
         */
        public void Quit()
        {
            Application.Quit(); 
        }
    }
}
