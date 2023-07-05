/**
 * Copyright (c) 2023 - 2023, The Mean Giants, All Rights Reserved.
 *
 * Authors
 *  - 
 */

//-----------------------------------------------------------------
// Using Namespaces
//-----------------------------------------------------------------
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

        public void PlayGame()
        {
            SceneManager.LoadScene("Main");
            Restart();
        }

        /**----------------------------------------------------------------
        * @brief
        */

        public void PlayTutorial()
        {
            SceneManager.LoadScene("Tutorial");
        }

        /**----------------------------------------------------------------
         * @brief
         */

        public void MainMenu()
        {
            SceneManager.LoadScene("MainMenu");
            GameManager.instance.PausedState();
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


        public void Settings()
        {
            GameManager.instance.activeMenu.SetActive(false);
            GameManager.instance.activeMenu = GameManager.instance.settingsMenu;
            GameManager.instance.activeMenu.SetActive(true);
        }

        public void SettingsReturn()
        {
            GameManager.instance.activeMenu.SetActive(false);
            GameManager.instance.activeMenu = GameManager.instance.pauseMenu;
            GameManager.instance.activeMenu.SetActive(true);
        }

        public void MenuSettings()
        {
            GameManager.instance.activeMenu.SetActive(false);
            GameManager.instance.activeMenu = GameManager.instance.settingsMenu;
            GameManager.instance.activeMenu.SetActive(true);
        }

        public void MenuSettingsReturn()
        {
            GameManager.instance.activeMenu.SetActive(false);
            GameManager.instance.activeMenu = GameManager.instance.pauseMenu;
            GameManager.instance.activeMenu.SetActive(true);
        }

        public void Save()
        {
            DataPersistenceManager.Instance.SaveGame();
        }

        public void Load()
        {
            DataPersistenceManager.Instance.LoadGame();
        }


    }
}
