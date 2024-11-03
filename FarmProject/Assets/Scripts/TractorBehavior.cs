using UnityEngine;

public class TractorController : MonoBehaviour
{
    [Header("Area Settings")]
    public float areaLength = 20f;   // Z-axis (length of the field)
    public float areaWidth = 10f;    // X-axis (width of the field)

    [Header("Tractor Settings")]
    public float velocity = 2f;      // Movement speed (units per second)
    public float rowSpacing = 1f;    // Distance between each row (along X-axis)

    [Header("Seed Settings")]
    public GameObject seedPrefab;
    public float seedDropInterval = 1f; // Distance between seed drops (z cm)

    private Vector3 startPosition;
    private bool movingForward = true;
    private float totalDistanceTraveled = 0f;
    private float distanceSinceLastSeed = 0f;
    private int currentRow = 0;
    private int totalRows;

    void Start()
    {
        // Set the starting position
        startPosition = transform.position;

        // Calculate the total number of rows the tractor needs to cover
        totalRows = Mathf.CeilToInt(areaWidth / rowSpacing);

        // Position the tractor at the starting edge
        transform.position = new Vector3(
            startPosition.x - areaWidth / 2 + currentRow * rowSpacing,
            startPosition.y,
            startPosition.z - areaLength / 2
        );
    }

    void Update()
    {
        MoveTractor();
        CheckRowCompletion();
        DropSeeds();
    }

    void MoveTractor()
    {
        // Move the tractor along the Z-axis
        float step = velocity * Time.deltaTime;
        float direction = movingForward ? 1 : -1;
        transform.Translate(0, 0, step * direction);

        // Update the total distance traveled and distance since last seed drop
        totalDistanceTraveled += step;
        distanceSinceLastSeed += step;
    }

    void CheckRowCompletion()
    {
        // Check if the tractor has reached the end of the current row
        if (movingForward && transform.position.z >= startPosition.z + areaLength / 2)
        {
            // Move to the next row
            ShiftToNextRow();
        }
        else if (!movingForward && transform.position.z <= startPosition.z - areaLength / 2)
        {
            // Move to the next row
            ShiftToNextRow();
        }
    }

    void ShiftToNextRow()
    {
        currentRow++;

        if (currentRow >= totalRows)
        {
            // Finished covering the area
            Debug.Log("Tractor has covered the entire area.");
            enabled = false; // Stop the tractor
            return;
        }

        // Move the tractor to the next row
        movingForward = !movingForward; // Change direction

        transform.position = new Vector3(
            startPosition.x - areaWidth / 2 + currentRow * rowSpacing,
            transform.position.y,
            movingForward ? startPosition.z - areaLength / 2 : startPosition.z + areaLength / 2
        );
    }

    void DropSeeds()
    {
        if (distanceSinceLastSeed >= seedDropInterval)
        {
            // Drop seeds behind the tractor
            Vector3 seedPosition = new Vector3(
                transform.position.x,
                transform.position.y,
                transform.position.z
            );

            Instantiate(seedPrefab, seedPosition, Quaternion.identity);

            // Reset the distance since last seed drop
            distanceSinceLastSeed = 0f;
        }
    }
}
