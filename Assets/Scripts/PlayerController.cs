using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //VARIABLES
    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

    private Vector3 moveDirection;
    private Vector3 angledMoveDirection;
    private Vector3 velocity;

    [SerializeField] private bool isGrounded;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float gravity;

    [SerializeField] private float jumpHeight;
    [SerializeField] private float maxHealth;
    [SerializeField] private float maxStamina;

    // Camera
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    public Transform camera; 

    public GameObject twoHanded;
    public Animator twoHanded_animator;
    public GameObject archer;
    public Animator archer_animator;
    public GameObject archer_arrow;
    public GameObject knight;
    public Animator knight_animator;
    public GameObject mage;
    public Animator mage_animator;
    public GameObject mage_fireball;

    private List<GameObject> characters;
    private List<Animator> character_animators;
    public List<float> character_attack_ranges;
    public List<GameObject> character_projectiles;
    public Transform attackPoint;
    public Transform projectileGuide;

    private GameObject current_character;
    private Animator current_character_animator;

    private int max_character_num = 4;
    private int character_num = 0;
    private List<float> attack_time;

    public Slider health_slider;
    public Slider stamina_slider;
    private float stamina_cost = 0.6f;

    GameManager gm;
    // https://www.mixamo.com/#/?page=1&query=archer

    public AudioClip ambience;
    private bool ambience_playing;

    public AudioClip twoHanded_SFX;
    public AudioClip archer_SFX;
    public AudioClip knight_SFX;
    public AudioClip mage_SFX;

    private List<AudioClip> character_audios;

    //REFERENCES
    private CharacterController controller;
    [SerializeField] private LayerMask whatIsEnemy;

    private void Start()
    {
        gm = GameManager.GetInstance();
        
        controller = GetComponent<CharacterController>();
        characters = new List<GameObject>(max_character_num);
        character_animators = new List<Animator>(max_character_num);
        character_audios = new List<AudioClip>(max_character_num);
        character_attack_ranges = new List<float>(max_character_num);
        character_projectiles = new List<GameObject>(max_character_num);

        characters.Add(twoHanded);
        character_animators.Add(twoHanded_animator);
        character_audios.Add(twoHanded_SFX);
        character_attack_ranges.Add(2.0f);
        character_projectiles.Add(null);

        characters.Add(archer);
        character_animators.Add(archer_animator);
        character_audios.Add(archer_SFX);
        character_attack_ranges.Add(0.0f);
        character_projectiles.Add(archer_arrow);

        characters.Add(knight);
        character_animators.Add(knight_animator);
        character_audios.Add(knight_SFX);
        character_attack_ranges.Add(1.5f);
        character_projectiles.Add(null);

        characters.Add(mage);
        character_animators.Add(mage_animator);
        character_audios.Add(mage_SFX);
        character_attack_ranges.Add(0.0f);
        character_projectiles.Add(mage_fireball);

        for (int i = 0; i < max_character_num; i++)
        {
            characters[i].SetActive(false);
        }

        attack_time = new List<float>(){1.2f, 0.5f, 0.5f, 1.2f};

        StarCharacter(character_num);

        runSpeed = 2 * walkSpeed;

        AudioManager.SetAmbience(ambience);
        ambience_playing = false;

        health_slider.maxValue = maxHealth;
        health_slider.value = maxHealth;
        health_slider.minValue = 0.0f;

        stamina_slider.maxValue = 1.0f;
        stamina_slider.minValue = 0.0f;
    }

    private void Update()
    {
        if (gm.gameState != GameManager.GameState.GAME)
        {
            PauseAmbience();
            return;
        } 

        if (Input.GetKeyDown(KeyCode.Escape) && gm.gameState == GameManager.GameState.GAME) {
            gm.ChangeState(GameManager.GameState.PAUSE);
            return;
        }

        PlayAmbience();
        ChangeCharacter();
        Movement();
    }

    private void PauseAmbience()
    {
        if (ambience_playing)
        {
            AudioManager.PauseAmbience();
            ambience_playing = false;
        }
    }

    private void PlayAmbience()
    {
        if (!ambience_playing)
        {
            AudioManager.PlayAbience();
            ambience_playing = true;
        }
    }

    private void Movement()
    {
        isGrounded = controller.isGrounded;
        // isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        moveDirection = new Vector3(moveX, 0, moveZ).normalized;
        angledMoveDirection = moveDirection;
 

        if (isGrounded && character_animators[character_num].GetBool("Attacking") != true)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Attack();
            }
            else if (moveDirection != Vector3.zero && (!Input.GetKey(KeyCode.LeftShift) || stamina_slider.value == 0.0f))
            {
                Walk();
            }
            else if (moveDirection != Vector3.zero && Input.GetKey(KeyCode.LeftShift) && stamina_slider.value > 0.0f)
            {
                Run();
            }
            else if (moveDirection == Vector3.zero)
            {
                Idle();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }

            angledMoveDirection *= moveSpeed;
        }

        
        // Horizontal movement
        controller.Move(angledMoveDirection * Time.deltaTime);
        // Vertical movement
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void CharecterRotation()
    {
        float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

        transform.rotation = Quaternion.Euler(0f, angle, 0f);
        angledMoveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
    }

    private void Idle()
    {   
        stamina_slider.value += stamina_cost * Time.deltaTime;
        moveSpeed = 0f;
        character_animators[character_num].SetFloat("Velocity", 0.0f);
        character_animators[character_num].SetBool("Moving", false);
        character_animators[character_num].ResetTrigger("Trigger");
    }

    private void Walk()
    {
        stamina_slider.value += stamina_cost/4 * Time.deltaTime;
        moveSpeed = walkSpeed;
        character_animators[character_num].SetFloat("Velocity", 0.5f);
        character_animators[character_num].SetBool("Moving", true);
        character_animators[character_num].ResetTrigger("Trigger");
        CharecterRotation();
    }

    private void Run()
    {
        stamina_slider.value -= stamina_cost * Time.deltaTime;
        moveSpeed = runSpeed;
        character_animators[character_num].SetFloat("Velocity", 1.0f);
        character_animators[character_num].SetBool("Moving", true);
        character_animators[character_num].ResetTrigger("Trigger");
        CharecterRotation();
    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        character_animators[character_num].SetBool("Moving", false);
        character_animators[character_num].SetInteger("Trigger Number", 1);
        character_animators[character_num].SetTrigger("Trigger");
    }

    void Attack()
    {   
        Debug.Log("Attack");
        AudioManager.PlaySFX(character_audios[character_num]);
        StartCoroutine(AttackRoutine());
        if (character_attack_ranges[character_num] > 0)
        {
            StartCoroutine(MeleeAttack());
        }
        else 
        {
            StartCoroutine(RangedAttack());
        }
    }

    IEnumerator AttackRoutine()
    {
        character_animators[character_num].SetBool("Moving", false);
        character_animators[character_num].SetInteger("Trigger Number", 2);
        character_animators[character_num].SetTrigger("Trigger");
        character_animators[character_num].SetBool("Attacking", true);
        yield return new WaitForSeconds(attack_time[character_num]);
        character_animators[character_num].SetBool("Attacking", false);
        character_animators[character_num].ResetTrigger("Trigger");
    }

    private void ChangeCharacter()
    {
        if (Input.GetKey(KeyCode.Alpha1) && character_num != 0)
        {
            StarCharacter(0);
        }
        else if (Input.GetKey(KeyCode.Alpha2) && character_num != 1)
        {
            StarCharacter(1);
        }
        else if (Input.GetKey(KeyCode.Alpha3) && character_num != 2)
        {
            StarCharacter(2);
        }
        else if (Input.GetKey(KeyCode.Alpha4) && character_num != 3)
        {
            StarCharacter(3);
        }
    }

    void StarCharacter(int id)
    {
        characters[character_num].SetActive(false);
        character_num = id;
        characters[character_num].SetActive(true);
        StarAnimator(character_animators[character_num]);
        StartCoroutine(ChangeCharacterRoutine());
        
    }

    IEnumerator ChangeCharacterRoutine()
    {
        yield return new WaitForSeconds(2);
    }

    IEnumerator MeleeAttack()
    {
        yield return new WaitForSeconds(attack_time[character_num] -1);
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.transform.position, character_attack_ranges[character_num], whatIsEnemy);
        foreach(Collider enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyAi>().TakeDamage(4);
        }
    }

    IEnumerator RangedAttack()
    {
        Debug.Log("Ranged");
        yield return new WaitForSeconds(attack_time[character_num] -1);
        GameObject projectile = Instantiate(character_projectiles[character_num], attackPoint.transform.position, Quaternion.identity);
        projectile.transform.LookAt(projectileGuide);

        Quaternion rotation=new Quaternion();
        rotation.eulerAngles = projectile.transform.eulerAngles;
        rotation.z = 0.0f;
        projectile.transform.rotation=rotation;

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
    }

    void StarAnimator(Animator animator)
    {
        animator.SetBool("Moving", false);
        animator.SetFloat("Animation Speed", 1.0f);
        animator.SetFloat("Velocity", 0.0f);
        animator.SetInteger("Jumping", 1);
        animator.SetInteger("Action", 0);
        animator.SetInteger("Trigger Number", 0);
        animator.ResetTrigger("Trigger");
        animator.SetBool("Attacking", false);
        
    }

    public void TakeDamage(int damage)
    {
        health_slider.value -= damage;

        if (health_slider.value <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.transform.position, character_attack_ranges[character_num]);
    }
}
