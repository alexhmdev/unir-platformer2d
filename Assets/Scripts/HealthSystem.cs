using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthSystem : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] private float health;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if(anim != null) {
            anim.SetTrigger("hit");
        }
        if (health <= 0 && !CompareTag("Player"))
        {
            if(anim != null) {
                anim.SetTrigger("die");
                Invoke("Die", 1f);
            }
        }
    }

    public float GetHealth()
    {
        return health;
    }

    private void Die()
    {
        Destroy(gameObject);
    }


}
