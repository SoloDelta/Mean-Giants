/**
 * Copyright (c) 2023 - 2023, The Mean Giants, All Rights Reserved.
 *
 * Authors
 *  - 
 */

//-----------------------------------------------------------------
// Using Namespaces
//-----------------------------------------------------------------
using System;
using UnityEngine;

namespace FPS
{
    /**----------------------------------------------------------------
     * @brief Utility class to quickly create singletons.
     */
    public class Singleton<T> : MonoBehaviour where T : class
    {
        /**----------------------------------------------------------------
         * @brief property to access singleton instance.
         */
        public static T instance
        {
            private set;
            get;
        }

        /**----------------------------------------------------------------
         * @brief Default constructor.
         */
        protected Singleton()
        {
            //-----------------------------------------------------------------
            // Leave blank.
        }

        /**----------------------------------------------------------------
         * @brief MonoBehaviour override
         */
        public void Awake()
        {
            //-----------------------------------------------------------------
            if(Singleton<T>.instance != default)
                return;

            //-----------------------------------------------------------------
            Singleton<T>.instance = (T)Convert.ChangeType(this, typeof(T));
            GameObject.DontDestroyOnLoad(this);
        }
    }
}
