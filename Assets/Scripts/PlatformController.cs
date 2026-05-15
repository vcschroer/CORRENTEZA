using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public enum PlatformType
    {
        Static,
        Falling,
        Moving
    }

    public PlatformType platformType;

    // =========================
    // FALLING PLATFORM
    // =========================
    public float fallDelay = 0.1f;
    public float shakeDuration = 1.5f;
    public float shakeIntensity = 0.05f;
    public float respawnTime = 5f; // 🔥 novo

    private bool playerOnPlatform = false;
    private Vector3 originalPosition;

    // =========================
    // MOVING PLATFORM
    // =========================
    public bool moveVertical = false;
    public float moveDistance = 3f;
    public float moveSpeed = 2f;

    private Vector3 startPos;
    private Rigidbody rb;

    void Start()
    {
        startPos = transform.position;
        originalPosition = transform.position;

        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (platformType == PlatformType.Moving)
        {
            MovePlatform();
        }
    }

    void MovePlatform()
    {
        float movement = Mathf.PingPong(Time.time * moveSpeed, moveDistance);

        Vector3 targetPosition;

        if (moveVertical)
        {
            targetPosition = startPos + Vector3.forward * movement;
        }
        else
        {
            targetPosition = startPos + Vector3.right * movement;
        }

        rb.MovePosition(targetPosition);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (platformType == PlatformType.Falling && collision.gameObject.CompareTag("Player"))
        {
            if (!playerOnPlatform)
            {
                playerOnPlatform = true;
                StartCoroutine(FallRoutine());
            }
        }
    }

    // =========================
    // FALL + SHAKE + RESPAWN
    // =========================
    System.Collections.IEnumerator FallRoutine()
    {
        float timer = 0f;

        // TREMER
        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;

            float offsetX = Random.Range(-shakeIntensity, shakeIntensity);
            float offsetZ = Random.Range(-shakeIntensity, shakeIntensity);

            transform.position = originalPosition + new Vector3(offsetX, 0, offsetZ);

            yield return null;
        }

        transform.position = originalPosition;

        yield return new WaitForSeconds(fallDelay);

        // CAIR
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.isKinematic = false;
        rb.useGravity = true;

        // ESPERA ANTES DE RESPAWNAR
        yield return new WaitForSeconds(respawnTime);

        Respawn();
    }

    // =========================
    // RESPAWN
    // =========================
    void Respawn()
    {
        // reseta física
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.useGravity = false;

        // volta posição
        transform.position = originalPosition;

        // permite cair de novo
        playerOnPlatform = false;
    }
}