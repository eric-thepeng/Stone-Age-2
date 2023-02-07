using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskCompleteButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        transform.Find("Button").GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
    }

    private void OnMouseDown()
    {
        transform.Find("Button").GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.3f, 0.3f);
    }

    private void OnMouseUp()
    {
        transform.Find("Button").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        TaskManager.i.ClickComplete();
    }

    private void OnMouseExit()
    {
        transform.Find("Button").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
    }
}
