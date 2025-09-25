using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;          // Drag your Globe here
    public float distance = 25f;      // Default camera distance from center
    public float xSpeed = 120f;       // Horizontal orbit speed
    public float ySpeed = 80f;        // Vertical orbit speed
    public float yMinLimit = -80f;    // Prevents flipping over poles
    public float yMaxLimit = 80f;
    public float zoomSpeed = 10f;     // Mouse scroll zoom speed
    public float minDistance = 5f;
    public float maxDistance = 60f;

    float x = 0f;
    float y = 0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    }

    void LateUpdate()
    {
        if (target)
        {
            // Left mouse drag to orbit
            if (Input.GetMouseButton(0))
            {
                x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
                y = Mathf.Clamp(y, yMinLimit, yMaxLimit);
            }

            // Scroll to zoom
            distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }
}
