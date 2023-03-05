using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularUI : MonoBehaviour
{
    Transform leftHalf;
    Transform rightHalf;

    float percentageValue;

    [SerializeField]
    Color32 gatheringColor;
    [SerializeField]
    Color32 energyColor;
    [SerializeField]
    Color32 nullColor;

    public enum CircularUIType {Gathering, Energy, Null}
    [SerializeField] CircularUIType circularUIType = CircularUIType.Null;

    void Start()
    {
        leftHalf = transform.Find("Left Half").transform.Find("Sprite Mask");

        rightHalf = transform.Find("Right Half").transform.Find("Sprite Mask");

        if (circularUIType == CircularUIType.Gathering)
        {
            SetCircularUIColor(gatheringColor);
        }
        else if (circularUIType == CircularUIType.Energy)
        {
            SetCircularUIColor(energyColor);
        }
        else if (circularUIType == CircularUIType.Null)
        {
            SetCircularUIColor(nullColor);
        }
    }

    void CircularUIUpdate()
    {
        if (percentageValue <= 50)
        {
            leftHalf.localEulerAngles = new Vector3(0, 0, 180 * (1 - percentageValue / 50));
            rightHalf.localEulerAngles = new Vector3(0, 0, 180);
        }
        else if (percentageValue > 50)
        {
            leftHalf.localEulerAngles = new Vector3(0, 0, 0);
            rightHalf.localEulerAngles = new Vector3(0, 0, 180 * (2 - percentageValue / 50));
        }
    }

    public void SetCircularUIPercentage(float percentage)
    {
        percentage = Mathf.Clamp(percentage, 0, 100);

        percentageValue = percentage;

        CircularUIUpdate();
    }

    public void SetCircularUIColor(Color32 Color)
    {
        transform.Find("Left Half").transform.Find("Ring").GetComponent<SpriteRenderer>().color = Color;
        transform.Find("Right Half").transform.Find("Ring").GetComponent<SpriteRenderer>().color = Color;
    }
}
