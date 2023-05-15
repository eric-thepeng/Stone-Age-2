using DG.Tweening;
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

    [SerializeField] Transform craftingButtonTransform;
    [SerializeField] Transform researchButtonTransform;
    [SerializeField] Transform homeAndBuildingButtonTransform;
    [SerializeField] Transform exitButtonTransform;
    [SerializeField] Transform indicatorTransform;

    const int NOT_DISPLAY = 0, RESEARCH = 1, CRAFTING = 2; 
    int displaying = NOT_DISPLAY;

    Vector3 topRightPosition_Left;
    Vector3 topRightPosition_Right;

    private void Start()
    {
        topRightPosition_Left = homeAndBuildingButtonTransform.localPosition;
        topRightPosition_Right = exitButtonTransform.localPosition;
    }

    /*
    float transitionTime = 0.3f;

    bool moving = false;
    Vector3 targetPos = new Vector3(0,0,0);
    Vector3 targetScale = new Vector3(0, 0, 0);

    float movingSpeed = 8f;
    float scalingSpeed = 5f;


    /*
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

    /*

    public void MoveToHomeBuilding()
    {
        moving = true;
        targetPos = buildingButtonTransform.localPosition;
        calculateSpeed();
    }

    public void MoveToCrafting()
    {
        moving = true;
        targetPos = craftingButtonTransform.localPosition;
        calculateSpeed();
    }

    public void MoveToBlueprintMap()
    {
        moving = true;
        targetPos = blueprintButtonTransform.localPosition;
        calculateSpeed();
    }

    public void MoveToHome()
    {
        moving = true;
        targetPos = homeButtonTransform.localPosition;
        calculateSpeed();
    }

    private void calculateSpeed()
    {
        StopAllCoroutines();
        movingSpeed = (targetPos - transform.localPosition).magnitude / transitionTime;
        scalingSpeed = (targetScale - transform.localScale).magnitude / transitionTime;
        StartCoroutine(MoveCor());
    }*/

    public void Exit()
    {
        if (displaying == NOT_DISPLAY) return;
        homeAndBuildingButtonTransform.DOLocalMove(topRightPosition_Left, 0.5f);
        exitButtonTransform.DOLocalMove(topRightPosition_Right, 0.5f);
        displaying = NOT_DISPLAY;
        indicatorTransform.DOLocalMove(indicatorTransform.localPosition + new Vector3(0, -2, 0),0.4f);
    }

    public void EnterResearch()
    {
        if(displaying == NOT_DISPLAY)
        {
            indicatorTransform.localPosition = researchButtonTransform.localPosition + new Vector3(0, -2, 0);
            homeAndBuildingButtonTransform.DOLocalMove(topRightPosition_Right, 0.5f);
            exitButtonTransform.DOLocalMove(topRightPosition_Left, 0.5f);
        }
        displaying = RESEARCH;
        indicatorTransform.DOMove(researchButtonTransform.position, 0.4f);

    }

    public void EnterCrafting()
    {
        if (displaying == NOT_DISPLAY)
        {
            indicatorTransform.localPosition = craftingButtonTransform.localPosition + new Vector3(0, -2, 0);
            homeAndBuildingButtonTransform.DOLocalMove(topRightPosition_Right, 0.5f);
            exitButtonTransform.DOLocalMove(topRightPosition_Left, 0.5f);
        }
        displaying = CRAFTING;
        indicatorTransform.DOMove(craftingButtonTransform.position, 0.5f);


    }

}
