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
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FPS
{
    /**----------------------------------------------------------------
     * @brief
     */
    public class Loader : Singleton<Loader>
    {
        [SerializeField] private Animator m_Animator;
        public float transitionSpeed;

        /**----------------------------------------------------------------
         * @brief
         */
        private new void Awake()
        {
            base.Awake();
        }

        public void LoadNextLevel()
        {
            StartCoroutine(LoadLevel());
        }

        IEnumerator LoadLevel()
        {
            yield return new WaitForSeconds(this.transitionSpeed);

            this.m_Animator.SetBool("FadeOut", true);

            yield return new WaitForSeconds(2);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

            yield return new WaitForSeconds(this.transitionSpeed);

            this.m_Animator.SetBool("FadeOut", false);
            this.m_Animator.SetBool("FadeIn", true);
        }
    }
}
