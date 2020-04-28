using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    [Header("Player detection")]
    public GameObject player;
    [SerializeField] [Range(10, 100)] int playerDetectionRadius = 40;

    [Header("AI Navigation")]
    public float distanceWithPlayer;
    public NavMeshAgent navMeshAgent;

    [Header("Enemy animator")]
    public Animator animatorEnemy;
    [SerializeField] bool isWalking = true;
    [SerializeField] bool isAttacking = true;

    void Start()
    {
        // Initialize values
        // Player instances
        player = GameObject.Find("Player");

        // Enemy instances (own)
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        animatorEnemy = this.GetComponent<Animator>();
    }

    void Update()
    {
        // Get the distance between the enemy and the player
        distanceWithPlayer = Vector3.Distance(this.transform.position, player.transform.position);

        if(distanceWithPlayer < playerDetectionRadius && distanceWithPlayer > navMeshAgent.stoppingDistance)
        {
            // Update Movement
            navMeshAgent.SetDestination(player.transform.position);

            // Update Rotation
            transform.LookAt(player.transform.position);

            // Walk animation
            isWalking = true;
            animatorEnemy.SetBool("isWalking", isWalking);

            // Attack animation
            isAttacking = false;
            animatorEnemy.SetBool("isAttacking", isAttacking);
        }
        else if (distanceWithPlayer < navMeshAgent.stoppingDistance)
        {
            // Update Rotation
            transform.LookAt(player.transform.position);

            // Walk animation
            isWalking = false;
            animatorEnemy.SetBool("isWalking", isWalking);

            // Attack animation
            isAttacking = true;
            animatorEnemy.SetBool("isAttacking", isAttacking);
        }
        else 
        {
            // Update Movement
            navMeshAgent.ResetPath();

            // Walk animation
            isWalking = false;
            animatorEnemy.SetBool("isWalking", isWalking);

            // Attack animation
            isAttacking = false;
            animatorEnemy.SetBool("isAttacking", isAttacking);
        }
    }

    // Show agression range with the gizmo on the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }
}
