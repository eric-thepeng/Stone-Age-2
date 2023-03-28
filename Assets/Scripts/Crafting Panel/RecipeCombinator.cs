using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//The class that is passed on during recursive search to combine all the Tetris together and form a recipe
public class RecipeCombinator
{
    List<Tetris> pastTetris; //The list of Tetris that is already processed
    List<KeyValuePair<Vector2, ItemScriptableObject>> recipeGrid; //The final formed recipe in grid form
    ObjectStackList<CraftingStationScriptableObject> craftingStationCounter;//crafting station in contact
    Tetris origionTetris;

    ItemScriptableObject mergeISO;
    GameObject mergeWindow;

    public RecipeCombinator(Tetris oT)
    {
        origionTetris = oT;
        pastTetris = new List<Tetris>();
        recipeGrid = new List<KeyValuePair<Vector2, ItemScriptableObject>>();
        craftingStationCounter = new ObjectStackList<CraftingStationScriptableObject>();
    }

    /// <summary>
    /// Add this Tetris into the recipe and process it.
    /// </summary>
    /// <param name="baseT">The base Tetris that calls this method.</param>
    /// <param name="newT">The new Tetris to be added to the RecipeCombinator.</param>
    /// <param name="baseCor">Coordination on the Tetris of the connected Edge of the base Tetris.</param>
    /// <param name="dir">Direction of connection from base Tetris to new Tetris</param>
    /// <param name="newCor">Coordination on the Tetris of the connected Edge of the new Tetris.</param>
    public void AddTetris(Tetris baseT, Tetris newT, Vector2 baseCor, Vector2 dir, Vector2 newCor)
    {
        //Avoid Repetition (extra prevention
        if (Searched(newT)) return;

        if (newT.itemSO is CraftingStationScriptableObject)
        {
            craftingStationCounter.AddAmount((CraftingStationScriptableObject)newT.itemSO);
            return;
        }

        pastTetris.Add(newT);
        newT.DestroyRC();
        newT.myRC = this;

        //Is it the first Tetris to be added (the Tetris that create this RecipeCombinator)
        if (baseT == newT)
        {
            newT.recipeFormingDelta = new Vector2(0, 0);
            List<KeyValuePair<Vector2, ItemScriptableObject>> toSearch = newT.itemSO.FormationRecipeCoord;
            foreach (KeyValuePair<Vector2, ItemScriptableObject> kvp in toSearch)
            {
                recipeGrid.Add(kvp);
            }
            return;
        }
        //It is not the first Tetris to be added.
        else
        {
            //Calculate and record Delta
            Vector2 delta = baseCor + dir - newCor + baseT.recipeFormingDelta;
            newT.recipeFormingDelta = delta;

            //Load into final receipe 
            foreach (KeyValuePair<Vector2, ItemScriptableObject> kvp in newT.itemSO.FormationRecipeCoord)
            {
                recipeGrid.Add(new KeyValuePair<Vector2, ItemScriptableObject>(kvp.Key + delta, kvp.Value));
            }
        }
    }

    public void CheckMerging()
    {

        mergeISO = null;

        foreach (ItemCraftScriptableObject icso in origionTetris.recipeListSO.list)
        {
            //print("Check icso");
            //if (!(icso.CraftingStationRequired != null && !CraftingManager.i.TetrisInPresent(icso.CraftingStationRequired)) && icso.CheckMatch(getRecipeGrid())) //find the scriptableobject with same recipe, if there is one
            //{
            if (icso.CheckMatch(getRecipeGrid(), craftingStationCounter))
            {
                //print("FOUND IT!!");
                mergeISO = icso.ItemCrafted;
                break;
            }
        }

        if (mergeISO != null)
        {
            mergeWindow = CraftingManager.i.CreateMergeWindow(this);
        }

    }

    public void DisassembleMerge(Tetris DisassembleFrom)
    {

        DestroyCraftPreview();
        foreach (Tetris t in DisassembleFrom.myRC.pastTetris)
        {
            if (t != DisassembleFrom)
            {
                t.myRC = null;
            }
        }
        foreach (Tetris t in DisassembleFrom.myRC.pastTetris)
        {
            if (t.myRC == null && t != DisassembleFrom)
            {
                t.RefreshEdges(DisassembleFrom);
                t.myRC = new RecipeCombinator(t);
                t.Search(t.myRC, t, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), DisassembleFrom);
                t.myRC.Organize();
                t.myRC.CheckMerging();
            }
        }
        DisassembleFrom.myRC.pastTetris.Clear();
        DisassembleFrom.myRC = null;
    }

    /// <summary>
    /// Has this Tetris been searched and added yet?
    /// </summary>
    /// <param name="t">The Tetris to be checked.</param>
    /// <returns></returns>
    public bool Searched(Tetris t)
    {
        return pastTetris.Contains(t);
    }

    /// <summary>
    /// Get the abstract central position of the Tetris's gameobject.
    /// </summary>
    /// <returns></returns>
    public Vector3 CentralPosition()
    {

        Vector3 topLeft = new Vector3(0, 0, 0);
        Vector3 botRight = new Vector3(0, 0, 0);

        bool first = true;

        foreach (Tetris t in pastTetris)
        {
            Vector3 tPosition = t.transform.position;
            if (first)
            {
                topLeft = tPosition;
                botRight = tPosition;
                first = false;
            }

            Vector3 thisBotRightDelta = t.itemSO.GetBottomRightDelta();
            Vector3 deltaWeight = thisBotRightDelta * t.gameObject.transform.localScale.x;
            deltaWeight = new Vector3(deltaWeight.x, deltaWeight.y / Mathf.Sqrt(2f), deltaWeight.y / Mathf.Sqrt(2f));
            Vector3 thisBotRight = tPosition + deltaWeight;
            Vector3 thisTopLeft = tPosition - deltaWeight;

            //print("processing: " + t.itemSO.tetrisHoverName + " botRightDelta is: " + thisBotRightDelta + " bot right pos: " + thisBotRight + " top left pos: " + thisTopLeft);
            //print("before process, big botRight is: " + botRight + " big topLeft is: " + topLeft);
            if (thisBotRight.x > botRight.x) botRight.x = thisBotRight.x;
            if (thisBotRight.y < botRight.y) { botRight.y = thisBotRight.y; botRight.z = thisBotRight.z; }

            if (thisTopLeft.x < topLeft.x) topLeft.x = thisTopLeft.x;
            if (thisTopLeft.y > topLeft.y) { topLeft.y = thisTopLeft.y; topLeft.z = thisTopLeft.z; }
            //print("after process, big botRight is: " + botRight + " big topLeft is: " + topLeft);
        }

        return (topLeft + botRight) / 2;
    }

    /// <summary>
    /// Organize the formed recipe, move all the nodes so that there are no negative coordinations.
    /// </summary>
    public void Organize()
    {
        Vector2 leftNTopBound = new Vector2(0, 0);
        foreach (KeyValuePair<Vector2, ItemScriptableObject> kvp in recipeGrid)
        {
            if (kvp.Key.x < leftNTopBound.x) leftNTopBound.x = kvp.Key.x;
            if (kvp.Key.y < leftNTopBound.y) leftNTopBound.y = kvp.Key.y;
        }
        List<KeyValuePair<Vector2, ItemScriptableObject>> editedRecipeGrid = new List<KeyValuePair<Vector2, ItemScriptableObject>>();
        foreach (KeyValuePair<Vector2, ItemScriptableObject> kvp in recipeGrid)
        {
            editedRecipeGrid.Add(new KeyValuePair<Vector2, ItemScriptableObject>(kvp.Key - leftNTopBound, kvp.Value));
        }
        recipeGrid = editedRecipeGrid;
    }

    /// <summary>
    /// Does the base Tetris has any other Tetirs connects to it?
    /// </summary>
    /// <returns></returns>
    public bool hasConnector()
    {
        return (pastTetris.Count != 1);
    }

    /// <summary>
    /// Print out a string representaion of all the nodes in the recipe.
    /// </summary>
    /// <returns></returns>
    public void DebugPrint()
    {
        Debug.Log("Debug Print Recipe Combinator");
        foreach (KeyValuePair<Vector2, ItemScriptableObject> kvp in recipeGrid)
        {
            Debug.Log(kvp.Key + " " + kvp.Value.name);
        }
        Debug.Log(craftingStationCounter);
        Debug.Log("End Debug Print Recipe Combinator");
    }

    public void Merge()
    {
        bool mergeFromInventory = true;
        foreach (KeyValuePair<ItemScriptableObject, int> kvp in GetIngredients())
        {
            if (Inventory.i.ItemInStockAmount(kvp.Key) < kvp.Value) mergeFromInventory = false;
        }
        if (mergeFromInventory) //merge from inventory
        {
            foreach (KeyValuePair<ItemScriptableObject, int> kvp in GetIngredients())
            {
                for (int i = kvp.Value; i > 0; i--)
                {
                    Inventory.i.UseItemFromStock(kvp.Key);
                }
            }
            Inventory.i.AddItemToStock(GetMergeISO());
            //2023 02 27 Recipe System to check if there is a unlock // Added by Will
            RecipeMapManager.i.CheckUnlock(GetMergeISO());
        }
        else //merge the inspected one
        {
            origionTetris.StartCoroutine(origionTetris.MergeProgress(this));
            DestroyCraftPreview();
        }

    }

    public void DestroyCraftPreview()
    {
        if (mergeWindow != null)
        {
            MonoBehaviour.Destroy(mergeWindow);
            mergeWindow = null;
        }
    }

    public bool IsOrigionTetris(Tetris thisTetris)
    {
        return thisTetris == origionTetris;
    }

    /// <summary>
    /// Return the grid representation (coordination + so) of recipe.
    /// </summary>
    public List<KeyValuePair<Vector2, ItemScriptableObject>> getRecipeGrid() { return recipeGrid; }

    /// <summary>
    /// Count and return the amount of each iso in the RC
    /// </summary>
    public Dictionary<ItemScriptableObject, int> GetIngredients()
    {
        Dictionary<ItemScriptableObject, int> result = new Dictionary<ItemScriptableObject, int>();
        foreach (KeyValuePair<Vector2, ItemScriptableObject> kvp in getRecipeGrid())
        {
            ItemScriptableObject afterCast = (ItemScriptableObject)kvp.Value;
            if (result.ContainsKey(afterCast))
            {
                result[afterCast] += 1;
            }
            else
            {
                result.Add(afterCast, 1);
            }
        }
        return result;
    }

    /// <summary>
    /// Get all the Tetris that has been processed before.
    /// </summary>
    public List<Tetris> GetPastTetris() { return pastTetris; }

    public ItemScriptableObject GetMergeISO() { return mergeISO; }
}


