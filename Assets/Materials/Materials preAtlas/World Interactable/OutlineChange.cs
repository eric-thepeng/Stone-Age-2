using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineChange : MonoBehaviour
{
    public Shader targetedShader;
    public Color highlightColor;
    public Color originalColor;
    public float highlightWidth;
    public float originalWidth;

    List<GameObject> listOfChildren = new List<GameObject>();
    void Start()
    {
        GetChildRecursive(gameObject);
    }

    void OnMouseEnter()
    {
        SetOutlineColor(highlightColor, highlightWidth);
    }

    void OnMouseExit()
    {
        SetOutlineColor(originalColor, originalWidth);
    }

    private void SetOutlineColor(Color color, float width)
    {
        foreach (GameObject child in listOfChildren)
        {
            if (child.GetComponent<Renderer>() != null)
            {
                Material[] currentMats = child.GetComponent<Renderer>().materials;
                for (int i2 = 0; i2 < currentMats.Length; i2++)
                {
                    if (currentMats[i2].shader == targetedShader)
                    {
                        currentMats[i2].SetColor("_OutlineColor", color);
                        currentMats[i2].SetFloat("_OutlineSize", width);
                    }
                }
            }
        }
    }

    private void GetChildRecursive(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        foreach (Transform child in obj.transform)
        {
            if (child == null)
                continue;
            listOfChildren.Add(child.gameObject);
            GetChildRecursive(child.gameObject);
        }
    }
}
