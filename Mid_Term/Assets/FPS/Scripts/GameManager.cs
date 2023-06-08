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
using UnityEngine.UI;

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
         */

        [Header("---User Interface---")]
        public GameObject activeMenu;
        public GameObject pauseMenu;
        public GameObject winScreen;
        public GameObject loseScreen;
        public Image playerBar;
        public GameObject playerFlashUI;


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
            base.Awake();
        }

        /**-----------------------------------------------------------------
         * @brief MonoBehaviour Override.
         */
        public void Update()
        {
            if (Input.GetButtonDown("Cancel") && activeMenu == null)
            {
                isPaused = !isPaused;
                activeMenu = pauseMenu;
                pauseMenu.SetActive(isPaused);
                statePaused();

               


            }
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

        public void statePaused()
        {
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        public void stateUnpaused()
        {
            Time.timeScale = this.timeScale;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            isPaused = !isPaused;
            activeMenu.SetActive(false);
            activeMenu = null; 
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
            activeMenu = winScreen;
            activeMenu.SetActive(true);
            statePaused();

            //this.Pause(true);
        }

        public void Youlose()
        {
            activeMenu = loseScreen;
            activeMenu.SetActive(true);
            statePaused();
        }
        

    }
}
