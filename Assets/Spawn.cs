using System.Collections;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] ObjectList; // List of objects to spawn
    public Transform spawnCenter;    // The transform around which to spawn objects
    public float spawnRange = 5f;    // Range around the spawn center
    public float spawnInterval = 2f;  // Time interval between spawns
    public Vector3 spawnOffset;        // Offset to be added to the spawn position

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnObjects());
    }

    // Coroutine to spawn objects at intervals
    private IEnumerator SpawnObjects()
    {
        while (true) // Continuously spawn objects
        {
            SpawnObject();
            yield return new WaitForSeconds(spawnInterval); // Wait for the specified interval
        }
    }

    // Method to spawn a random object
    private void SpawnObject()
    {
        if (ObjectList.Length == 0) return; // Ensure there are objects to spawn

        // Select a random object from the list
        GameObject randomObject = ObjectList[Random.Range(0, ObjectList.Length)];

        // Generate a random position around the spawn center
        Vector3 randomPosition = spawnCenter.position + new Vector3(
            Random.Range(-spawnRange, spawnRange), // X position
            0, // Y position (use 0 or modify for height)
            Random.Range(-spawnRange, spawnRange)  // Z position
        );

        // Add the spawn offset to the random position
        randomPosition += spawnOffset;

        // Instantiate the object at the random position
        Instantiate(randomObject, randomPosition, Quaternion.identity);
    }
}

