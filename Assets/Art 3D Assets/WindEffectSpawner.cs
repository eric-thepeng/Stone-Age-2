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

    ParticleSystemRenderer[] WindList;

    bool isWindOn = true;

    static WindEffectSpawner instance;
    public static WindEffectSpawner i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<WindEffectSpawner>();
            }
            return instance;
        }
    }

    void Start()
    {
        WindList = new ParticleSystemRenderer[10];

        for (int count = 0; count < WindList.Length; count++)
        {
            WindList[count] = null;
        }

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

        if ((PlayerState.state == PlayerState.State.BlueprintAndResearch) && isWindOn)
        {
            HideWind();
        }
        else if ((PlayerState.state == PlayerState.State.Browsing || PlayerState.state == PlayerState.State.Building) && !isWindOn)
        {
            ShowWind();
        }
    }

    void SpawnWind() {
        Vector3 spawnPosition = new Vector3(hitPosition.x - 5 + Random.Range(-OffsetX, OffsetX), 20 + Random.Range(-OffsetY, OffsetY), hitPosition.z + 5 + Random.Range(-OffsetX, OffsetX));

        GameObject wind = Instantiate(
            WindEffects[Random.Range(0, WindEffects.Length)], 
            spawnPosition, 
            Quaternion.Euler(new Vector3(-180, -150, 180)), 
            gameObject.transform);
        wind.GetComponent<ParticleSystem>().Emit(1);

        for (int count = 0; count < WindList.Length; count++)
        {
            if (WindList[count] == null)
            {
                WindList[count] = wind.GetComponent<ParticleSystemRenderer>();

                break;
            }
        }

        if (!isWindOn)
        {
            wind.SetActive(false);
        }
    }

    public void HideWind()
    {
        for (int count = 0; count < WindList.Length; count++)
        {
            if (WindList[count] != null)
            {
                WindList[count].enabled = false;
            }
        }

        isWindOn = false;
    }

    public void ShowWind()
    {
        for (int count = 0; count < WindList.Length; count++)
        {
            if (WindList[count] != null)
            {
                WindList[count].enabled = true;
            }
        }

        isWindOn = true;
    }
}
