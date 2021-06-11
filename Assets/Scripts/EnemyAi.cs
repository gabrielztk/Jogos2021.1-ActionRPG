using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAi : MonoBehaviour
{
    public UnityEngine.AI.NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health;

    //Patroling
    public Vector3 walkPoint;
    public Vector3 currentPosition;
    public bool walkPointSet;
    public float walkPointMaxRange;
    private float walkPointRange;

    public float patrol_distance;
    private Vector3 initial_postion;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;
    public string enemyType;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public Transform attackPoint;

    GameManager gm;

    public GameObject character;
    public Animator character_animator;

    private bool moving;
    private bool attacking;

    private void Start()
    {
        gm = GameManager.GetInstance();
        if (enemyType == "melee")
        {
            attackRange = 1.5f;
        }
    }

    private void Awake()
    {
        attacking = false;
        moving = true;
        player = GameObject.Find("Player").transform;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        initial_postion = transform.position;
        currentPosition = transform.position;
    }

    private void Update()
    {
        if (gm.gameState != GameManager.GameState.GAME)
        {
            agent.SetDestination(currentPosition);
            return;
        }
        currentPosition = transform.position;
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(currentPosition, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(currentPosition, attackRange, whatIsPlayer);
        bool outOfRange = (currentPosition - initial_postion).magnitude >= patrol_distance;

        if (outOfRange)
        {
            walkPointSet = true;
            agent.SetDestination(initial_postion);
        }

        if (!playerInSightRange && !playerInAttackRange && !outOfRange && !attacking) Patroling();
        if (playerInSightRange && !playerInAttackRange && !outOfRange && !attacking) ChasePlayer();
        if (playerInAttackRange && playerInSightRange && !outOfRange) AttackPlayer();

        
        
    }

    private void Move()
    {
        if (!attacking)
        {
            character_animator.SetFloat("Velocity", agent.velocity.magnitude/agent.speed);
            character_animator.SetBool("Moving", true);
            character_animator.ResetTrigger("Trigger");
        }
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = currentPosition - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 2.0f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        walkPointRange = Random.Range(1.0f, walkPointMaxRange);
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(currentPosition.x + randomX, currentPosition.y, currentPosition.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround) && (initial_postion - walkPoint).magnitude <= patrol_distance)
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        walkPointSet = true;
        agent.SetDestination(currentPosition);
        

        if (!alreadyAttacked)
        {
            transform.LookAt(player);
            if (enemyType == "melee")
            {
                Collider[] hitPlayer = Physics.OverlapSphere(attackPoint.transform.position, attackRange, whatIsPlayer);
                foreach(Collider player in hitPlayer)
                {
                    Debug.Log("hit player");
                    player.GetComponent<PlayerController>().TakeDamage(4);
                }
            }
            else
            {
                GameObject arrow = Instantiate(projectile, attackPoint.transform.position, Quaternion.identity);
                arrow.transform.LookAt(player);

                Quaternion rotation=new Quaternion();
                rotation.eulerAngles = arrow.transform.eulerAngles;
                rotation.z = 0.0f;
                arrow.transform.rotation=rotation;

                Rigidbody rb = arrow.GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            }      

            alreadyAttacked = true;
            StartCoroutine(AttackRoutine());
        }

        
    }

    IEnumerator AttackRoutine()
    {
        attacking = true;
        if (enemyType == "melee")
        {
            character_animator.SetBool("Moving", false);
            character_animator.SetInteger("Trigger Number", 2);
            character_animator.SetTrigger("Trigger");
            yield return new WaitForSeconds(timeBetweenAttacks);
            character_animator.SetBool("Moving", true);
            character_animator.ResetTrigger("Trigger");
            alreadyAttacked = false;
        }
        else
        {
            character_animator.SetFloat("Velocity", 0.0f);
            character_animator.SetBool("Moving", false);
            character_animator.ResetTrigger("Trigger");
            yield return new WaitForSeconds(timeBetweenAttacks);
            character_animator.SetBool("Moving", true);
            character_animator.SetFloat("Velocity", 1.0f);
            alreadyAttacked = false;
        }
        attacking = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(currentPosition, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(currentPosition, sightRange);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(initial_postion, patrol_distance);
        if (enemyType == "melee")
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackPoint.transform.position, attackRange);
        }
    }
}