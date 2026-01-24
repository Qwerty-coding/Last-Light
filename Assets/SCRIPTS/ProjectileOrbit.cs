using UnityEngine;

public class ProjectileOrbit : MonoBehaviour
{
    [Header("Settings")]
    public Transform pivotObject; // The object to circle around (e.g., Player or Enemy)
    public float rotationSpeed = 100f; // How fast it circles
    public float radius = 2f; // Distance from the center
    public Vector3 axis = Vector3.up; // Axis to rotate around (Y is usually best)

    private Vector3 offset;

    void Start()
    {
        // If no pivot is set, create a temporary invisible one at the current position
        if (pivotObject == null)
        {
            GameObject tempPivot = new GameObject("TempPivot");
            tempPivot.transform.position = transform.position;
            pivotObject = tempPivot.transform;
        }

        // Set initial distance
        offset = (transform.position - pivotObject.position).normalized * radius;
        transform.position = pivotObject.position + offset;
    }

    void Update()
    {
        if (pivotObject != null)
        {
            // Rotate the projectile around the pivot point
            transform.RotateAround(pivotObject.position, axis, rotationSpeed * Time.deltaTime);
            
            // Optional: Keep the projectile facing forward along the path
            // transform.LookAt(transform.position + transform.forward); 
        }
    }
}