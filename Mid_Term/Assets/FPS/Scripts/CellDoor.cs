using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellDoor : MonoBehaviour
{
    [Header("--- Door ---")]
    public Vector3 endPos;
    [Range(1.0f, 5.0f)] [SerializeField] public float speed;
    [Range(1.0f, 5.0f)] [SerializeField] public float openTime;
    [Range(1.0f, 5.0f)] [SerializeField] public float returnDelay;

    private bool moving = false;
    private bool isOpening = true;
    private Vector3 startPos;
    private float timer = 0.0f;

    // Start is called before the first frame update
    private void Start()
    {
        startPos = transform.localPosition;
    }

    private void Update()
    {
        if (moving)
        {
            MoveDoor();
        }
    }

    private void MoveDoor()
    {
        Vector3 targetPos = isOpening ? endPos : startPos;
        float delay = isOpening ? openTime : returnDelay;

        transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos, speed * Time.deltaTime);

        if (transform.localPosition == targetPos)
        {
            timer += Time.deltaTime;
            if (timer >= delay)
            {
                if (isOpening)
                {
                    moving = true;
                    isOpening = false;
                    timer = 0.0f;
                }
                else
                {
                    moving = false;
                    timer = 0.0f;
                    isOpening = true;
                }
            }
        }
    }

    public bool Moving
    {
        get { return moving; }
        set { moving = value; }
    }
}
