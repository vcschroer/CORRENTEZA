using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public float fallLimit = -10f;

    private Rigidbody rb;
    private bool isGrounded;

    string nomeScene;

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
        nomeScene = SceneManager.GetActiveScene().name;
        Vector3 movement;
        if (nomeScene.ToLower().Contains("interior"))
        {
            movement = new Vector3(moveInput.x, 0, 0).normalized;
        }
        else
        {
            movement = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        }

        Vector3 targetVelocity = movement * speed;
        Vector3 currentVelocity = rb.linearVelocity;

        Vector3 velocityChange = new Vector3(
            targetVelocity.x - currentVelocity.x,
            0,
            targetVelocity.z - currentVelocity.z
        );

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
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

        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            currentPlatform = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            if (boia)
            {
                isGrounded = true;
            }
            else
            {
                Die(spawnPosition);
            }
        }
    }
}