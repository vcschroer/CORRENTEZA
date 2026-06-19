using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 10, -6);
    public float smoothSpeed = 5f;
    string nomeScene;

    private bool hasSnappedToTarget = false;

    void Start()
    {
        nomeScene = SceneManager.GetActiveScene().name;
        FindPlayerTarget();
        SnapToTargetIfPossible();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        nomeScene = scene.name;
        hasSnappedToTarget = false;
        FindPlayerTarget();
        SnapToTargetIfPossible();
    }

    void LateUpdate()
    {
        if (target == null)
        {
            FindPlayerTarget();
            if (target == null) return;
        }

        if (!hasSnappedToTarget)
        {
            SnapToTargetIfPossible();
        }

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        if (nomeScene.ToLower().Contains("interior"))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(60f, 0f, 0f);
        }
    }

    void FindPlayerTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }

    void SnapToTargetIfPossible()
    {
        if (target == null || !target.gameObject.activeInHierarchy) return;

        transform.position = target.position + offset;
        transform.rotation = nomeScene.ToLower().Contains("interior")
            ? Quaternion.Euler(0f, 0f, 0f)
            : Quaternion.Euler(60f, 0f, 0f);

        hasSnappedToTarget = true;
    }
}