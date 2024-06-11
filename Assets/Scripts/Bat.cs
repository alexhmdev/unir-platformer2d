using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// This is the Bat class that inherits from EnemyPatrol
public class Bat : Enemy 
{
    [SerializeField] private float chaseSpeed;
    [SerializeField] public Transform player;
    [SerializeField] private Transform attackHitBoxTransform;
    [SerializeField] private float attackHitBoxRadius = 0.5f;
    [SerializeField] private LayerMask affectedLayer;
    private Animator anim;
    private bool isChasing;
    // Start is called before the first frame update
    private void Start()
    {
        anim = GetComponent<Animator>();
        base.Start();
    }

    // Update is called once per frame
    private void Update()
    {

        if (isChasing)
        {
              transform.position =  Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
             ViewToDirection(player);
            Debug.Log(Vector2.Distance(transform.position, player.position));
        }
        base.Update();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Hit");
            anim.SetTrigger("attack");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("DetectionPlayer"))
        {
            Debug.Log("Player Detected");
            StopAllCoroutines();
            isChasing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("DetectionPlayer"))
        {
            Debug.Log("Player Lost");
            //StartCoroutine(ChasePlayer());
            if(gameObject != null)
            {
                StartCoroutine(Patrol());
            }
            isChasing = false;

        }
    }

    void AttackPlayer()
    {
        // create the hitbox
        Collider2D[] hit = Physics2D.OverlapCircleAll(attackHitBoxTransform.position, attackHitBoxRadius, affectedLayer);
        foreach (Collider2D other in hit)
        {
            if(other.CompareTag("Player"))
            {
                other.GetComponent<HealthSystem>().TakeDamage(damageToPlayer);
                other.GetComponent<Player>().OnTakeDamage();
            }
        }
    }
}
