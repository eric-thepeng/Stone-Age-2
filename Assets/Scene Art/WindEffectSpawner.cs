using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEffectSpawner : MonoBehaviour
{
    [SerializeField]
    Camera camera;

    Vector3 hitPosition;

    [SerializeField]
    GameObject[] WindEffects;

    float SpawnTimer = 0;
    [SerializeField]
    float SpawnTimerMax = 5.0f;

    [SerializeField]
    float OffsetX;

    [SerializeField]
    float OffsetY;

    void Start()
    {
        SpawnWind();
    }

    void Update()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(camera.transform.position, camera.transform.forward, 200.0f);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.tag.Equals("Terrain"))
            {
                hitPosition = hit.point;
            }
        }

        if (SpawnTimer < SpawnTimerMax)
        {
            SpawnTimer += Time.deltaTime;
        }
        else
        {
            SpawnTimer = 0;

            SpawnWind();
        }
    }

    void SpawnWind() {
        Vector3 spawnPosition = new Vector3(hitPosition.x - 5 + Random.Range(-OffsetX, OffsetX), 20 + Random.Range(-OffsetY, OffsetY), hitPosition.z + 5 + Random.Range(-OffsetX, OffsetX));

        GameObject wind = Instantiate(WindEffects[Random.Range(0, WindEffects.Length)], spawnPosition, Quaternion.Euler(new Vector3(-180, -150, 180)), gameObject.transform);
        wind.GetComponent<ParticleSystem>().Emit(1);
    }
}
