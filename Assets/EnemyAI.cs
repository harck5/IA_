using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
        [SerializeField] private Transform player;

        private NavMeshAgent _agent;

    private float visionRange = 20f;
    private float attackRange = 10f;

    private bool playerInVisionRange;
    private bool playerInAttackRange;

    [SerializeField] private LayerMask playerLayer;

    //Patrulla
    [SerializeField] private Transform[] waypoints;
    private int totalWaypoints;
    private int nextPoint;

    //Ataque
    [SerializeField] private GameObject bullet;
    private float timeBetweenAttack = 2f;
    private bool canAttack;
    [SerializeField] private float upAttackForce = 15f;
    [SerializeField] private float forwardAttackForce = 15f;
        private void Awake()
        {
        _agent = GetComponent<NavMeshAgent>();
        }
    private void Start()
    {
        totalWaypoints = waypoints.Length;
        nextPoint = 1;
        canAttack = true;
    }
    private void Update()
    {
        Vector3 pos = transform.position;
        playerInVisionRange = Physics.CheckSphere(pos, visionRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(pos, attackRange, playerLayer);
        //_agent.SetDestination(player.position);
        if(!playerInVisionRange && !playerInAttackRange)
        {
            Patrol();
        }
        if(playerInVisionRange && !playerInAttackRange)
        {
            Chase();
        }
        if(playerInVisionRange && playerInAttackRange)
        {
            Attack();
        }
    }
    private void Patrol()
    {
        if(Vector3.Distance(transform.position, waypoints[nextPoint].position) < 2.5f)
        {
            nextPoint++;
            if (nextPoint == totalWaypoints)
            {
                nextPoint = 0;
            }
            transform.LookAt(waypoints[nextPoint].position);
        }
        _agent.SetDestination(waypoints[nextPoint].position);
    }
    private void Chase()
    {
        _agent.SetDestination(player.position);
        transform.LookAt(player);
    }
    private void Attack()
    {
        _agent.SetDestination(transform.position);
        if (canAttack)
        {
            Rigidbody rigidbody = Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rigidbody.AddForce(transform.forward * forwardAttackForce, ForceMode.Impulse);
            rigidbody.AddForce(transform.up * upAttackForce, ForceMode.Impulse);
            canAttack = false;
            StartCoroutine(AttackCooldown());
        }
    }
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(timeBetweenAttack);
        canAttack = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
