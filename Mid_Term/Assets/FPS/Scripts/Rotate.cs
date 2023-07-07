using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Rotate : MonoBehaviour
{
    [Header("--- Up and Down Movement ---")]
    [Range(.5f, 15f)][SerializeField] float speed;
    [Range(0.1f, 5f)][SerializeField] float height;

    [Header("--- Rotation ---")]
    [SerializeField] int xPos;
    [SerializeField] int yPos;
    [SerializeField] int zPos;

    Vector3 pos;

    private void Start()
    {
        pos = transform.position;
    }
    void Update()
    {
        transform.Rotate(new Vector3(xPos, yPos, zPos) * Time.deltaTime);

        float moveY = Mathf.Sin(Time.time * speed) * height + pos.y;

        transform.position = new Vector3(transform.position.x, moveY, transform.position.z);
    }
}
