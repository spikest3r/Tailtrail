using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // player
    [Range(0f, 1f)] public float deadZoneWidth = 0.4f;
    [Range(0f, 1f)] public float deadZoneHeight = 0.3f;
    public float smoothSpeed = 5f;

    private Vector3 velocity;

    void FixedUpdate()
    {
        Camera cam = Camera.main;
        Vector3 camPos = transform.position;

        Vector3 lowerLeft = cam.ViewportToWorldPoint(new Vector3(0.5f - deadZoneWidth / 2f, 0.5f - deadZoneHeight / 2f, cam.nearClipPlane));
        Vector3 upperRight = cam.ViewportToWorldPoint(new Vector3(0.5f + deadZoneWidth / 2f, 0.5f + deadZoneHeight / 2f, cam.nearClipPlane));

        Vector3 targetPos = target.position;

        float offsetX = 0f;
        float offsetY = 0f;

        if (targetPos.x < lowerLeft.x)
            offsetX = targetPos.x - lowerLeft.x;
        else if (targetPos.x > upperRight.x)
            offsetX = targetPos.x - upperRight.x;

        if (targetPos.y < lowerLeft.y)
            offsetY = targetPos.y - lowerLeft.y;
        else if (targetPos.y > upperRight.y)
            offsetY = targetPos.y - upperRight.y;

        Vector3 newPos = camPos + new Vector3(offsetX, offsetY, 0);
        transform.position = Vector3.SmoothDamp(camPos, newPos, ref velocity, 1f / smoothSpeed);
    }
}

