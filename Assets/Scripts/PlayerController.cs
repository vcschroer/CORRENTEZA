using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public float fallLimit = -10f;
    public Vector3 playerInitialPosition;
    private int lastDirection = 0;

    private Rigidbody rb;
    private bool isGrounded;
    public bool isJumping;
    public bool playingJumpAnim = false;
    public bool sleeping = true;
    public bool isInWater = false;

    string nomeScene;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject shadow;
    private Quaternion shadowDefaultRotation;

    public bool boia
    {
        get { return GameManager.Instance != null ? GameManager.Instance.temBoia : false; }
        set { if (GameManager.Instance != null) GameManager.Instance.temBoia = value; }
    }

    private PlayerInputActions inputActions;
    private Vector2 moveInput;

    private Vector3 spawnPosition;

    private Transform currentPlatform;
    private Vector3 lastPlatformPosition;

    void Awake()
    {
        inputActions = new PlayerInputActions();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => Jump();

        DontDestroyOnLoad(gameObject);

        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        spawnPosition = transform.position;
        shadowDefaultRotation = shadow.transform.localRotation;
        playerInitialPosition = transform.position;

        StartCoroutine(WakeUp());
    }

    void FixedUpdate()
    {
        ApplyPlatformMovement();
        Move();
    }

    void Update()
    {
        CheckFall();
    }

    void Move()
    {
        int direction;
        nomeScene = SceneManager.GetActiveScene().name;
        Vector3 movement;
        if (nomeScene.ToLower().Contains("interior"))
        {
            movement = new Vector3(moveInput.x, 0, 0).normalized;
            if (sleeping)
            {
                animator.SetInteger("Direction", 12);//dormindo
                lastDirection = 12;
            }
            else if (isInWater)
            {
                animator.SetInteger("Direction", 11); //nadando
                lastDirection = 11;
            }
            else if (movement.sqrMagnitude < 0.01f && !isJumping)
            {
                animator.SetInteger("Direction", 9); // Idle
                lastDirection = 9;
            }
            else
            {
                if (isJumping)
                {
                    animator.SetInteger("Direction", 13);//side jump
                    lastDirection = 13;
                }
                else
                {
                    animator.SetInteger("Direction", 10); // Side
                    lastDirection = 10;
                }
            }


            direction = animator.GetInteger("Direction");
            if (movement.x != 0 && direction != 12)
            {
                spriteRenderer.flipX = movement.x < 0;
            }
        }
        else
        {
            movement = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            if (movement.sqrMagnitude < 0.01f && !isJumping)
            {
                animator.SetInteger("Direction", 0); // Idle
            }
            else if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y) && !playingJumpAnim)
            {
                if (isJumping)
                {
                    animator.SetInteger("Direction", 6);//side jum
                    lastDirection = 6;
                }
                else
                {
                    animator.SetInteger("Direction", 3); // Side
                    lastDirection = 3;
                }
            }
            else if (moveInput.y > 0 && !playingJumpAnim)
            {
                if (isJumping)
                {
                    animator.SetInteger("Direction", 7);//up jump
                    lastDirection = 6;
                }
                else
                {
                    animator.SetInteger("Direction", 1); // Up
                    lastDirection = 1;
                }
            }
            else if (!playingJumpAnim)
            {
                //colocar mais um ifelse aqui para a animacao de pulo para baixo assim q ela estiver disponivel

                animator.SetInteger("Direction", 2); // Down
                lastDirection = 2;
            }

            if (movement.sqrMagnitude < 0.01f && !isJumping)
            {
                switch (lastDirection)
                {
                    case 1:
                        animator.SetInteger("Direction", 5); // IdleUp
                        break;
                    case 2:
                        animator.SetInteger("Direction", 0); // IdleDown
                        break;
                    case 3:
                        animator.SetInteger("Direction", 4); // IdleSide
                        break;
                    case 6:
                        animator.SetInteger("Direction", 4);
                        break;
                    case 7:
                        animator.SetInteger("Direction", 5);
                        break;
                    case 8:
                        animator.SetInteger("Direction", 0);
                        break;
                }
            }
            if (movement.x != 0)
            {
                spriteRenderer.flipX = movement.x < 0;
            }
        }

        direction = animator.GetInteger("Direction");
        if(direction != 12)// se n ta dormindo, pode andar
        {
            Vector3 targetVelocity = movement * speed;
            Vector3 currentVelocity = rb.linearVelocity;

            Vector3 velocityChange = new Vector3(
                targetVelocity.x - currentVelocity.x,
                0,
                targetVelocity.z - currentVelocity.z
            );

            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        if (nomeScene.ToLower().Contains("interior"))
        {
            shadow.SetActive(false);
        }
        else
        {
            shadow.SetActive(true);
            if (direction == 3 || direction == 4 || direction == 6)
            {
                shadow.transform.localRotation = shadowDefaultRotation * Quaternion.Euler(0, 0, 90);
                shadow.transform.localPosition = new Vector3(0f, playerInitialPosition.y - transform.position.y, -0.5f);
            }
            else
            {
                shadow.transform.localRotation = shadowDefaultRotation;
                shadow.transform.localPosition = new Vector3(0f, playerInitialPosition.y - transform.position.y, -0.3f);
            }
        }
    }

    void ApplyPlatformMovement()
    {
        if (currentPlatform != null)
        {
            Vector3 platformDelta = currentPlatform.position - lastPlatformPosition;
            rb.MovePosition(rb.position + platformDelta);
            lastPlatformPosition = currentPlatform.position;
        }
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
        }
    }

    void CheckFall()
    {
        if (transform.position.y < fallLimit)
        {
            Die(spawnPosition);
        }
    }

    public void Die(Vector3 respawnPosition)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        GameObject spawnPoint = GameObject.Find("DefaultSpawnPoint");
        if (spawnPoint != null)
        {
            respawnPosition = spawnPoint.transform.position;
        }

        transform.position = respawnPosition;
        currentPlatform = null;
    }

    void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
        playerInitialPosition = transform.position;

        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            if (currentPlatform != collision.transform)
            {
                currentPlatform = collision.transform;
                lastPlatformPosition = currentPlatform.position;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
        if (collision.gameObject.CompareTag("Water"))
        {
            isInWater = false;
        }

        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            currentPlatform = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isJumping = false;
        playingJumpAnim = false;

        if (collision.gameObject.CompareTag("Water"))
        {
            if (boia || nomeScene.ToLower().Contains("interior"))
            {
                isGrounded = true;
                isInWater = true;
            }
            else
            {
                Die(spawnPosition);
            }
        }
    }

    IEnumerator WakeUp()
    {
        yield return new WaitForSeconds(5f);
        sleeping = false;
    }
}