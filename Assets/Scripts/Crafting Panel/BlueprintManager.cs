using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class BlueprintManager : MonoBehaviour
{
    [SerializeField] private RecipeListScriptableObject allBlueprints = null;
    [SerializeField] private List<ItemCraftScriptableObject> obtainedBlueprints = null;
    [SerializeField] private Transform blueprintPanel;
    [Header("Blueprint Card Set Up"),SerializeField] private BlueprintCard blueprintCardTemplate;
    [SerializeField] private Vector2 blueprintCardDelta = new Vector2(0.1f, -0.2f);
    [SerializeField] private Vector2Int blueprintCardGridMax = new Vector2Int(10, 3);
    [Header("Press N to test obtain these blueprints ----------"),SerializeField] private List<ItemCraftScriptableObject> blueprintsToObtain = null;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            ObtainBlueprints();
        }
    }

    public void ObtainBlueprints(List<ItemCraftScriptableObject> newBlueprints)
    {
        blueprintsToObtain = newBlueprints;
        ObtainBlueprints();
    }

    public void ObtainBlueprints()
    {
        foreach (var blueprint in blueprintsToObtain)
        {
            if(obtainedBlueprints.Contains(blueprint)) continue;
            obtainedBlueprints.Add(blueprint);
            AddNewBlueprintCard(blueprint, obtainedBlueprints.Count-1);
        }
    }

    private void AddNewBlueprintCard(ItemCraftScriptableObject icso, int index)
    {
        // Create and Set Position of New Card
        Vector2Int coord = GetBlueprintCoordByAmount(index);
        GameObject newCard = Instantiate(blueprintCardTemplate.gameObject, blueprintPanel);
        newCard.SetActive(true);
        newCard.transform.localPosition = blueprintCardTemplate.transform.localPosition +
                                          new Vector3(coord.x * blueprintCardDelta.x, coord.y * blueprintCardDelta.y, 0);

        newCard.gameObject.name = "" + icso.ItemCrafted + " " + coord.x + "," + coord.y + " -" + (index);
        
        // Adjust Visual of New Card
        newCard.GetComponent<BlueprintCard>().SetUpCardInfo(icso);
    }

    /// <summary>
    /// Get a coord for card that is at the n-th index of the obtainedBlueprints list.
    /// </summary>
    /// <param name="amount">The n of the n-th blueprint to be added.</param>
    /// <returns>Coordinate starting with top-left is (0,0)</returns>
    private Vector2Int GetBlueprintCoordByAmount(int amount)
    {
        return new Vector2Int(amount/blueprintCardGridMax.y, amount % blueprintCardGridMax.y);
    }
}
