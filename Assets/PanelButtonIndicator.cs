using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelButtonIndicator : MonoBehaviour
{
    static PanelButtonIndicator instance = null;
    public static PanelButtonIndicator i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PanelButtonIndicator>();
            }
            return instance;
        }
    }

    [SerializeField] Transform buildingButtonTransform;
    [SerializeField] Transform craftingButtonTransform;
    [SerializeField] Transform blueprintButtonTransform;
    [SerializeField] Transform homeButtonTransform;

    Vector3 homeButtonScale = new Vector3(1.15f, 1.15f, 1f);
    Vector3 otherButtonScale = new Vector3(0.8f, 0.8f, 1f);

    float transitionTime = 0.3f;

    bool moving = false;
    Vector3 targetPos = new Vector3(0,0,0);
    Vector3 targetScale = new Vector3(0, 0, 0);

    float movingSpeed = 8f;
    float scalingSpeed = 5f;

    // Update is called once per frame
    void Update()
    {
        /*

        if (!moving) return;

        bool stopMove = false;
        bool stopScale = false;

        if ((transform.localPosition - targetPos).magnitude >= 0.2f)
        {
            transform.localPosition += (targetPos - transform.localPosition).normalized * movingSpeed * Time.deltaTime;
        }
        else
        {
            transform.localPosition = targetPos;
            stopMove = true;
        }

        if ((transform.localScale - targetScale).magnitude >= 0.2f)
        {
            transform.localScale += (targetScale - transform.localScale).normalized * scalingSpeed * Time.deltaTime;
        }
        else
        {
            transform.localScale = targetScale;
            stopScale = true;
        }

        moving = !(stopMove && stopScale);

        */
    }

    IEnumerator MoveCor()
    {
        float timeCount = 0;
        while(timeCount < transitionTime)
        {
            transform.localPosition += (targetPos - transform.localPosition).normalized * movingSpeed * Time.deltaTime;
            transform.localScale += (targetScale - transform.localScale).normalized * scalingSpeed * Time.deltaTime;
            timeCount += Time.deltaTime;
            yield return new WaitForSeconds(0);
        }
        transform.localPosition = targetPos;
        transform.localScale = targetScale;
    }

    public void MoveToHomeBuilding()
    {
        moving = true;
        targetPos = buildingButtonTransform.localPosition;
        targetScale = otherButtonScale;
        calculateSpeed();
    }

    public void MoveToCrafting()
    {
        moving = true;
        targetPos = craftingButtonTransform.localPosition;
        targetScale = otherButtonScale;
        calculateSpeed();
    }

    public void MoveToBlueprintMap()
    {
        moving = true;
        targetPos = blueprintButtonTransform.localPosition;
        targetScale = otherButtonScale;
        calculateSpeed();
    }

    public void MoveToHome()
    {
        moving = true;
        targetPos = homeButtonTransform.localPosition;
        targetScale = homeButtonScale;
        calculateSpeed();
    }

    private void calculateSpeed()
    {
        StopAllCoroutines();
        movingSpeed = (targetPos - transform.localPosition).magnitude / transitionTime;
        scalingSpeed = (targetScale - transform.localScale).magnitude / transitionTime;
        StartCoroutine(MoveCor());
    }

}
