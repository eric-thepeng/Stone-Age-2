using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface TestInferface
{
    
}

public class Test : MonoBehaviour
{
    public TestInferface TIF;
    private void Start()
    {
        //Quest newQuest = toTest.StartQuest();
        //print(newQuest.GetName());
    }
}
