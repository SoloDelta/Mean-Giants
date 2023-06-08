/**
 * Copyright (c) 2023 - 2023, The Mean Giants, All Rights Reserved.
 *
 * Authors
 *  - 
 */

//-----------------------------------------------------------------
// Using Namespaces
//-----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    /**----------------------------------------------------------------
     * @brief Simple game state manager to handle overall game data.
     */
    public class GameManager : Singleton<GameManager>
    {
        /**-----------------------------------------------------------------
         * @brief The player game object.
         */
        public GameObject player;

        /**-----------------------------------------------------------------
         * @brief The player controller.
         */
        public PlayerController playerController;

        /**-----------------------------------------------------------------
         * @brief Count of the remaining enemies.
         */
        public int remainingEnemies;

        /**-----------------------------------------------------------------
         * @brief Is the game currently paused.
         */
        public bool isPaused;

        /**-----------------------------------------------------------------
         * @brief The games normal time scale.
         */
        public float timeScale;

        /**-----------------------------------------------------------------
         * @brief Default Constructor.
         */
        public GameManager()
        {
            //------------------------------------------------------------------
            // Leave blank.
        }

        /**-----------------------------------------------------------------
         * @brief MonoBehaviour Override.
         */
        public new void Awake()
        {
            this.timeScale = Time.timeScale;
            this.player = GameObject.FindGameObjectWithTag("Player");
            this.playerController = this.player.GetComponent<PlayerController>();
        }

        /**-----------------------------------------------------------------
         * @brief MonoBehaviour Override.
         */
        public void Update()
        {

        }

        /**-----------------------------------------------------------------
         * @brief Pause / Un-Pause the game.
         */
        public void Pause(bool paused)
        {
            if(paused)
            {
                Time.timeScale = 0;
                this.isPaused = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            } else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Confined;
                Time.timeScale = this.timeScale;
                this.isPaused = false;
            }
        }

        /**-----------------------------------------------------------------
         * @brief Update the games goal by adding more enemies and check
         * to see if we have reached the win condition.
         */
        public void UpdateGameGoal(int amount)
        {
            //------------------------------------------------------------------
            // Add the new amount of enemies to the scene.
            this.remainingEnemies += amount;

            //------------------------------------------------------------------
            // This number should never reach below zero! if it does somethigng
            // is very very broken.            
            if(this.remainingEnemies == 0)
            {
                StartCoroutine(this.Youwin());
            }
        }

        /**-----------------------------------------------------------------
         * @brief Coroutine for the win menu.
         */
        public IEnumerator Youwin()
        {
            yield return new WaitForSeconds(3);
            
            this.Pause(true);
        }
    }
}
