using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ROSpawn : MonoBehaviour
{
    public List<GameObject> objectsToSpawn; // List of objects to spawn

    void Start()
    {
        SpawnObject();
    }

    void SpawnObject()
    {
        // Choose a random object from the list
        GameObject objectToSpawn = objectsToSpawn[Random.Range(0, objectsToSpawn.Count)];

        // Spawn the object at the position of this GameObject
        Instantiate(objectToSpawn, transform.position, objectToSpawn.transform.rotation);
    }
}
