using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float followSpeed = 5f;

    private Vector3 originalPosition;

    // The camera can't move further left than where it started
    private float minX;

    void Start()
    {
        originalPosition = transform.position;
        minX = transform.position.x;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        float targetX = target.position.x;

        // Stops the camera from moving too far to the left
        targetX = Mathf.Max(targetX, minX);

        Vector3 targetPosition = new Vector3(
            targetX,
            transform.position.y,
            transform.position.z);

        //makes the camera smoothly move toward the bobber instead of instantly snapping to it
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime);
    }

    public void StartFollowing(Transform newTarget)
    {
        target = newTarget;
    }

    public void StopFollowing()
    {
        target = null;
    }
}
