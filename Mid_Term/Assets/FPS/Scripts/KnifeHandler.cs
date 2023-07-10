using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeHandler : MonoBehaviour
{
    public GameObject Knife;
    public bool CanAttack = true;
    public float AttackCooldown = 1.0f;
    

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (CanAttack)
            {
                KnifeAttack();
            }
        }
    }

    public void KnifeAttack()
    {
        CanAttack = false;
        Animator anim = Knife.GetComponent<Animator>();
        anim.SetTrigger("Attack");
        StartCoroutine(ResetAttackCD());

    }

    IEnumerator ResetAttackCD()
    {
        yield return new WaitForSeconds(AttackCooldown);
        CanAttack = true;
    }
}
