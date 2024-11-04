using UnityEngine;
using System.Collections;

public class TractorController : MonoBehaviour
{
    // Grid parameters
    [Header("Grid Settings")]
    public int rows = 5;               // Number of rows in the grid
    public int columns = 5;            // Number of columns in the grid
    public float cellSize = 2f;        // Size of each grid cell

    // Movement parameters
    [Header("Movement Settings")]
    public float moveSpeed = 2f;       // Forward movement speed (units per second)
    public float rotationSpeed = 90f;  // Rotation speed (degrees per second)

    // Internal state
    private int currentRow = 0;
    private int currentColumn = 0;
    private bool movingRight = true;    // Direction flag

    private Vector3 basePosition;       // Starting position of the tractor

    void Start()
    {
        // Capture the initial position as the base position
        basePosition = transform.position;

        // Position the tractor at the starting cell relative to basePosition
        Vector3 startPos = GetWorldPosition(currentRow, currentColumn);
        transform.position = startPos;

        // Set initial rotation to face right (positive X)
        transform.rotation = Quaternion.Euler(0, 90, 0);

        // Start the movement coroutine
        StartCoroutine(TractorMovement());
    }

    IEnumerator TractorMovement()
    {
        while (currentRow < rows)
        {
            // Determine target column based on direction
            int targetColumn = movingRight ? columns - 1 : 0;

            // Move horizontally across the row
            while (currentColumn != targetColumn)
            {
                // Rotate to face horizontal direction (right or left)
                Quaternion targetRotation = Quaternion.Euler(0, movingRight ? 90 : -90, 0);
                yield return StartCoroutine(RotateTowards(targetRotation));

                // Move forward to next cell
                yield return StartCoroutine(MoveForward());

                // Update current column
                currentColumn += movingRight ? 1 : -1;

                Debug.Log($"Moved to Column: {currentColumn}, Row: {currentRow}");
            }

            // Reached end of the row, check if there is a next row
            if (currentRow < rows - 1)
            {
                // Rotate to face forward (positive Z)
                Quaternion rotateForward = Quaternion.Euler(0, 0, 0);
                yield return StartCoroutine(RotateTowards(rotateForward));

                // Move forward to next row
                yield return StartCoroutine(MoveForward());

                // Update current row
                currentRow += 1;

                Debug.Log($"Moved to Row: {currentRow}");

                // Rotate to face opposite horizontal direction
                Quaternion rotateOpposite = Quaternion.Euler(0, movingRight ? -90 : 90, 0);
                yield return StartCoroutine(RotateTowards(rotateOpposite));

                // Reverse direction
                movingRight = !movingRight;

                Debug.Log($"Direction Reversed. Now moving {(movingRight ? "Right" : "Left")}");
            }
            else
            {
                // Last row completed
                Debug.Log("Tractor has completed traversing the grid.");
                yield break;
            }
        }
    }

    IEnumerator MoveForward()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + transform.forward * cellSize;
        float distance = Vector3.Distance(startPos, endPos);
        float duration = distance / moveSpeed;
        float elapsed = 0f;

        Debug.Log($"Moving from {startPos} to {endPos}");

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
    }

    IEnumerator RotateTowards(Quaternion targetRotation)
    {
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    // Helper method to convert grid coordinates to world position relative to basePosition
    Vector3 GetWorldPosition(int row, int column)
    {
        float x = column * cellSize;
        float z = row * cellSize;
        return basePosition + new Vector3(x, 0, z);
    }

    // Optional: Visualize the grid in the editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // Calculate basePosition if the game is running
        Vector3 origin = Application.isPlaying ? basePosition : transform.position;

        // Draw rows
        for (int i = 0; i <= rows; i++)
        {
            Vector3 start = origin + new Vector3(0, 0, i * cellSize);
            Vector3 end = start + new Vector3((columns - 1) * cellSize, 0, 0);
            Gizmos.DrawLine(start, end);
        }

        // Draw columns
        for (int j = 0; j <= columns; j++)
        {
            Vector3 start = origin + new Vector3(j * cellSize, 0, 0);
            Vector3 end = start + new Vector3(0, 0, (rows - 1) * cellSize);
            Gizmos.DrawLine(start, end);
        }
    }
}
