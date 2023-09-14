using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FogController : MonoBehaviour
{
    [SerializeField]
    GameObject fogVFX;

    [SerializeField]
    int areaWidth;

    [SerializeField]
    int areaHeight;

    [SerializeField]
    int spacing;

    void Start()
    {
        GenerateFog();
    }

    void GenerateFog()
    {
        float x = transform.position.x - areaWidth / 2;
        float y = transform.position.y;
        float z = transform.position.z - areaHeight / 2;

        for (int i1 = 0; i1 < areaHeight; i1 += spacing)
        {
            for (int i2 = 0; i2 < areaWidth; i2 += spacing)
            {
                Instantiate(fogVFX, new Vector3(x + i2, y, z + i1), Quaternion.identity, gameObject.transform);
                //VisualEffect thisFogVFX = Instantiate(fogVFX, new Vector3(x + i2, y, z + i1), Quaternion.identity, gameObject.transform).GetComponent<VisualEffect>();
            }
        }
    }
}
