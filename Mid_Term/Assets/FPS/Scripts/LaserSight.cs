using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class LaserSight : MonoBehaviour
{

    LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, transform.position);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit))
        {
            lineRenderer.SetPosition(1, hit.point);

        }
        else
        {
            lineRenderer.SetPosition(1, transform.position +  (transform.forward * 5000));
        }
    }
}
