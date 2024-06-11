using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private float patrolSpeed;
    [SerializeField] public float damageToPlayer = 10f;

    // Start is called before the first frame update
    public virtual void Start()
    {
        StartCoroutine(Patrol());
    }

    // Update is called once per frame
    public virtual void Update()
    {
  
    }

    public virtual IEnumerator Patrol()
    {
        while (true)
        {
            for(int i = 0; i < wayPoints.Length; i++)
            {
                yield return StartCoroutine(Move(wayPoints[i]));
            }
        }
    }

    public virtual IEnumerator Move(Transform wayPoint)
    {
        while(transform.position != wayPoint.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, wayPoint.position, patrolSpeed * Time.deltaTime);
            ViewToDirection(wayPoint);
            yield return null;
        }
    }

    public virtual void ViewToDirection(Transform direction)
    { 
        if(direction.position.x > transform.position.x)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if(direction.position.x < transform.position.x)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }
}
