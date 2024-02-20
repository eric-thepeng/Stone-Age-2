using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ROSpawn : MonoBehaviour
{
    public List<GameObject> objectsToSpawn; // List of objects to spawn

    public float spawnScale = 1; 

    void Start()
    {
        SpawnObject();
    }

    void SpawnObject()
    {
        // Choose a random object from the list
        GameObject objectToSpawn = objectsToSpawn[Random.Range(0, objectsToSpawn.Count)];

        // Spawn the object at the position of this GameObject
        GameObject spawnedObject = Instantiate(objectToSpawn, transform.position, objectToSpawn.transform.rotation, transform);

        Quaternion randomRotation = Quaternion.Euler(-90, Random.Range(0, 360), 0);

        spawnedObject.transform.rotation = randomRotation;

        spawnedObject.transform.localScale = Vector3.Scale(spawnedObject.transform.localScale, new Vector3(spawnScale, spawnScale, spawnScale));
    }
}
