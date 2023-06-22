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
    public class KnifeRotation : MonoBehaviour
    {
        [Range(-40, 40)][SerializeField] private int x;
        [Range(-40, 40)][SerializeField] private int y;
        [Range(-40, 40)][SerializeField] private int z;
        void Update()
        {
            transform.Rotate(x,y,z);
        }
    }
}
