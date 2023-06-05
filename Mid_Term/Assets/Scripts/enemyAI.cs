using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Project: Mean_Giants Midterm
public class enemyAI : MonoBehaviour, IDamage
{
    [Header("-----Components-----")]
    [SerializeField] Renderer model;

    [Header("-----Enemy Stats-----")]
    [SerializeField] int HP;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(int dmg)
    {
        HP -= dmg;
        StartCoroutine(flashColor());
        if(HP <= 0)
        {
            //decrement enemies remaining in GM
            Destroy(gameObject);
        }
    }

    IEnumerator flashColor()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }
}
