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

namespace FPS
{
    /**----------------------------------------------------------------
     * @brief Interface for implementing damagable objects.
     */
    public interface IDamage
    {
        /**----------------------------------------------------------------
         * @brief
         */
        void TakeDamage(int dmg);
    }
}
