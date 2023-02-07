using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    float tCount = 0, tRefresh = 0.1f;
    Vector3 lastTimeScale = new Vector3(0, 0, 0);
    enum Axis {x,y,z}
    [SerializeField] Axis myAxis;
    [SerializeField]Transform targetTransform;
    [SerializeField]float rangeEmpty, rangeFull;
    [SerializeField] bool destroyAfterFull = false;

    /// <summary>
    /// set bar to a certain ratio, return true if it is full.
    /// </summary>
    /// <returns>true if it is full after setting</returns>
    public void setTo(float ratio)
    {
        float setTo = rangeEmpty;
        if(ratio <= 0)
        {
            setTo = rangeEmpty;
        }
        else if(ratio >= 1)
        {
            setTo = rangeFull;
            if (destroyAfterFull)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            setTo = rangeEmpty + (rangeFull - rangeEmpty) * ratio;
        }


        if(myAxis == Axis.x)
        {
            targetTransform.localScale = new Vector3(setTo, targetTransform.localScale.y, targetTransform.localScale.z);
        }
        else if(myAxis == Axis.y)
        {
            targetTransform.localScale = new Vector3(targetTransform.localScale.x, setTo, targetTransform.localScale.z);
        }
        else if(myAxis == Axis.z)
        {
            targetTransform.localScale = new Vector3(targetTransform.localScale.x, targetTransform.localScale.y, setTo);
        }
    }

    void Update()
    {
        //print(tCount + "   " +(lastTimeScale - targetTransform.localScale).magnitude);
        if(tCount > tRefresh)
        {
            if (lastTimeScale == targetTransform.localScale)
            {
                destroySelf();
            }
            else
            {
                tCount = 0;
                lastTimeScale = targetTransform.localScale;
            }
        }
        tCount += Time.deltaTime;
        
    }

    public void destroySelf()
    {
        Destroy(gameObject);
    }
}
