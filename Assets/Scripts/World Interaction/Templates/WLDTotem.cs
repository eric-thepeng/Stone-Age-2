using System.Collections;
using System.Collections.Generic;
using UnityEngine.VFX;
using UnityEngine;

public class WLDTotem : WorldInteractable
{
    [SerializeField, Header("DO NOT EDIT CurrentInteraction ABOVE")]
    private int PointToAdd = 1;
    [SerializeField]
    private VisualEffect VFXSystem;
    [SerializeField]
    private ParticleSystem particleSystem;


    void Start()
    {
        SetCurrentInteraction(new InteractionType(InteractionType.TypeName.Click, AddPoint));
    }

    public void AddPoint()
    {
        SpiritPoint.i.Add(PointToAdd);

        if (particleSystem != null)
        {
            particleSystem.Play();
        }
        if (VFXSystem != null)
        {
            VFXSystem.Play();
        }

        
        StartCoroutine(waitForDuration(particleSystem.main.duration));
    }

    private IEnumerator waitForDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        SetCurrentInteraction(new InteractionType(InteractionType.TypeName.Click, AddPoint));
    }
}
