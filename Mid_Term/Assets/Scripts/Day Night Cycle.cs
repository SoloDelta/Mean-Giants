using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float currentTime;
    public float dayLengthMinutes;

    float rotationSpeed;
    private void Start()
    {
        rotationSpeed = 360 / dayLengthMinutes / 60;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += 1 * Time.deltaTime;

        transform.Rotate(new Vector3(1, 0, 0) * rotationSpeed * Time.deltaTime);
    }
}
