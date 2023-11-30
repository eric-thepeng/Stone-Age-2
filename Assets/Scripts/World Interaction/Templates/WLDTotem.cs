using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WLDTotem : WorldInteractable
{
    [SerializeField]
    private int PointToAdd = 1;
    private ParticleSystem bubble;
    public void AddPoint()
    {
        SpiritPoint.i.Add(PointToAdd);
        bubble.Play();
        StartCoroutine(waitForDuration(bubble.main.duration));
    }

    private IEnumerator waitForDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        SetCurrentInteraction(new InteractionType(InteractionType.TypeName.Click, AddPoint));
    }

    void Start()
    {
        SetCurrentInteraction(new InteractionType(InteractionType.TypeName.Click, AddPoint));
        bubble = gameObject.GetComponentInChildren<ParticleSystem>();
    }

}
