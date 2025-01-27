using UnityEngine;
using System;
using System.Collections;

public class DestroyAnimationUI : MonoBehaviour
{

    [SerializeField] private GameObject ObjectDestroy; // The prefab to instantiate
    [SerializeField] private Transform spawnPosition; // Default spawn position
    [SerializeField] private float destroyDelay = 5f; // Time in seconds before the object is destroyed

    private GameObject spawnedObject; // Store reference to the spawned object

    public void SpawnObject()
    {
        if (ObjectDestroy != null && spawnPosition != null)
        {
            // Check if an object is already spawned
            if (spawnedObject != null)
            {
                Debug.LogWarning("An object is already spawned. Destroy it before spawning another.");
                return;
            }

            // Instantiate the prefab at the specified position and rotation
            spawnedObject = Instantiate(ObjectDestroy, spawnPosition.position, spawnPosition.rotation);

            // Destroy the spawned object after the delay
            Destroy(spawnedObject, destroyDelay);
        }
        else
        {
            Debug.LogWarning("Prefab or spawn position is not assigned!");
        }
    }

    public void StopSpawn()
    {
        if (spawnedObject != null)
        {
            // Destroy the currently spawned object immediately
            Destroy(spawnedObject);
            spawnedObject = null;
            Debug.Log("Spawned object destroyed.");
        }
        else
        {
            Debug.LogWarning("No object is currently spawned to destroy.");
        }
    }
}