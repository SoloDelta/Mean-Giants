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
     * @brief
     */
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private int sensitivity;

        [SerializeField] private int lockVerMin;
        [SerializeField] private int lockVerMax;

        [SerializeField] private bool invertY;

        private float xRotation;

        /**----------------------------------------------------------------
         * @brief MonoBehaviour override.
         */
        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        /**----------------------------------------------------------------
         * @brief MonoBehaviour override.
         */
        private void Update()
        {
            //get input
            float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
            float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;

            if (invertY)
            {
                xRotation += mouseY;
            }
            else
            {
                xRotation -= mouseY;
            }

            //clamp the camera rotation on the X-axis
            xRotation = Mathf.Clamp(xRotation, lockVerMin, lockVerMax);

            //rotate the camera on the X-axis
            transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

            //rotate the player on the Y-axis
            transform.parent.Rotate(Vector3.up * mouseX);
        }
    }
}
