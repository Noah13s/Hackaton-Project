using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] objectsToSpawn; // Array of GameObjects (26 objects)
    public Transform[] spawnPoints; // Array of possible spawn points (3 points)
    public float fallSpeed = 2f; // Speed at which objects fall down
    public float spawnInterval = 3f; // Time interval between spawns

    private void Start()
    {
        // Start the repeated spawning of objects
        StartCoroutine(SpawnObjects());
    }

    private IEnumerator SpawnObjects()
    {
        while (true)
        {
            // Randomly select an object from the array
            int randomObjectIndex = Random.Range(0, objectsToSpawn.Length);
            GameObject selectedObject = objectsToSpawn[randomObjectIndex];

            // Randomly select one of the spawn points
            int randomSpawnIndex = Random.Range(0, spawnPoints.Length);
            Transform selectedSpawnPoint = spawnPoints[randomSpawnIndex];

            // Spawn the selected object at the random spawn point
            GameObject spawnedObject = Instantiate(selectedObject, selectedSpawnPoint.position, Quaternion.identity);

            // Start the falling behavior for the object
            StartCoroutine(FallDown(spawnedObject));

            // Destroy the object after 10 seconds to save memory
            Destroy(spawnedObject, 10f);

            // Wait for the specified interval before spawning the next object
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private IEnumerator FallDown(GameObject fallingObject)
    {
        // As long as the object exists, make it fall down slowly
        while (fallingObject != null)
        {
            fallingObject.transform.position += Vector3.down * fallSpeed * Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }
    }
}
