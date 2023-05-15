using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularUI : MonoBehaviour
{
    Transform leftHalf;
    Transform rightHalf;

    [SerializeField]
    [Range(0f, 100f)]
    float currentPercentage;
    float targetPercentage;

    [SerializeField]
    Color32 displayColor;
    Color32 nonDisplayColor = new Color32(255, 255, 255, 0);

    [SerializeField]
    int sortOrder = 0;

    public enum CircularUIState {Display, NonDisplay}
    [SerializeField]
    CircularUIState circularUIState = CircularUIState.NonDisplay;

    void Start()
    {
        leftHalf = transform.Find("Left Half").transform.Find("Sprite Mask");
        rightHalf = transform.Find("Right Half").transform.Find("Sprite Mask");

        SetSortingOrder(sortOrder);

        CircularUIColorUpdate();
    }

    public void SetSortingOrder(int sortOrder)
    {
        transform.Find("Left Half").transform.Find("Sprite Mask").GetComponent<SpriteMask>().backSortingOrder = sortOrder + 0;
        transform.Find("Left Half").transform.Find("Ring").GetComponent<SpriteRenderer>().sortingOrder = sortOrder + 1;
        transform.Find("Left Half").transform.Find("Sprite Mask").GetComponent<SpriteMask>().frontSortingOrder = sortOrder + 2;

        transform.Find("Right Half").transform.Find("Sprite Mask").GetComponent<SpriteMask>().backSortingOrder = sortOrder + 3;
        transform.Find("Right Half").transform.Find("Ring").GetComponent<SpriteRenderer>().sortingOrder = sortOrder + 4;
        transform.Find("Right Half").transform.Find("Sprite Mask").GetComponent<SpriteMask>().frontSortingOrder = sortOrder + 5;
    }

    public void SetCircularUIState(CircularUIState circularUIState)
    {
        this.circularUIState = circularUIState;

        CircularUIColorUpdate();
    }

    public void SetCircularUIPercentage(float percentage, bool isLerp)
    {
        percentage = Mathf.Clamp(percentage, 0, 100);

        targetPercentage = percentage;

        if (isLerp)
        {
            currentPercentage = Mathf.Lerp(currentPercentage, targetPercentage, 0.15f);
        }
        else 
        {
            currentPercentage = percentage;
        }
       
        CircularUIUpdate();
    }

    void CircularUIColorUpdate()
    {
        if (circularUIState == CircularUIState.Display)
        {
            transform.Find("Left Half").transform.Find("Ring").GetComponent<SpriteRenderer>().color = displayColor;
            transform.Find("Right Half").transform.Find("Ring").GetComponent<SpriteRenderer>().color = displayColor;
        }
        else if (circularUIState == CircularUIState.NonDisplay)
        {
            transform.Find("Left Half").transform.Find("Ring").GetComponent<SpriteRenderer>().color = nonDisplayColor;
            transform.Find("Right Half").transform.Find("Ring").GetComponent<SpriteRenderer>().color = nonDisplayColor;
        }

    }

    void CircularUIUpdate()
    {
        if (currentPercentage <= 50)
        {
            leftHalf.localEulerAngles = new Vector3(0, 0, 180 * (1 - currentPercentage / 50));
            rightHalf.localEulerAngles = new Vector3(0, 0, 180);
        }
        else if (currentPercentage > 50)
        {
            leftHalf.localEulerAngles = new Vector3(0, 0, 0);
            rightHalf.localEulerAngles = new Vector3(0, 0, 180 * (2 - currentPercentage / 50));
        }
    }
}
