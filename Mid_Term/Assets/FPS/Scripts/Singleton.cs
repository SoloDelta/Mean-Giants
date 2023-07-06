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
    public class Singleton<T> : MonoBehaviour where T : class
    {
        public static T instance
        {
            private set;
            get;
        }

        protected Singleton()
        {

        }

        protected void Awake()
        {
            if(Singleton<T>.instance != default)
                return;

            Singleton<T>.instance = (T)Convert.ChangeType(this, typeof(T));
            GameObject.DontDestroyOnLoad(this);
        }
    }
}
