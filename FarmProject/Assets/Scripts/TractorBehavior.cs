using UnityEngine;
using System.Collections;

public class TractorController : MonoBehaviour
{
 
    [Header("Grid Settings")]
    public int rows = 5;              
    public int columns = 5;            
    public float cellSize = 2f;        

    public float moveSpeed = 2f;    
    public float rotationSpeed = 90f; 

    private int currentRow = 0;
    private int currentColumn = 0;
    private bool movingRight = true;    

    private Vector3 basePosition;       

    void Start()
    {
       
        basePosition = transform.position;

      
        Vector3 startPos = GetWorldPosition(currentRow, currentColumn);
        transform.position = startPos;
        transform.rotation = Quaternion.Euler(0, 90, 0);

        StartCoroutine(TractorMovement());
    }

    IEnumerator TractorMovement()
    {
        while (currentRow < rows)
        {
        
            int targetColumn = movingRight ? columns - 1 : 0;

            
            while (currentColumn != targetColumn)
            {
               
                Quaternion targetRotation = Quaternion.Euler(0, movingRight ? 90 : -90, 0);
                yield return StartCoroutine(RotateTowards(targetRotation));

               
                yield return StartCoroutine(MoveForward());

                
                currentColumn += movingRight ? 1 : -1;

   
            }

            
            if (currentRow < rows - 1)
            {
                
                Quaternion rotateForward = Quaternion.Euler(0, 0, 0);
                yield return StartCoroutine(RotateTowards(rotateForward));
                yield return StartCoroutine(MoveForward());
                currentRow += 1;
                Quaternion rotateOpposite = Quaternion.Euler(0, movingRight ? -90 : 90, 0);
                yield return StartCoroutine(RotateTowards(rotateOpposite));
                movingRight = !movingRight;
            }  
            else
            {

                Debug.Log("Tractor has completed.");
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

    
    Vector3 GetWorldPosition(int row, int column)
    {
        float x = column * cellSize;
        float z = row * cellSize;
        return basePosition + new Vector3(x, 0, z);
    }

    
    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;

    //    // Calculate basePosition if the game is running
    //    Vector3 origin = Application.isPlaying ? basePosition : transform.position;

    //    // Draw rows
    //    for (int i = 0; i <= rows; i++)
    //    {
    //        Vector3 start = origin + new Vector3(0, 0, i * cellSize);
    //        Vector3 end = start + new Vector3((columns - 1) * cellSize, 0, 0);
    //        Gizmos.DrawLine(start, end);
    //    }

    //    // Draw columns
    //    for (int j = 0; j <= columns; j++)
    //    {
    //        Vector3 start = origin + new Vector3(j * cellSize, 0, 0);
    //        Vector3 end = start + new Vector3(0, 0, (rows - 1) * cellSize);
    //        Gizmos.DrawLine(start, end);
    //    }
    
}
