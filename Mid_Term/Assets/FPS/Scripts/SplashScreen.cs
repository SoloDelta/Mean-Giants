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
    public class SplashScreen : MonoBehaviour
    {   
        [SerializeField] private GameObject m_DirectionalLight;

        /**----------------------------------------------------------------
         * @brief Monobehaviour override.
         */
        private void Start()
        {
            //-----------------------------------------------------------------
            // Load next level
            Loader.instance.LoadNextLevel();
        }

        /**----------------------------------------------------------------
         * @brief Monobehaviour override.
         */
        private void Update()
        {
            this.m_DirectionalLight.GetComponent<Transform>().Rotate(new Vector3(0.0f, -0.1f, 0.0f));
        }
    }
}
