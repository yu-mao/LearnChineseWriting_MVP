using UnityEngine;

public class TutorialEvaluation : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToSpawn; // List of objects to spawn
    [SerializeField] private Transform spawnPosition;    // Spawn position
    [SerializeField] private float destroyDelay = 5f;    // Time in seconds before the object is destroyed

    private int currentIndex = 0;                       // Current object index in the sequence
    private GameObject currentObject;                   // Reference to the currently instantiated object
    private bool isRunning = false;                     // Check if the loop is active

    public void StartSequence()
    {
        if (!isRunning)
        {
            isRunning = true;
            StartCoroutine(SpawnAndDestroySequence());
        }
    }

    private System.Collections.IEnumerator SpawnAndDestroySequence()
    {
        while (isRunning)
        {
            // Spawn the current object
            if (currentObject != null)
            {
                Destroy(currentObject); // Ensure the previous object is destroyed
            }

            currentObject = Instantiate(objectsToSpawn[currentIndex], spawnPosition.position, spawnPosition.rotation);

            // Wait for the destroy delay
            yield return new WaitForSeconds(destroyDelay);

            // Move to the next object in the list
            currentIndex = (currentIndex + 1) % objectsToSpawn.Length;
        }
    }

    public void StopSequence()
    {
        isRunning = false;
        if (currentObject != null)
        {
            Destroy(currentObject); // Cleanup the last object
        }
    }
}