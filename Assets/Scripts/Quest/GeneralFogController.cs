using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[Serializable]public class SDFVariables
{
    public float sizeX;
    public float sizeZ;
    public float centerX;
    public float centerZ;
}

public struct SDFNaming
{
    public string sizeX;
    public string sizeZ;
    public string centerX;
    public string centerZ;

    public SDFNaming(string sx, string sz, string cx, string cz)
    { 
         sizeX = sx;
         sizeZ = sz;
         centerX = cx;
         centerZ = cz;
    }
}

public class GeneralFogController : MonoBehaviour, IUniActionInteraction
{
    private int index;
    VisualEffect fogVFX;

    [SerializeField] private SDFVariables SDF1_AfterRevealScroll;
    [SerializeField] private SDFVariables SDF1_AfterRevealBuildingWorld;

    private SDFNaming SDF1_Naming;
    private SDFNaming SDF2_Naming;

    void Start()
    {
        fogVFX = GetComponent<VisualEffect>();
        SDF1_Naming = new SDFNaming("SDFSizeX_01", "SDFSizeZ_01", "SDFCenterX_01", "SDFCenterZ_01");
        SDF2_Naming = new SDFNaming("SDFSizeX_02", "SDFSizeZ_02", "SDFCenterX_02", "SDFCenterZ_02");
    }

    void AdjustSDF(SDFNaming naming, SDFVariables variables)
    {
        /*
        fogVFX.SetFloat(naming.sizeX, variables.sizeX);
        fogVFX.SetFloat(naming.sizeZ, variables.sizeZ);
        fogVFX.SetFloat(naming.centerX, variables.centerX);
        fogVFX.SetFloat(naming.centerZ, variables.centerZ);*/
        StartCoroutine(AdjustSDFCor(naming, variables, 1f));
    }

    IEnumerator AdjustSDFCor(SDFNaming naming, SDFVariables variables, float duration)
    {
        float orgSizeX = fogVFX.GetFloat(naming.sizeX);
        float orgSizeZ = fogVFX.GetFloat(naming.sizeZ);
        float orgCenterX = fogVFX.GetFloat(naming.centerX);
        float orgCenterZ = fogVFX.GetFloat(naming.centerZ);
        
        float finalSizeX = variables.sizeX;
        float finalSizeZ = variables.sizeZ;
        float finalCenterX = variables.centerX;
        float finalCenterZ = variables.centerZ;

        duration = (finalSizeX - orgSizeX) / 25;

        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            
            fogVFX.SetFloat(naming.sizeX, orgSizeX + (finalSizeX - orgSizeX) * (time/duration));
            fogVFX.SetFloat(naming.sizeZ, orgSizeZ + (finalSizeZ - orgSizeZ) * (time/duration));
            fogVFX.SetFloat(naming.centerX, orgCenterX + (finalCenterX - orgCenterX) * (time/duration));
            fogVFX.SetFloat(naming.centerZ, orgCenterZ + (finalCenterZ - orgCenterZ) * (time/duration));
            
            yield return new WaitForSeconds(0);
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
            //DeleteClouds();
            return;
        }
        float currentX = fogVFX.GetFloat("SDFSizeX");
        float currentZ = fogVFX.GetFloat("SDFSizeZ");
        fogVFX.SetFloat("SDFSizeX",currentX * 1.1f);
        fogVFX.SetFloat("SDFSizeZ",currentZ * 1.1f);
    }

    private void Effect_1_RevealScroll()
    {
        AdjustSDF(SDF1_Naming, SDF1_AfterRevealScroll);
    }

    private void Effect_2_RevealBuildingArea()
    {
        AdjustSDF(SDF1_Naming, SDF1_AfterRevealBuildingWorld);

    }

    /// <summary>
    /// To be used by IUniActionInteraction with according index
    /// </summary>
    /// <param name="index">1. Reveal scroll. 2. Reveal building area</param>
    public void TriggerInteractionByUniAction(int index)
    {
        if (index == 1) Effect_1_RevealScroll();
        else if(index == 2) Effect_2_RevealBuildingArea();
        else Debug.LogError("An index with no corresponding method is input: GameObject " + gameObject.name);
    }
}