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

namespace FPS
{
    /**----------------------------------------------------------------
     * @brief
     */
    public interface IHealth
    {
        void healthPickup(int amount, GameObject obj);
    }
}