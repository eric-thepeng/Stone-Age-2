using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingInformationPanel : MonoBehaviour
{
    class ShadowBoxManager
    {
        private Transform parentGO;
        private Transform boxTemplate;
        private List<Transform> allBoxes;
        private float distance = 0.21f;
        public ShadowBoxManager(Transform parent)
        {
            parentGO = parent;
            boxTemplate = parent.transform.GetChild(0);
            allBoxes = new List<Transform>();
        }
        public void GenerateBoxes(List<Vector2> coords)
        {
            HideBoxes();
            boxTemplate.gameObject.SetActive(true);
            foreach (Vector2 c in coords)
            {
                GameObject newGO = Instantiate(boxTemplate.gameObject, parentGO);
                newGO.transform.localPosition += new Vector3(c.x * distance, -c.y * distance, 0f);
                allBoxes.Add(newGO.transform);
            }
            boxTemplate.gameObject.SetActive(false);
        }

        public void HideBoxes()
        {
            for (int i = allBoxes.Count - 1; i >= 0; i--)
            {
                Destroy(allBoxes[i].gameObject);
            }
            allBoxes.Clear();
        }
        
    }
    
    private ShadowBoxManager shadowBoxManager;
    [SerializeField] private GameObject shadowBoxManagerGameObject;

    private void Start()
    {
        shadowBoxManager = new ShadowBoxManager(shadowBoxManagerGameObject.transform);
    }

    public void DisplayBlueprintCard(BlueprintCard blueprintCard)
    {
        shadowBoxManager.HideBoxes();
        shadowBoxManager.GenerateBoxes(blueprintCard.GetICSO().GetDefaultRecipeCoords());
    }
}

