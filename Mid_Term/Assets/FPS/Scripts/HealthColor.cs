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
    public class HealthColor : MonoBehaviour
    {
        [SerializeField] private Gradient healthBarGradient;
        public float healthCheck = 1f;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            GameManager.instance.playerHpBar.color = healthBarGradient.Evaluate(GameManager.instance.playerHpBar.fillAmount);
        }
    }
}