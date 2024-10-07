using System;
using UnityEngine;
using UnityEngine.Events;

public class ARSpawner : MonoBehaviour
{
    public float scale = 1f;
    public UnityEvent onSpawn;
    private ARPlayer player;
    [NonSerialized]
    public GameObject spawnedObject;
    private void Awake()
    {
        player = GetComponentInParent<ARPlayer>();
    }
    public void SpawnPrefab(GameObject prefab)
    {
        RaycastHit hit;
        if (Physics.Raycast(player.camera.transform.position, player.camera.transform.forward, out hit))
        {
            spawnedObject = Instantiate(prefab, hit.point, new Quaternion());
            spawnedObject.transform.localScale = new Vector3(scale, scale, scale);
            onSpawn.Invoke();
        }
    }
}
