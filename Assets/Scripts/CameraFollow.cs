using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 10, -6);
    public float smoothSpeed = 5f;

    void Start()
    {
        FindPlayerTarget();
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

        transform.rotation = Quaternion.Euler(60f, 0f, 0f);
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