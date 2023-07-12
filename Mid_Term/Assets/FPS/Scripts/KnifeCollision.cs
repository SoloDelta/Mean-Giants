using FPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeCollision : MonoBehaviour
{
    public int damageAmount = 10;
    private void OnTriggerEnter(Collider other)
    {
        IDamage damageReceiver = other.GetComponent<IDamage>();
        EnemyAIRefactor enemyAI = other.GetComponent<EnemyAIRefactor>();
        if (enemyAI != null)
        {
            enemyAI.TakeDamage(damageAmount);
        }
    }
}
