
using UnityEngine;
using UnityEngine.AI;

public class EnemyAiMelee : MonoBehaviour
{
    public NavMeshAgent agent;

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

    private void Start()
    {
        gm = GameManager.GetInstance();
    }

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        initial_postion = transform.position;
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

        if (!playerInSightRange && !playerInAttackRange && !outOfRange) Patroling();
        if (playerInSightRange && !playerInAttackRange && !outOfRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange && !outOfRange) AttackPlayer();

        
        
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
        agent.SetDestination(currentPosition);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.transform.position, attackRange, whatIsPlayer);
            foreach(Collider enemy in hitEnemies)
            {
                
            }    

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
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
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.transform.position, attackRange);
    }
}