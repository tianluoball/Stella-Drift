using UnityEngine;
using System.Collections;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] prototypePrefabs; // Array of different prototype objects to spawn
    public float minSpawnTime = 0.5f;     // Minimum time between spawns
    public float maxSpawnTime = 2.0f;     // Maximum time between spawns
    
    [Header("Position Settings")]
    public float minX = -10f;             // Minimum X position (left)
    public float maxX = 10f;              // Maximum X position (right)
    public float minY = -10f;             // Minimum Y position (bottom)
    public float maxY = 10f;              // Maximum Y position (top)
    public float spawnZ = 20f;            // Z position where objects spawn
    public float destroyZ = -1f;          // Z position where objects are destroyed
    
    [Header("Object Settings")]
    public float minSpeed = 3.0f;         // Minimum movement speed
    public float maxSpeed = 7.0f;         // Maximum movement speed
    public float minScale = 0.5f;         // Minimum object scale
    public float maxScale = 2.0f;         // Maximum object scale
    
    void Start()
    {
        if (prototypePrefabs.Length == 0)
        {
            Debug.LogError("No prototype prefabs assigned! Add some prefabs to the array in the inspector.");
            return;
        }
        
        // Start spawning objects
        StartCoroutine(SpawnObjects());
    }
    
    IEnumerator SpawnObjects()
    {
        while (true)
        {
            // Wait for a random time before spawning the next object
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);
            
            SpawnObject();
        }
    }
    
    void SpawnObject()
    {
        // Generate random position within the defined rectangle
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(randomX, randomY, spawnZ);
        
        // Select a random prototype from the array
        GameObject selectedPrefab = prototypePrefabs[Random.Range(0, prototypePrefabs.Length)];
        
        // Instantiate the object
        GameObject obj = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        
        // Randomize the size
        float randomScale = Random.Range(minScale, maxScale);
        obj.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        
        // Add ObjectMovement component to make it move along Z-axis
        ObjectMovement movement = obj.AddComponent<ObjectMovement>();
        movement.speed = Random.Range(minSpeed, maxSpeed);
        movement.destroyZ = destroyZ;
    }
}

// Separate script for object movement
public class ObjectMovement : MonoBehaviour
{
    public float speed = 5f;
    public float destroyZ = -1f;
    
    void Update()
    {
        // Move along negative Z-axis
        transform.Translate(0, 0, -speed * Time.deltaTime);
        
        // Destroy the object when it reaches or passes the destroyZ position
        if (transform.position.z <= destroyZ)
        {
            Destroy(gameObject);
        }
    }
}