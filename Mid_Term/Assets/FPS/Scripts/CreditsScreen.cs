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
using UnityEngine.SceneManagement;

namespace FPS
{
    public class CreditsScreen : MonoBehaviour
    {   
        public Animator animator;

        /**----------------------------------------------------------------
         * @brief Monobehaviour override.
         */
        private void Start()
        {

        }

        /**----------------------------------------------------------------
         * @brief Monobehaviour override.
         */
        private void Update()
        {
            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                SceneManager.LoadScene("MainMenu");
                GameManager.instance.PausedState();
            }
        }
    }
}
