using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] int damage;
    [SerializeField] int speed;
    [SerializeField] float destroyTime;

    [SerializeField] Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroyTime);
        rb.velocity = transform.forward * speed;
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        IDamage dam = other.GetComponent<IDamage>();

        if(dam != null)
        {
            dam.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
