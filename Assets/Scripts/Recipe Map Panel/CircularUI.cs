using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularUI : MonoBehaviour
{
    Transform leftHalf;
    Transform rightHalf;

    [SerializeField]
    [Range(0f, 100f)]
    float percentageValue;

    Color32 gatheringColor = new Color32(88, 151, 255, 255);
    Color32 energyColor = new Color32(88, 255, 96, 255);
    Color32 nullColor = new Color32(255, 255, 255, 0);

    public enum CircularUIType {Gathering, Energy, Null}
    [SerializeField]
    CircularUIType circularUIType = CircularUIType.Null;

    void Start()
    {
        leftHalf = transform.Find("Left Half").transform.Find("Sprite Mask");

        rightHalf = transform.Find("Right Half").transform.Find("Sprite Mask");

        CircularUIColorUpdate();
    }
    private void Update()
    {
        CircularUIUpdate();
    }

    public void SetCircularUIType(CircularUIType circularUIType)
    {
        this.circularUIType = circularUIType;

        CircularUIColorUpdate();
    }

    public void SetCircularUIPercentage(float percentage)
    {
        percentage = Mathf.Clamp(percentage, 0, 100);

        percentageValue = percentage;

        CircularUIUpdate();
    }

    void CircularUIColorUpdate()
    {
        if (circularUIType == CircularUIType.Gathering)
        {
            transform.Find("Left Half").transform.Find("Ring").GetComponent<SpriteRenderer>().color = gatheringColor;
            transform.Find("Right Half").transform.Find("Ring").GetComponent<SpriteRenderer>().color = gatheringColor;
        }
        else if (circularUIType == CircularUIType.Energy)
        {
            transform.Find("Left Half").transform.Find("Ring").GetComponent<SpriteRenderer>().color = energyColor;
            transform.Find("Right Half").transform.Find("Ring").GetComponent<SpriteRenderer>().color = energyColor;
        }
        else if (circularUIType == CircularUIType.Null)
        {
            transform.Find("Left Half").transform.Find("Ring").GetComponent<SpriteRenderer>().color = nullColor;
            transform.Find("Right Half").transform.Find("Ring").GetComponent<SpriteRenderer>().color = nullColor;
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
}
