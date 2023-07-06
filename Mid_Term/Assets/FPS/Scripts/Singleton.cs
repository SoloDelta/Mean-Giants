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
        public static T Instance
        {
            private set;
            get;
        }

        protected Singleton()
        {

        }

        protected void Awake()
        {
            if(Singleton<T>.Instance != default)
                return;

            Singleton<T>.Instance = (T)Convert.ChangeType(this, typeof(T));
            GameObject.DontDestroyOnLoad(this);
        }
    }
}
