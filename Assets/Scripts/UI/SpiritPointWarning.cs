using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpiritPointWarning : MonoBehaviour
{
    [Header("Vibration controls")]
    [SerializeField]
    private float vibrationSpeed = 2f;
    [SerializeField]
    private float vibrationAmount = 0.5f;
    [SerializeField]
    private float enlargeAmount = 1.5f;

    private GameObject textObject;
    private Vector3 originalScale;


    void Awake()
    {
        textObject = GameObject.Find("Spirit Point Amount");
        originalScale = textObject.transform.localScale;
    }


    public void TextHighlight()
    {
        StartCoroutine(Highlights());
    }

    IEnumerator Highlights()
    {
        float elapsedTime = 0f;
        float duration = 1f;

        while (elapsedTime < duration)
        {
            if(elapsedTime < duration *2 / 3)
            {
                textObject.transform.localScale = originalScale * (Mathf.Lerp(1f, enlargeAmount, elapsedTime / duration));
            }
            else
            {
                float vibrationFactor = (Mathf.PingPong(elapsedTime * vibrationSpeed, vibrationAmount) * 0.5f);
                textObject.transform.localScale = originalScale * (1f + vibrationFactor);             
            }
            elapsedTime += Time.fixedDeltaTime;
            yield return null;
        }
  
        textObject.transform.localScale = originalScale;
    }
}
