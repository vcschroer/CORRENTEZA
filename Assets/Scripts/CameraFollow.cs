using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 10, -6);
    public float smoothSpeed = 5f;

    string nomeScene;


    void Start()
    {
        FindPlayerTarget();
        nomeScene = SceneManager.GetActiveScene().name;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            FindPlayerTarget();
            if (target == null) return; 
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
}