using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class OpeningFogController : MonoBehaviour, IUniActionInteraction
{
    [SerializeField]private int index;
    
    VisualEffect fogVFX;

    void Start()
    {
        fogVFX = GetComponent<VisualEffect>();
        if (index == 2)
        {
            SpiritPoint.i.GetPlayerStat().SubscribeStatChange(UpdateInnerCircle);
        }
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Alpha1) && index == 1) || (Input.GetKeyDown(KeyCode.Alpha2) && index == 2))
        {
            DeleteClouds();
        }
    }

    void DeleteClouds()
    {
        fogVFX.Stop();
        fogVFX.playRate = 2.0f;
        Destroy(gameObject, 4.0f);
    }

    void UpdateInnerCircle(int currentAmount)
    {
        if(index != 2) return;
        if(currentAmount > 8) return;
        if (currentAmount == 8)
        {
            DeleteClouds();
            return;
        }
        float currentX = fogVFX.GetFloat("SDFSizeX");
        float currentZ = fogVFX.GetFloat("SDFSizeZ");
        fogVFX.SetFloat("SDFSizeX",currentX * 1.1f);
        fogVFX.SetFloat("SDFSizeZ",currentZ * 1.1f);
    }

    public void TriggerInteractionByUniAction(int index)
    {
        if (index == this.index)
        {
            DeleteClouds();
        }
    }
}
