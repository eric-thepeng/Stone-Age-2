using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularUI : MonoBehaviour
{
    Transform leftHalf;
    Transform rightHalf;

    [SerializeField]
    [Range(0, 100)]
    float percentageValue;

    void Start()
    {
        leftHalf = transform.Find("Left Half").transform.Find("Sprite Mask");

        rightHalf = transform.Find("Right Half").transform.Find("Sprite Mask");
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

    public void SetCircularUI(float percentage)
    {
        percentage = Mathf.Clamp(percentage, 0, 100);

        percentageValue = percentage;

        CircularUIUpdate();
    }
}
