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
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

namespace FPS
{
    /**----------------------------------------------------------------
     * @brief A simple game manager to handle data across states.
     */
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        //-----------------------------------------------------------------
        // Player Variables
        //-----------------------------------------------------------------

        [Header("----- Player Stuff -----")]
        public GameObject player;
        public PlayerController playerScript;
        public GameObject playerSpawnPos;

        //-----------------------------------------------------------------
        // UI Variables
        //-----------------------------------------------------------------

        [Header("----- UI Stuff -----")]
        public GameObject activeMenu;
        public GameObject pauseMenu;
        public GameObject winMenu;
        public GameObject settingsMenu;
        public GameObject settingMainMenu;
        public Image playerHpBar;
        public Image playerStaminaBar;
        public Image playerShieldBar;
        public GameObject playerFlashUI;
        public GameObject loseMenu;
        public TextMeshProUGUI objectiveText;
        public TextMeshProUGUI enemiesToKill;
        public TextMeshProUGUI itemCollectedText;
        public TextMeshProUGUI ammoMaxText;
        public TextMeshProUGUI ammoStorageText;
        public TextMeshProUGUI ammoCurText;
        public TextMeshProUGUI currencyAmount;
        public Image heavy;
        public Image smg;
        public Image assaultRifle;
        public Image pistol;
        public Image hitMark;
        public Image hitMarkKill;
        public GameObject checkPointPopup;
        public GameObject gunPos;


        //-----------------------------------------------------------------
        // Objective Variables
        //-----------------------------------------------------------------

        [Header("----- Objectives -----")]
        public int enemiesRemaining;
        NpcInteract npc;

        public bool isPaused;
        private float timeScaleOriginal;

        /**----------------------------------------------------------------
         * @brief MonoBehaviour override.
         */
        private void Awake()
        {
            instance = this;
            timeScaleOriginal = Time.timeScale;
            player = GameObject.FindGameObjectWithTag("Player");
            playerScript = player.GetComponent<PlayerController>();
            playerSpawnPos = GameObject.FindWithTag("PlayerSpawnPos");
        }

        /**----------------------------------------------------------------
         * @brief MonoBehaviour override.
         */
        private void Update()
        {
            if (Input.GetButtonDown("Cancel") && activeMenu == null)
            {
                PausedState();
                activeMenu = pauseMenu;
                activeMenu.SetActive(isPaused);
            }

        }

        public void PausedState()
        {
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            isPaused = true;
        }

        public void UnpausedState()
        {
            Time.timeScale = timeScaleOriginal;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            isPaused = false;
            activeMenu.SetActive(isPaused);
            activeMenu = null;
        }

        public void UpdateObjective(int amount)
        {
            enemiesRemaining += amount;
            enemiesToKill.text = enemiesRemaining.ToString("F0");

            if (enemiesRemaining <= 0)
            {
                enemiesRemaining = 0;
            }
        }

        public IEnumerator YouWin()
        {
            yield return new WaitForSeconds(1);
            activeMenu = winMenu;
            activeMenu.SetActive(true);
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene("Credits");
        }

        public void YouLose()
        {
            PausedState();
            activeMenu = loseMenu;
            activeMenu.SetActive(true);
        }





    }

 
}
